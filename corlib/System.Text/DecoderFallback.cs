using System;
using System.Collections.Generic;
using System.Text;

namespace System.Text
{
    public abstract class DecoderFallback
    {
        static DecoderFallback exception_fallback =
            new DecoderExceptionFallback();
        static DecoderFallback replacement_fallback =
            new DecoderReplacementFallback();
        static DecoderFallback standard_safe_fallback =
            new DecoderReplacementFallback("\uFFFD");

        protected DecoderFallback()
        {
        }

        public static DecoderFallback ExceptionFallback
        {
            get { return exception_fallback; }
        }

        public abstract int MaxCharCount { get; }

        public static DecoderFallback ReplacementFallback
        {
            get { return replacement_fallback; }
        }

        internal static DecoderFallback StandardSafeFallback
        {
            get { return standard_safe_fallback; }
        }

        public abstract DecoderFallbackBuffer CreateFallbackBuffer();
    }
    public abstract class DecoderFallbackBuffer
    {
        protected DecoderFallbackBuffer()
        {
        }

        public abstract int Remaining { get; }

        public abstract bool Fallback(byte[] bytesUnknown, int index);

        public abstract char GetNextChar();

        public abstract bool MovePrevious();

        public virtual void Reset()
        {
        }
    }
    public sealed class DecoderFallbackException : ArgumentException
    {
        public DecoderFallbackException()
            : this(null)
        {
        }

        public DecoderFallbackException(string message)
            : base(message)
        {
        }

        public DecoderFallbackException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DecoderFallbackException(string message,
            byte[] bytesUnknown, int index)
            : base(message)
        {
            bytes_unknown = bytesUnknown;
            this.index = index;
        }

        byte[] bytes_unknown;
        int index = -1;

        [MonoTODO]
        public byte[] BytesUnknown
        {
            get { return bytes_unknown; }
        }

        [MonoTODO]
        public int Index
        {
            get { return index; }
        }
    }
    public sealed class DecoderExceptionFallback : DecoderFallback
    {
        public DecoderExceptionFallback()
        {
        }

        public override int MaxCharCount
        {
            get { return 0; }
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return new DecoderExceptionFallbackBuffer();
        }

        public override bool Equals(object value)
        {
            return (value is DecoderExceptionFallback);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
    public sealed class DecoderExceptionFallbackBuffer
        : DecoderFallbackBuffer
    {
        public DecoderExceptionFallbackBuffer()
        {
        }

        public override int Remaining
        {
            get { return 0; }
        }

        public override bool Fallback(byte[] bytesUnknown, int index)
        {
            throw new DecoderFallbackException(null, bytesUnknown, index);
        }

        public override char GetNextChar()
        {
            return char.MinValue;
        }

        public override bool MovePrevious()
        {
            return false;
        }
    }
    public sealed class DecoderReplacementFallback : DecoderFallback
    {
        public DecoderReplacementFallback()
            : this("?")
        {
        }

        [MonoTODO]
        public DecoderReplacementFallback(string replacement)
        {
            if (replacement == null)
                throw new ArgumentNullException();
            // FIXME: check replacement validity (invalid surrogate)

            this.replacement = replacement;
        }

        string replacement;

        public string DefaultString
        {
            get { return replacement; }
        }

        public override int MaxCharCount
        {
            get { return replacement.Length; }
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return new DecoderReplacementFallbackBuffer(this);
        }

        public override bool Equals(object value)
        {
            DecoderReplacementFallback f = value as DecoderReplacementFallback;
            return f != null && replacement == f.replacement;
        }

        public override int GetHashCode()
        {
            return replacement.GetHashCode();
        }
    }

    public sealed class DecoderReplacementFallbackBuffer
        : DecoderFallbackBuffer
    {
        bool fallback_assigned;
        int current;
        string replacement;

        public DecoderReplacementFallbackBuffer(
            DecoderReplacementFallback fallback)
        {
            if (fallback == null)
                throw new ArgumentNullException("fallback");
            replacement = fallback.DefaultString;
            current = 0;
        }

        public override int Remaining
        {
            get { return fallback_assigned ? replacement.Length - current : 0; }
        }

        public override bool Fallback(byte[] bytesUnknown, int index)
        {
            if (bytesUnknown == null)
                throw new ArgumentNullException("bytesUnknown");
            if (fallback_assigned && Remaining != 0)
                throw new ArgumentException("Reentrant Fallback method invocation occured. It might be because either this FallbackBuffer is incorrectly shared by multiple threads, invoked inside Encoding recursively, or Reset invocation is forgotten.");
            if (index < 0 || bytesUnknown.Length < index)
                throw new ArgumentOutOfRangeException("index");
            fallback_assigned = true;
            current = 0;

            return replacement.Length > 0;
        }

        public override char GetNextChar()
        {
            if (!fallback_assigned)
                return '\0';
            if (current >= replacement.Length)
                return char.MinValue;
            return replacement[current++];
        }

        public override bool MovePrevious()
        {
            if (current == 0)
                return false;
            current--;
            return true;
        }

        public override void Reset()
        {
            fallback_assigned = false;
            current = 0;
        }
    }
}
