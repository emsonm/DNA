#if !LOCALTEST

namespace System.Text
{
    public class UnicodeEncoding : Encoding
    {

        private class UnicodeDecoder : Decoder
        {

            private bool bigEndian;

            public UnicodeDecoder(bool bigEndian)
            {
                this.bigEndian = bigEndian;
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                throw new NotImplementedException();
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                throw new NotImplementedException();
            }

            //protected override int GetCharsSafe(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, bool flush) {
            //	throw new Exception("The method or operation is not implemented.");
            //}
        }

        private bool bigEndian;
        private bool byteOrderMark;
        private bool throwOnInvalidBytes;

        public UnicodeEncoding() : this(false, true, false) { }

        public UnicodeEncoding(bool bigEndian, bool byteOrderMark) : this(bigEndian, byteOrderMark, false) { }

        public UnicodeEncoding(bool bigEndian, bool byteOrderMark, bool throwOnInvalidBytes)
        {
            this.bigEndian = bigEndian;
            this.byteOrderMark = byteOrderMark;
            this.throwOnInvalidBytes = throwOnInvalidBytes;
        }

        public override byte[] GetPreamble()
        {
            if (byteOrderMark)
            {
                byte[] preamble = new byte[2] { 0xff, 0xfe };
                if (bigEndian)
                {
                    Array.Reverse(preamble);
                }
                return preamble;
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
            return byteCount / 2;
        }

        public override Decoder GetDecoder()
        {
            return new UnicodeDecoder(this.bigEndian);
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new NotImplementedException();
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            throw new NotImplementedException();
        }

        public override int GetMaxByteCount(int charCount)
        {
            throw new NotImplementedException();
        }
    }
}

#endif
