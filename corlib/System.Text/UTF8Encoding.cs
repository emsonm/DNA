#if !LOCALTEST

namespace System.Text
{
    public class UTF8Encoding : Encoding
    {
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            throw new NotImplementedException();
        }
        public override int GetByteCount(char[] chars, int index, int count)
        {
            throw new NotImplementedException();
        }
        public override int CodePage
        {
            get
            {
                return 65001;
            }
        }
        public override string EncodingName
        {
            get
            {
                return "utf-8";
            }
        }
        public override int GetMaxByteCount(int charCount)
        {
            return charCount * 6;
        }
        private class UTF8Encoder : Encoder
        {
            public override int GetByteCount(char[] chars, int index, int count, bool flush)
            {
                throw new NotImplementedException();
            }
            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
            {

                int i = 0;
                int end = 0 + charCount;
                int j = 0;
                int end_bytes = bytes.Length;
                char leftOver = '\0';
                while (i < end)
                {
                    if (leftOver == 0)
                    {
                        for (; i < end; i++)
                        {
                            int ch = chars[i];
                            if (ch < '\x80')
                            {
                                if (j >= end_bytes)
                                    goto fail_no_space;
                                bytes[j] = (byte)ch;
                                j++;
                            }
                            else if (ch < '\x800')
                            {
                                if (j + 1 >= end_bytes)
                                    goto fail_no_space;
                                bytes[j + 0] = (byte)(0xC0 | (ch >> 6));
                                bytes[j + 1] = (byte)(0x80 | (ch & 0x3F));
                                j += 2;
                            }
                            else if (ch < '\uD800' || ch > '\uDFFF')
                            {
                                if (j + 2 >= end_bytes)
                                    goto fail_no_space;
                                bytes[j + 0] = (byte)(0xE0 | (ch >> 12));
                                bytes[j + 1] = (byte)(0x80 | ((ch >> 6) & 0x3F));
                                bytes[j + 2] = (byte)(0x80 | (ch & 0x3F));
                                j += 3;
                            }
                            else if (ch <= '\uDBFF')
                            {
                                // This is a surrogate char, exit the inner loop.
                                leftOver = chars[i];
                                i++;
                                break;
                            }
                            else
                            {
                                // We have a surrogate tail without 
                                // leading surrogate. In NET_2_0 it
                                // uses fallback. In NET_1_1 we output
                                // wrong surrogate.
                                Console.WriteLine("Warning: broken string");
                                // char[] fallback_chars = GetFallbackChars(chars,i, start, fallback, ref buffer);
                                // char dummy = '\0';
                                // if (j + InternalGetByteCount(fallback_chars, 0, fallback_chars.Length, fallback, ref dummy, true) > end_bytes)
                                //     goto fail_no_space;
                                //// fixed (char* fb_chars = fallback_chars)
                                // {
                                //     j += InternalGetBytes(fallback_chars, fallback_chars.Length, bytes, bcount - (int)(j - start_bytes), fallback, ref buffer, ref dummy, true);
                                // }

                                leftOver = '\0';
                            }
                        }
                    }
                    else
                    {
                        if (chars[i] >= '\uDC00' && chars[i] <= '\uDFFF')
                        {
                            // We have a correct surrogate pair.
                            int ch = 0x10000 + (int)chars[i] - 0xDC00 + (((int)leftOver - 0xD800) << 10);
                            if (j + 3 >= end_bytes)
                                goto fail_no_space;
                            bytes[0] = (byte)(0xF0 | (ch >> 18));
                            bytes[1] = (byte)(0x80 | ((ch >> 12) & 0x3F));
                            bytes[2] = (byte)(0x80 | ((ch >> 6) & 0x3F));
                            bytes[3] = (byte)(0x80 | (ch & 0x3F));
                            j += 4;
                            i++;
                        }
                        else
                        {
                            // We have a surrogate start followed by a
                            // regular character.  Technically, this is
                            // invalid, but we have to do something.
                            // We write out the surrogate start and then
                            // re-visit the current character again.
                            Console.WriteLine("Warning: broken string");
                            //        char[] fallback_chars = GetFallbackChars(chars,i, start, fallback, ref buffer);
                            //  char dummy = '\0';
                            //  if (j + InternalGetByteCount(fallback_chars, 0, fallback_chars.Length, fallback, ref dummy, true) > end_bytes)
                            //      goto fail_no_space;
                            ////  fixed (char* fb_chars = fallback_chars)
                            //  {
                            //      InternalGetBytes(fallback_chars, fallback_chars.Length, bytes, bcount - (int)(j - start_bytes), fallback, ref buffer, ref dummy, true);
                            //  }

                            leftOver = '\0';
                        }
                        leftOver = '\0';
                    }
                }
                if (flush)
                {
                    // Flush the left-over surrogate pair start.
                    if (leftOver != '\0')
                    {
                        int ch = leftOver;
                        if (j + 2 < end_bytes)
                        {
                            bytes[j + 0] = (byte)(0xE0 | (ch >> 12));
                            bytes[j + 1] = (byte)(0x80 | ((ch >> 6) & 0x3F));
                            bytes[j + 2] = (byte)(0x80 | (ch & 0x3F));
                            j += 3;
                        }
                        else
                        {
                            goto fail_no_space;
                        }
                        leftOver = '\0';
                    }
                }
                return j;// (int)(j - (end_bytes - bcount));
            fail_no_space:
                throw new ArgumentException("Insufficient Space", "bytes");
            }
        }
        private class UTF8Decoder : Decoder
        {

            private byte b0, b1, b2, b3;
            private int bufOfs = 0;

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                return GetChars(bytes, byteIndex, byteCount, chars, charIndex, true);
            }

            public override int GetCharCount(byte[] bytes, int index, int count, bool flush)
            {
                throw new NotImplementedException();
            }
            public override int GetCharCount(byte[] bytes, int idx, int count)
            {
                throw new NotImplementedException();
            }
            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, bool flush)
            {
                int charIndexOrg = charIndex;
                int charsLen = chars.Length;
                char ch = '\u0000';
                bool chSet = false;
                for (int i = 0; i < byteCount; i++)
                {
                    byte b = bytes[byteIndex + i];
                    switch (bufOfs)
                    {
                        case 0:
                            if (b < 0x80)
                            {
                                ch = (char)b;
                                chSet = true;
                            }
                            else
                            {
                                b0 = b;
                                bufOfs = 1;
                            }
                            break;
                        case 1:
                            if ((b0 & 0xe0) == 0xc0)
                            {
                                ch = (char)(((b0 & 0x1f) << 6) | (b & 0x3f));
                                chSet = true;
                            }
                            else
                            {
                                b1 = b;
                                bufOfs = 2;
                            }
                            break;
                        case 2:
                            if ((b0 & 0xf0) == 0xe0)
                            {
                                ch = (char)(((b0 & 0x0f) << 12) | ((b1 & 0x3f) << 6) | (b & 0x3f));
                                chSet = true;
                            }
                            else
                            {
                                b2 = b;
                                bufOfs = 3;
                            }
                            break;
                        case 3:
                            if ((b0 & 0xf8) == 0xf0)
                            {
                                ch = (char)(((b0 & 0x07) << 18) | ((b1 & 0x3f) << 12) | ((b2 & 0x3f) << 6) | (b & 0x3f));
                                chSet = true;
                            }
                            else
                            {
                                b3 = b;
                                bufOfs = 4;
                            }
                            break;
                        default:
                            throw new NotSupportedException("Cannot handle UTF8 characters more than 4 bytes");
                    }
                    if (chSet)
                    {
                        if (charIndex >= charsLen)
                        {
                            throw new ArgumentException();
                        }
                        chars[charIndex++] = ch;
                        bufOfs = 0;
                        chSet = false;
                    }
                }
                if (flush)
                {
                    bufOfs = 0;
                }
                return charIndex - charIndexOrg;
            }

        }
     


        private bool emitIdentifier;
        private bool throwOnInvalidBytes;
        internal const int UTF8_CODE_PAGE = 65001;
        public UTF8Encoding() : this(false, false) { }

        public UTF8Encoding(bool encoderShouldEmitUTF8Identifier) : this(encoderShouldEmitUTF8Identifier, false) { }

        public UTF8Encoding(bool encoderShouldEmitUTF8Identifier, bool throwOnInvalidBytes)
        {
            this.emitIdentifier = encoderShouldEmitUTF8Identifier;
            this.throwOnInvalidBytes = throwOnInvalidBytes;
        }

        public override byte[] GetPreamble()
        {
            if (emitIdentifier)
            {
                return new byte[3] { 0xef, 0xbb, 0xbf };
            }
            else
            {
                return new byte[0];
            }
        }

        public override int GetMaxCharCount(int byteCount)
        {
            if (byteCount < 0)
            {
                throw new ArgumentOutOfRangeException("byteCount");
            }
            return byteCount;
        }

        public override Decoder GetDecoder()
        {
            return new UTF8Decoder();
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return (new UTF8Decoder()).GetChars(bytes, byteIndex, byteCount, chars, charIndex, true);
        }
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return (new UTF8Encoder()).GetBytes(chars, charIndex, charCount, bytes, byteIndex, true);
        }
    }
    public enum NormalizationForm
    {
        FormC = 1,
        FormD = 2,
        FormKC = 5,
        FormKD = 6
    }

}

#endif


//namespace System.Text
//{

//    using System;
//    using System.Runtime.InteropServices;

//    public enum NormalizationForm
//    {
//        FormC = 1,
//        FormD = 2,
//        FormKC = 5,
//        FormKD = 6
//    }






//    [Serializable]
////    [MonoLimitation("Serialization format not compatible with .NET")]
//  // //[ComVisible(true)]
//    public class UTF8Encoding : Encoding
//    {
//        // Magic number used by Windows for UTF-8.
//        internal const int UTF8_CODE_PAGE = 65001;

//        // Internal state.
//        private bool emitIdentifier;

//        // Constructors.
//        public UTF8Encoding() : this(false, false) { }
//        public UTF8Encoding(bool encoderShouldEmitUTF8Identifier)
//            : this(encoderShouldEmitUTF8Identifier, false) { }

//        public UTF8Encoding(bool encoderShouldEmitUTF8Identifier, bool throwOnInvalidBytes)
//            : base(UTF8_CODE_PAGE)
//        {
//            emitIdentifier = encoderShouldEmitUTF8Identifier;
//            if (throwOnInvalidBytes)
//                SetFallbackInternal(EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
//            else
//                SetFallbackInternal(EncoderFallback.StandardSafeFallback, DecoderFallback.StandardSafeFallback);

//            web_name = body_name = header_name = "utf-8";
//            encoding_name = "Unicode (UTF-8)";
//            is_browser_save = true;
//            is_browser_display = true;
//            is_mail_news_display = true;
//            is_mail_news_save = true;
//            windows_code_page = 65001;// UnicodeEncoding.UNICODE_CODE_PAGE;
//        }

//        #region GetByteCount()


//        private unsafe static int InternalGetByteCount(char[] chars,int startIdx, int count, EncoderFallback fallback, ref char leftOver, bool flush)
//        {
//            // Validate the parameters.
//            if (chars == null)
//            {
//                throw new ArgumentNullException("chars");
//            }
//            if (startIdx < 0 || startIdx > chars.Length)
//            {
//                throw new ArgumentOutOfRangeException("index", _("ArgRange_Array"));
//            }
//            if (count < 0 || count > (chars.Length - startIdx))
//            {
//                throw new ArgumentOutOfRangeException("count", _("ArgRange_Array"));
//            }

//            if (startIdx == chars.Length)
//            {
//                if (flush && leftOver != '\0')
//                {
//                    // Flush the left-over surrogate pair start.
//                    leftOver = '\0';
//                    return 3;
//                }
//                return 0;
//            }

//            int length = 0;
//            int end = startIdx+ count;
//            int start = startIdx;
//            int i = startIdx;
//            EncoderFallbackBuffer buffer = null;
//            while (chars[i] < end)
//            {
//                if (leftOver == 0)
//                {
//                    for (; i < count; i++)
//                    {
//                        if (chars[i] < '\x80')
//                        {
//                            ++length;
//                        }
//                        else if (chars[i] < '\x800')
//                        {
//                            length += 2;
//                        }
//                        else if (chars[i] < '\uD800' || chars[i] > '\uDFFF')
//                        {
//                            length += 3;
//                        }
//                        else if (chars[i] <= '\uDBFF')
//                        {
//                            // This is a surrogate start char, exit the inner loop only
//                            // if we don't find the complete surrogate pair.
//                            if (chars[i + 1] < end && chars[1] >= '\uDC00' && chars[i+1] <= '\uDFFF')
//                            {
//                                length += 4;
//                                i++;
//                                continue;
//                            }
//                            leftOver = chars[i];
//                            i++;
//                            break;
//                        }
//                        else
//                        {
//                            // We have a surrogate tail without 
//                            // leading surrogate. In NET_2_0 it
//                            // uses fallback. In NET_1_1 we output
//                            // wrong surrogate.
//                            char[] fallback_chars = GetFallbackChars(chars,i, start, fallback, ref buffer);
//                          //  fixed (char* fb_chars = fallback_chars)
//                            {
//                                char dummy = '\0';
//                                length += InternalGetByteCount(fallback_chars, i, fallback_chars.Length, fallback, ref dummy, true);
//                            }

//                            leftOver = '\0';
//                        }
//                    }
//                }
//                else
//                {
//                    if (chars[i] >= '\uDC00' &&chars[i] <= '\uDFFF')
//                    {
//                        // We have a correct surrogate pair.
//                        length += 4;
//                        i++;
//                    }
//                    else
//                    {
//                        // We have a surrogate start followed by a
//                        // regular character.  Technically, this is
//                        // invalid, but we have to do something.
//                        // We write out the surrogate start and then
//                        // re-visit the current character again.
//                        char[] fallback_chars = GetFallbackChars(chars, i,start, fallback, ref buffer);
//                       // fixed (char* fb_chars = fallback_chars)
//                        {
//                            char dummy = '\0';
//                            length += InternalGetByteCount(fallback_chars,i, fallback_chars.Length, fallback, ref dummy, true);
//                        }
//                    }
//                    leftOver = '\0';
//                }
//            }
//            if (flush)
//            {
//                // Flush the left-over surrogate pair start.
//                if (leftOver != '\0')
//                {
//                    length += 3;
//                    leftOver = '\0';
//                }
//            }
//            return length;
//        }

//        unsafe static char[] GetFallbackChars(char[] chars, int startIdx, int start, EncoderFallback fallback, ref EncoderFallbackBuffer buffer)
//        {
//            if (buffer == null)
//                buffer = fallback.CreateFallbackBuffer();
//            int j=startIdx;
//            buffer.Fallback(chars[j], (int)(startIdx - start));

//            char[] fallback_chars = new char[buffer.Remaining];
//            for (int i = 0; i < fallback_chars.Length; i++)
//                fallback_chars[i] = buffer.GetNextChar();

//            buffer.Reset();

//            return fallback_chars;
//        }

//        // Get the number of bytes needed to encode a character buffer.
//        public override int GetByteCount(char[] chars, int index, int count)
//        {
//            char dummy = '\0';
//            return InternalGetByteCount(chars, index, count, EncoderFallback, ref dummy, true);
//        }


//      //  [CLSCompliant(false)]
//      //// //[ComVisible(false)]
//      //  public unsafe override int GetByteCount(char* chars, int count)
//      //  {
//      //      if (chars == null)
//      //          throw new ArgumentNullException("chars");
//      //      if (count == 0)
//      //          return 0;
//      //      char dummy = '\0';
//      //      return InternalGetByteCount(chars, count, EncoderFallback, ref dummy, true);
//      //  }

//        #endregion

//        #region GetBytes()

//        // Internal version of "GetBytes" which can handle a rolling
//        // state between multiple calls to this method.
//        private static int InternalGetBytes(char[] chars, int charIndex,
//                             int charCount, byte[] bytes,
//                             int byteIndex,
//                             EncoderFallback fallback, ref EncoderFallbackBuffer buffer,
//                             ref char leftOver, bool flush)
//        {
//            // Validate the parameters.
//            if (chars == null)
//            {
//                throw new ArgumentNullException("chars");
//            }
//            if (bytes == null)
//            {
//                throw new ArgumentNullException("bytes");
//            }
//            if (charIndex < 0 || charIndex > chars.Length)
//            {
//                throw new ArgumentOutOfRangeException("charIndex", _("ArgRange_Array"));
//            }
//            if (charCount < 0 || charCount > (chars.Length - charIndex))
//            {
//                throw new ArgumentOutOfRangeException("charCount", _("ArgRange_Array"));
//            }
//            if (byteIndex < 0 || byteIndex > bytes.Length)
//            {
//                throw new ArgumentOutOfRangeException("byteIndex", _("ArgRange_Array"));
//            }

//            if (charIndex == chars.Length)
//            {
//                if (flush && leftOver != '\0')
//                {
//                    // FIXME: use EncoderFallback.
//                    //
//                    // By default it is empty, so I do nothing for now.
//                    leftOver = '\0';
//                }
//                return 0;
//            }

//            unsafe
//            {
//                fixed (char* cptr = chars)
//                {
//                    if (bytes.Length == byteIndex)
//                        return InternalGetBytes(
//                            cptr + charIndex, charCount,
//                            null, 0, fallback, ref buffer, ref leftOver, flush);
//                    fixed (byte* bptr = bytes)
//                    {
//                        return InternalGetBytes(
//                            cptr + charIndex, charCount,
//                            bptr + byteIndex, bytes.Length - byteIndex,
//                            fallback, ref buffer,
//                            ref leftOver, flush);
//                    }
//                }
//            }
//        }



//        // Get the bytes that result from encoding a character buffer.
//        public override int GetBytes(char[] chars, int charIndex, int charCount,
//                                     byte[] bytes, int byteIndex)
//        {
//            char leftOver = '\0';
//            EncoderFallbackBuffer buffer = null;
//            return InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, EncoderFallback, ref buffer, ref leftOver, true);
//        }

//        // Convenience wrappers for "GetBytes".
//        //public override int GetBytes(String s, int charIndex, int charCount,
//        //                             byte[] bytes, int byteIndex)
//        //{
//        //    // Validate the parameters.
//        //    if (s == null)
//        //    {
//        //        throw new ArgumentNullException("s");
//        //    }
//        //    if (bytes == null)
//        //    {
//        //        throw new ArgumentNullException("bytes");
//        //    }
//        //    if (charIndex < 0 || charIndex > s.Length)
//        //    {
//        //        throw new ArgumentOutOfRangeException("charIndex", _("ArgRange_StringIndex"));
//        //    }
//        //    if (charCount < 0 || charCount > (s.Length - charIndex))
//        //    {
//        //        throw new ArgumentOutOfRangeException("charCount", _("ArgRange_StringRange"));
//        //    }
//        //    if (byteIndex < 0 || byteIndex > bytes.Length)
//        //    {
//        //        throw new ArgumentOutOfRangeException("byteIndex", _("ArgRange_Array"));
//        //    }

//        //    if (charIndex == s.Length)
//        //        return 0;

//        //    unsafe
//        //    {
//        //        fixed (char* cptr = s)
//        //        {
//        //            char dummy = '\0';
//        //            EncoderFallbackBuffer buffer = null;
//        //            if (bytes.Length == byteIndex)
//        //                return InternalGetBytes(
//        //                    cptr + charIndex, charCount,
//        //                    null, 0, EncoderFallback, ref buffer, ref dummy, true);
//        //            fixed (byte* bptr = bytes)
//        //            {
//        //                return InternalGetBytes(
//        //                    cptr + charIndex, charCount,
//        //                    bptr + byteIndex, bytes.Length - byteIndex,
//        //                    EncoderFallback, ref buffer,
//        //                    ref dummy, true);
//        //            }
//        //        }
//        //    }
//        //}

//        [CLSCompliant(false)]
//       //[ComVisible(false)]
//        public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
//        {
//            if (chars == null)
//                throw new ArgumentNullException("chars");
//            if (charCount < 0)
//                throw new IndexOutOfRangeException("charCount");
//            if (bytes == null)
//                throw new ArgumentNullException("bytes");
//            if (byteCount < 0)
//                throw new IndexOutOfRangeException("charCount");

//            if (charCount == 0)
//                return 0;

//            char dummy = '\0';
//            EncoderFallbackBuffer buffer = null;
//            if (byteCount == 0)
//                return InternalGetBytes(chars, charCount, null, 0, EncoderFallback, ref buffer, ref dummy, true);
//            else
//                return InternalGetBytes(chars, charCount, bytes, byteCount, EncoderFallback, ref buffer, ref dummy, true);
//        }

//        #endregion

//        // Internal version of "GetCharCount" which can handle a rolling
//        // state between multiple calls to this method.
//        private unsafe static int InternalGetCharCount(
//            byte[] bytes, int index, int count, uint leftOverBits,
//            uint leftOverCount, object provider,
//            ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
//        {
//            // Validate the parameters.
//            if (bytes == null)
//            {
//                throw new ArgumentNullException("bytes");
//            }
//            if (index < 0 || index > bytes.Length)
//            {
//                throw new ArgumentOutOfRangeException("index", _("ArgRange_Array"));
//            }
//            if (count < 0 || count > (bytes.Length - index))
//            {
//                throw new ArgumentOutOfRangeException("count", _("ArgRange_Array"));
//            }

//            if (count == 0)
//                return 0;
//            fixed (byte* bptr = bytes)
//                return InternalGetCharCount(bptr + index, count,
//                    leftOverBits, leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
//        }

//        private unsafe static int InternalGetCharCount(
//            byte* bytes, int count, uint leftOverBits,
//            uint leftOverCount, object provider,
//            ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
//        {
//            int index = 0;

//            int length = 0;

//            if (leftOverCount == 0)
//            {
//                int end = index + count;
//                for (; index < end; index++, count--)
//                {
//                    if (bytes[index] < 0x80)
//                        length++;
//                    else
//                        break;
//                }
//            }

//            // Determine the number of characters that we have.
//            uint ch;
//            uint leftBits = leftOverBits;
//            uint leftSoFar = (leftOverCount & (uint)0x0F);
//            uint leftSize = ((leftOverCount >> 4) & (uint)0x0F);
//            while (count > 0)
//            {
//                ch = (uint)(bytes[index++]);
//                --count;
//                if (leftSize == 0)
//                {
//                    // Process a UTF-8 start character.
//                    if (ch < (uint)0x0080)
//                    {
//                        // Single-byte UTF-8 character.
//                        ++length;
//                    }
//                    else if ((ch & (uint)0xE0) == (uint)0xC0)
//                    {
//                        // Double-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x1F);
//                        leftSoFar = 1;
//                        leftSize = 2;
//                    }
//                    else if ((ch & (uint)0xF0) == (uint)0xE0)
//                    {
//                        // Three-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x0F);
//                        leftSoFar = 1;
//                        leftSize = 3;
//                    }
//                    else if ((ch & (uint)0xF8) == (uint)0xF0)
//                    {
//                        // Four-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x07);
//                        leftSoFar = 1;
//                        leftSize = 4;
//                    }
//                    else if ((ch & (uint)0xFC) == (uint)0xF8)
//                    {
//                        // Five-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x03);
//                        leftSoFar = 1;
//                        leftSize = 5;
//                    }
//                    else if ((ch & (uint)0xFE) == (uint)0xFC)
//                    {
//                        // Six-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x03);
//                        leftSoFar = 1;
//                        leftSize = 6;
//                    }
//                    else
//                    {
//                        // Invalid UTF-8 start character.
//                        length += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, index - 1, 1);
//                    }
//                }
//                else
//                {
//                    // Process an extra byte in a multi-byte sequence.
//                    if ((ch & (uint)0xC0) == (uint)0x80)
//                    {
//                        leftBits = ((leftBits << 6) | (ch & (uint)0x3F));
//                        if (++leftSoFar >= leftSize)
//                        {
//                            // We have a complete character now.
//                            if (leftBits < (uint)0x10000)
//                            {
//                                // is it an overlong ?
//                                bool overlong = false;
//                                switch (leftSize)
//                                {
//                                    case 2:
//                                        overlong = (leftBits <= 0x7F);
//                                        break;
//                                    case 3:
//                                        overlong = (leftBits <= 0x07FF);
//                                        break;
//                                    case 4:
//                                        overlong = (leftBits <= 0xFFFF);
//                                        break;
//                                    case 5:
//                                        overlong = (leftBits <= 0x1FFFFF);
//                                        break;
//                                    case 6:
//                                        overlong = (leftBits <= 0x03FFFFFF);
//                                        break;
//                                }
//                                if (overlong)
//                                {
//                                    length += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, index - leftSoFar, leftSoFar);
//                                }
//                                else if ((leftBits & 0xF800) == 0xD800)
//                                {
//                                    // UTF-8 doesn't use surrogate characters
//                                    length += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, index - leftSoFar, leftSoFar);
//                                }
//                                else
//                                    ++length;
//                            }
//                            else if (leftBits < (uint)0x110000)
//                            {
//                                length += 2;
//                            }
//                            else
//                            {
//                                length += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, index - leftSoFar, leftSoFar);
//                            }
//                            leftSize = 0;
//                        }
//                    }
//                    else
//                    {
//                        // Invalid UTF-8 sequence: clear and restart.
//                        length += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, index - leftSoFar, leftSoFar);
//                        leftSize = 0;
//                        --index;
//                        ++count;
//                    }
//                }
//            }
//            if (flush && leftSize != 0)
//            {
//                // We had left-over bytes that didn't make up
//                // a complete UTF-8 character sequence.
//                length += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, index - leftSoFar, leftSoFar);
//            }

//            // Return the final length to the caller.
//            return length;
//        }

//        // for GetCharCount()
//        static unsafe int Fallback(object provider, ref DecoderFallbackBuffer buffer, ref byte[] bufferArg, byte* bytes, long index, uint size)
//        {
//            if (buffer == null)
//            {
//                DecoderFallback fb = provider as DecoderFallback;
//                if (fb != null)
//                    buffer = fb.CreateFallbackBuffer();
//                else
//                    buffer = ((Decoder)provider).FallbackBuffer;
//            }
//            if (bufferArg == null)
//                bufferArg = new byte[1];
//            int ret = 0;
//            for (int i = 0; i < size; i++)
//            {
//                bufferArg[0] = bytes[(int)index + i];
//                buffer.Fallback(bufferArg, 0);
//                ret += buffer.Remaining;
//                buffer.Reset();
//            }
//            return ret;
//        }

//        // for GetChars()
//        static unsafe void Fallback(object provider, ref DecoderFallbackBuffer buffer, ref byte[] bufferArg, byte[] bytes, long byteIndex, uint size,
//            char* chars, ref int charIndex)
//        {
//            if (buffer == null)
//            {
//                DecoderFallback fb = provider as DecoderFallback;
//                if (fb != null)
//                    buffer = fb.CreateFallbackBuffer();
//                else
//                    buffer = ((Decoder)provider).FallbackBuffer;
//            }
//            if (bufferArg == null)
//                bufferArg = new byte[1];
//            for (int i = 0; i < size; i++)
//            {
//                bufferArg[0] = bytes[byteIndex + i];
//                buffer.Fallback(bufferArg, 0);
//                while (buffer.Remaining > 0)
//                    chars[charIndex++] = buffer.GetNextChar();
//                buffer.Reset();
//            }
//        }

//        // Get the number of characters needed to decode a byte buffer.
//        public override int GetCharCount(byte[] bytes, int index, int count)
//        {
//            DecoderFallbackBuffer buf = null;
//            byte[] bufferArg = null;
//            return InternalGetCharCount(bytes, index, count, 0, 0, DecoderFallback, ref buf, ref bufferArg, true);
//        }

//        [CLSCompliant(false)]
//       //[ComVisible(false)]
//        public unsafe override int GetCharCount(byte* bytes, int count)
//        {
//            DecoderFallbackBuffer buf = null;
//            byte[] bufferArg = null;
//            return InternalGetCharCount(bytes, count, 0, 0, DecoderFallback, ref buf, ref bufferArg, true);
//        }

//        // Get the characters that result from decoding a byte buffer.
//        private unsafe static int InternalGetChars(
//            byte[] bytes, int byteIndex, int byteCount, char[] chars,
//            int charIndex, ref uint leftOverBits, ref uint leftOverCount,
//            object provider,
//            ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
//        {
//            // Validate the parameters.
//            if (bytes == null)
//            {
//                throw new ArgumentNullException("bytes");
//            }
//            if (chars == null)
//            {
//                throw new ArgumentNullException("chars");
//            }
//            if (byteIndex < 0 || byteIndex > bytes.Length)
//            {
//                throw new ArgumentOutOfRangeException("byteIndex", _("ArgRange_Array"));
//            }
//            if (byteCount < 0 || byteCount > (bytes.Length - byteIndex))
//            {
//                throw new ArgumentOutOfRangeException("byteCount", _("ArgRange_Array"));
//            }
//            if (charIndex < 0 || charIndex > chars.Length)
//            {
//                throw new ArgumentOutOfRangeException("charIndex", _("ArgRange_Array"));
//            }

//            if (charIndex == chars.Length)
//                return 0;

//            //fixed (char* cptr = chars)
//            {
//                if (byteCount == 0 || byteIndex == bytes.Length)
//                    return InternalGetChars(null, 0, cptr + charIndex, chars.Length - charIndex, ref leftOverBits, ref leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
//                // otherwise...
//                fixed (byte* bptr = bytes)
//                    return InternalGetChars(bptr + byteIndex, byteCount, cptr + charIndex, chars.Length - charIndex, ref leftOverBits, ref leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
//            }
//        }

//        private unsafe static int InternalGetChars(
//            byte[] bytes, int byteCount, char[] chars, int charCount,
//            ref uint leftOverBits, ref uint leftOverCount,
//            object provider,
//            ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
//        {
//            int charIndex = 0, byteIndex = 0;
//            int length = charCount;
//            int posn = charIndex;

//            if (leftOverCount == 0)
//            {
//                int end = byteIndex + byteCount;
//                for (; byteIndex < end; posn++, byteIndex++, byteCount--)
//                {
//                    if (bytes[byteIndex] < 0x80)
//                        chars[posn] = (char)bytes[byteIndex];
//                    else
//                        break;
//                }
//            }

//            // Convert the bytes into the output buffer.
//            uint ch;
//            uint leftBits = leftOverBits;
//            uint leftSoFar = (leftOverCount & (uint)0x0F);
//            uint leftSize = ((leftOverCount >> 4) & (uint)0x0F);

//            int byteEnd = byteIndex + byteCount;
//            for (; byteIndex < byteEnd; byteIndex++)
//            {
//                // Fetch the next character from the byte buffer.
//                ch = (uint)(bytes[byteIndex]);
//                if (leftSize == 0)
//                {
//                    // Process a UTF-8 start character.
//                    if (ch < (uint)0x0080)
//                    {
//                        // Single-byte UTF-8 character.
//                        if (posn >= length)
//                        {
//                            throw new ArgumentException(("Arg_InsufficientSpace"), "chars");
//                        }
//                        chars[posn++] = (char)ch;
//                    }
//                    else if ((ch & (uint)0xE0) == (uint)0xC0)
//                    {
//                        // Double-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x1F);
//                        leftSoFar = 1;
//                        leftSize = 2;
//                    }
//                    else if ((ch & (uint)0xF0) == (uint)0xE0)
//                    {
//                        // Three-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x0F);
//                        leftSoFar = 1;
//                        leftSize = 3;
//                    }
//                    else if ((ch & (uint)0xF8) == (uint)0xF0)
//                    {
//                        // Four-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x07);
//                        leftSoFar = 1;
//                        leftSize = 4;
//                    }
//                    else if ((ch & (uint)0xFC) == (uint)0xF8)
//                    {
//                        // Five-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x03);
//                        leftSoFar = 1;
//                        leftSize = 5;
//                    }
//                    else if ((ch & (uint)0xFE) == (uint)0xFC)
//                    {
//                        // Six-byte UTF-8 character.
//                        leftBits = (ch & (uint)0x03);
//                        leftSoFar = 1;
//                        leftSize = 6;
//                    }
//                    else
//                    {
//                        // Invalid UTF-8 start character.
//                        Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, byteIndex, 1, chars, ref posn);
//                    }
//                }
//                else
//                {
//                    // Process an extra byte in a multi-byte sequence.
//                    if ((ch & (uint)0xC0) == (uint)0x80)
//                    {
//                        leftBits = ((leftBits << 6) | (ch & (uint)0x3F));
//                        if (++leftSoFar >= leftSize)
//                        {
//                            // We have a complete character now.
//                            if (leftBits < (uint)0x10000)
//                            {
//                                // is it an overlong ?
//                                bool overlong = false;
//                                switch (leftSize)
//                                {
//                                    case 2:
//                                        overlong = (leftBits <= 0x7F);
//                                        break;
//                                    case 3:
//                                        overlong = (leftBits <= 0x07FF);
//                                        break;
//                                    case 4:
//                                        overlong = (leftBits <= 0xFFFF);
//                                        break;
//                                    case 5:
//                                        overlong = (leftBits <= 0x1FFFFF);
//                                        break;
//                                    case 6:
//                                        overlong = (leftBits <= 0x03FFFFFF);
//                                        break;
//                                }
//                                if (overlong)
//                                {
//                                    Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, byteIndex - leftSoFar, leftSoFar, chars, ref posn);
//                                }
//                                else if ((leftBits & 0xF800) == 0xD800)
//                                {
//                                    // UTF-8 doesn't use surrogate characters
//                                    Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, byteIndex - leftSoFar, leftSoFar, chars, ref posn);
//                                }
//                                else
//                                {
//                                    if (posn >= length)
//                                    {
//                                        throw new ArgumentException
//                                            (("Arg_InsufficientSpace"), "chars");
//                                    }
//                                    chars[posn++] = (char)leftBits;
//                                }
//                            }
//                            else if (leftBits < (uint)0x110000)
//                            {
//                                if ((posn + 2) > length)
//                                {
//                                    throw new ArgumentException
//                                        (("Arg_InsufficientSpace"), "chars");
//                                }
//                                leftBits -= (uint)0x10000;
//                                chars[posn++] = (char)((leftBits >> 10) +
//                                                       (uint)0xD800);
//                                chars[posn++] =
//                                    (char)((leftBits & (uint)0x3FF) + (uint)0xDC00);
//                            }
//                            else
//                            {
//                                Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, byteIndex - leftSoFar, leftSoFar, chars, ref posn);
//                            }
//                            leftSize = 0;
//                        }
//                    }
//                    else
//                    {
//                        // Invalid UTF-8 sequence: clear and restart.
//                        Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, byteIndex - leftSoFar, leftSoFar, chars, ref posn);
//                        leftSize = 0;
//                        --byteIndex;
//                    }
//                }
//            }
//            if (flush && leftSize != 0)
//            {
//                // We had left-over bytes that didn't make up
//                // a complete UTF-8 character sequence.
//                Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, byteIndex - leftSoFar, leftSoFar, chars, ref posn);
//            }
//            leftOverBits = leftBits;
//            leftOverCount = (leftSoFar | (leftSize << 4));

//            // Return the final length to the caller.
//            return posn - charIndex;
//        }

//        // Get the characters that result from decoding a byte buffer.
//        public override int GetChars(byte[] bytes, int byteIndex, int byteCount,
//                                     char[] chars, int charIndex)
//        {
//            uint leftOverBits = 0;
//            uint leftOverCount = 0;
//            DecoderFallbackBuffer buf = null;
//            byte[] bufferArg = null;
//            return InternalGetChars(bytes, byteIndex, byteCount, chars,
//                    charIndex, ref leftOverBits, ref leftOverCount, DecoderFallback, ref buf, ref bufferArg, true);
//        }

//       // [CLSCompliant(false)]
//       ////[ComVisible(false)]
//       // public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
//       // {
//       //     DecoderFallbackBuffer buf = null;
//       //     byte[] bufferArg = null;
//       //     uint leftOverBits = 0;
//       //     uint leftOverCount = 0;
//       //     return InternalGetChars(bytes, byteCount, chars,
//       //             charCount, ref leftOverBits, ref leftOverCount, DecoderFallback, ref buf, ref bufferArg, true);
//       // }

//        // Get the maximum number of bytes needed to encode a
//        // specified number of characters.
//        public override int GetMaxByteCount(int charCount)
//        {
//            if (charCount < 0)
//            {
//                throw new ArgumentOutOfRangeException("charCount", _("ArgRange_NonNegative"));
//            }
//            return charCount * 4;
//        }

//        // Get the maximum number of characters needed to decode a
//        // specified number of bytes.
//        public override int GetMaxCharCount(int byteCount)
//        {
//            if (byteCount < 0)
//            {
//                throw new ArgumentOutOfRangeException("byteCount", _("ArgRange_NonNegative"));
//            }
//            return byteCount;
//        }

//        // Get a UTF8-specific decoder that is attached to this instance.
//        public override Decoder GetDecoder()
//        {
//            return new UTF8Decoder(DecoderFallback);
//        }

//        // Get a UTF8-specific encoder that is attached to this instance.
//        public override Encoder GetEncoder()
//        {
//            return new UTF8Encoder(EncoderFallback, emitIdentifier);
//        }

//        // Get the UTF8 preamble.
//        public override byte[] GetPreamble()
//        {
//            if (emitIdentifier)
//                return new byte[] { 0xEF, 0xBB, 0xBF };

//            return new byte[0];
//        }

//        // Determine if this object is equal to another.
//        public override bool Equals(Object value)
//        {
//            UTF8Encoding enc = (value as UTF8Encoding);
//            if (enc != null)
//            {
//                return (codePage == enc.codePage &&
//                    emitIdentifier == enc.emitIdentifier &&
//                    DecoderFallback.Equals(enc.DecoderFallback) &&
//                    EncoderFallback.Equals(enc.EncoderFallback));
//            }
//            else
//            {
//                return false;
//            }
//        }

//        // Get the hash code for this object.
//        public override int GetHashCode()
//        {
//            return base.GetHashCode();
//        }

//        public override int GetByteCount(string chars)
//        {
//            // hmm, does this override make any sense?
//            return base.GetByteCount(chars);
//        }

//       //[ComVisible(false)]
//        public override string GetString(byte[] bytes, int index, int count)
//        {
//            // hmm, does this override make any sense?
//            return base.GetString(bytes, index, count);
//        }

//        // UTF-8 decoder implementation.
//        [Serializable]
//        private class UTF8Decoder : Decoder
//        {
//            private uint leftOverBits;
//            private uint leftOverCount;

//            // Constructor.
//            public UTF8Decoder(DecoderFallback fallback)
//            {
//                Fallback = fallback;
//                leftOverBits = 0;
//                leftOverCount = 0;
//            }

//            // Override inherited methods.
//            public override int GetCharCount(byte[] bytes, int index, int count)
//            {
//                DecoderFallbackBuffer buf = null;
//                byte[] bufferArg = null;
//                return InternalGetCharCount(bytes, index, count,
//                    leftOverBits, leftOverCount, this, ref buf, ref bufferArg, false);
//            }
//            public override int GetChars(byte[] bytes, int byteIndex,
//                             int byteCount, char[] chars, int charIndex)
//            {
//                DecoderFallbackBuffer buf = null;
//                byte[] bufferArg = null;
//                return InternalGetChars(bytes, byteIndex, byteCount,
//                    chars, charIndex, ref leftOverBits, ref leftOverCount, this, ref buf, ref bufferArg, false);
//            }

//        } // class UTF8Decoder

//        // UTF-8 encoder implementation.
//        [Serializable]
//        private class UTF8Encoder : Encoder
//        {
//            //		private bool emitIdentifier;
//            private char leftOverForCount;
//            private char leftOverForConv;

//            // Constructor.
//            public UTF8Encoder(EncoderFallback fallback, bool emitIdentifier)
//            {
//                Fallback = fallback;
//                //			this.emitIdentifier = emitIdentifier;
//                leftOverForCount = '\0';
//                leftOverForConv = '\0';
//            }

//            // Override inherited methods.
//            public override int GetByteCount(char[] chars, int index,
//                         int count, bool flush)
//            {
//                return InternalGetByteCount(chars, index, count, Fallback, ref leftOverForCount, flush);
//            }
//            public override int GetBytes(char[] chars, int charIndex,
//                         int charCount, byte[] bytes, int byteIndex, bool flush)
//            {
//                int result;
//                EncoderFallbackBuffer buffer = null;
//                result = InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, Fallback, ref buffer, ref leftOverForConv, flush);
//                //			emitIdentifier = false;
//                return result;
//            }

//            //public unsafe override int GetByteCount(char* chars, int count, bool flush)
//            //{
//            //    return InternalGetByteCount(chars, count, Fallback, ref leftOverForCount, flush);
//            //}

//            //public unsafe override int GetBytes(char* chars, int charCount,
//            //    byte* bytes, int byteCount, bool flush)
//            //{
//            //    int result;
//            //    EncoderFallbackBuffer buffer = null;
//            //    result = InternalGetBytes(chars, charCount, bytes, byteCount, Fallback, ref buffer, ref leftOverForConv, flush);
//            //    //			emitIdentifier = false;
//            //    return result;
//            //}
//        } // class UTF8Encoder

//    }; // class UTF8Encoding

//}; // namespace System.Text
