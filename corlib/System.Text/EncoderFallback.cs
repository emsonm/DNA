using System;
using System.Collections.Generic;
using System.Text;

namespace System.Text
{
    public abstract class EncoderFallback
    {
        static EncoderFallback exception_fallback =
            new EncoderExceptionFallback();
        static EncoderFallback replacement_fallback =
            new EncoderReplacementFallback();
        static EncoderFallback standard_safe_fallback =
            new EncoderReplacementFallback("\uFFFD");

        protected EncoderFallback()
        {
        }

        public static EncoderFallback ExceptionFallback
        {
            get { return exception_fallback; }
        }

        public abstract int MaxCharCount { get; }

        public static EncoderFallback ReplacementFallback
        {
            get { return replacement_fallback; }
        }

        internal static EncoderFallback StandardSafeFallback
        {
            get { return standard_safe_fallback; }
        }

        public abstract EncoderFallbackBuffer CreateFallbackBuffer();
    }

    public abstract class EncoderFallbackBuffer
    {
        protected EncoderFallbackBuffer()
        {
        }

        public abstract int Remaining { get; }

        public abstract bool Fallback(char charUnknown, int index);

        public abstract bool Fallback(char charUnknownHigh, char charUnknownLow, int index);

        public abstract char GetNextChar();

        public abstract bool MovePrevious();

        public virtual void Reset()
        {
            while (GetNextChar() != '\0')
                ;
        }
    }

    public sealed class EncoderExceptionFallback : EncoderFallback
    {
        public EncoderExceptionFallback()
        {
        }

        public override int MaxCharCount
        {
            get { return 0; }
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new EncoderExceptionFallbackBuffer();
        }

        public override bool Equals(object value)
        {
            return (value is EncoderExceptionFallback);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public sealed class EncoderExceptionFallbackBuffer
        : EncoderFallbackBuffer
    {
        public EncoderExceptionFallbackBuffer()
        {
        }

        public override int Remaining
        {
            get { return 0; }
        }

        public override bool Fallback(char charUnknown, int index)
        {
            throw new EncoderFallbackException(charUnknown, index);
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            throw new EncoderFallbackException(charUnknownHigh, charUnknownLow, index);
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

    public sealed class EncoderReplacementFallback : EncoderFallback
    {
        public EncoderReplacementFallback()
            : this("?")
        {
        }

        [MonoTODO]
        public EncoderReplacementFallback(string replacement)
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

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new EncoderReplacementFallbackBuffer(this);
        }

        public override bool Equals(object value)
        {
            EncoderReplacementFallback f = value as EncoderReplacementFallback;
            return f != null && replacement == f.replacement;
        }

        public override int GetHashCode()
        {
            return replacement.GetHashCode();
        }
    }
    public sealed class EncoderReplacementFallbackBuffer
        : EncoderFallbackBuffer
    {
        string replacement;
        int current;
        bool fallback_assigned;

        public EncoderReplacementFallbackBuffer(
            EncoderReplacementFallback fallback)
        {
            if (fallback == null)
                throw new ArgumentNullException("fallback");
            replacement = fallback.DefaultString;
            current = 0;
        }

        public override int Remaining
        {
            get { return replacement.Length - current; }
        }

        public override bool Fallback(char charUnknown, int index)
        {
            return Fallback(index);
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            return Fallback(index);
        }

        // hmm, what is this index for???
        private bool Fallback(int index)
        {
            if (fallback_assigned && Remaining != 0)
                throw new ArgumentException("Reentrant Fallback method invocation occured. It might be because either this FallbackBuffer is incorrectly shared by multiple threads, invoked inside Encoding recursively, or Reset invocation is forgotten.");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            fallback_assigned = true;
            current = 0;

            return replacement.Length > 0;
        }

        public override char GetNextChar()
        {
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
            current = 0;
        }
    }
    public sealed class EncoderFallbackException : ArgumentException
    {
        public EncoderFallbackException()
            : this(null)
        {
        }

        public EncoderFallbackException(string message)
            : base(message)
        {
        }

        public EncoderFallbackException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal EncoderFallbackException(char charUnknown, int index)
            : base(null)
        {
            char_unknown = charUnknown;
            this.index = index;
        }

        internal EncoderFallbackException(char charUnknownHigh,
            char charUnknownLow, int index)
            : base(null)
        {
            char_unknown_high = charUnknownHigh;
            char_unknown_low = charUnknownLow;
            this.index = index;
        }

        char char_unknown, char_unknown_high, char_unknown_low;
        int index = -1;

        public char CharUnknown
        {
            get { return char_unknown; }
        }

        public char CharUnknownHigh
        {
            get { return char_unknown_high; }
        }

        public char CharUnknownLow
        {
            get { return char_unknown_low; }
        }

        [MonoTODO]
        public int Index
        {
            get { return index; }
        }

        [MonoTODO]
        public bool IsUnknownSurrogate()
        {
            throw new NotImplementedException();
        }
    }
}
