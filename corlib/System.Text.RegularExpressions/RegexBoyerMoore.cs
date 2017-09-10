namespace System.Text.RegularExpressions
{
    using System;
    using System.Globalization;

    internal sealed class RegexBoyerMoore
    {
        internal bool _caseInsensitive;
        internal CultureInfo _culture;
        internal int _highASCII;
        internal int _lowASCII;
        internal int[] _negativeASCII;
        internal int[][] _negativeUnicode;
        internal string _pattern;
        internal int[] _positive;
        internal bool _rightToLeft;
        internal const int infinite = 0x7fffffff;

        internal RegexBoyerMoore(string pattern, bool caseInsensitive, bool rightToLeft, CultureInfo culture)
        {
            int length;
            int num2;
            int num3;
            int num6;
            if (pattern.Length == 0)
            {
                throw new NotSupportedException();
            }
            if (caseInsensitive)
            {
                pattern = pattern.ToLower(culture);
            }
            this._pattern = pattern;
            this._rightToLeft = rightToLeft;
            this._caseInsensitive = caseInsensitive;
            this._culture = culture;
            if (!rightToLeft)
            {
                length = -1;
                num2 = pattern.Length - 1;
                num3 = 1;
            }
            else
            {
                length = pattern.Length;
                num2 = 0;
                num3 = -1;
            }
            this._positive = new int[pattern.Length];
            int index = num2;
            char ch = pattern[index];
            this._positive[index] = num3;
            index -= num3;
        Label_0084:
            if (index == length)
            {
                for (num6 = num2 - num3; num6 != length; num6 -= num3)
                {
                    if (this._positive[num6] == 0)
                    {
                        this._positive[num6] = num2 - length;
                    }
                }
                this._negativeASCII = new int[0x80];
                for (int i = 0; i < 0x80; i++)
                {
                    this._negativeASCII[i] = num2 - length;
                }
                this._lowASCII = 0x7f;
                this._highASCII = 0;
                for (index = num2; index != length; index -= num3)
                {
                    ch = pattern[index];
                    if ((ch >= '\0') && (ch < '\x0080'))
                    {
                        if (this._lowASCII > ch)
                        {
                            this._lowASCII = ch;
                        }
                        if (this._highASCII < ch)
                        {
                            this._highASCII = ch;
                        }
                        if (this._negativeASCII[ch] == (num2 - length))
                        {
                            this._negativeASCII[ch] = num2 - index;
                        }
                    }
                    else
                    {
                        int num8 = ch >> 8;
                        int num9 = ch & '\x00ff';
                        if (this._negativeUnicode == null)
                        {
                            this._negativeUnicode = new int[0x100][];
                            for (int j = 0; j < 0x100; j++)
                            {
                                this._negativeUnicode[j] = null;
                            }
                        }
                        if (this._negativeUnicode[num8] == null)
                        {
                            int[] destinationArray = new int[0x100];
                            for (int k = 0; k < 0x100; k++)
                            {
                                destinationArray[k] = num2 - length;
                            }
                            if (num8 == 0)
                            {
                                Array.Copy(this._negativeASCII, 0, destinationArray, 0, 0x80);
                                this._negativeASCII = destinationArray;
                            }
                            this._negativeUnicode[num8] = destinationArray;
                        }
                        if (this._negativeUnicode[num8][num9] == (num2 - length))
                        {
                            this._negativeUnicode[num8][num9] = num2 - index;
                        }
                    }
                }
            }
            else
            {
                if (pattern[index] != ch)
                {
                    index -= num3;
                }
                else
                {
                    num6 = num2;
                    int num5 = index;
                    while (true)
                    {
                        if ((num5 == length) || (pattern[num6] != pattern[num5]))
                        {
                            if (this._positive[num6] == 0)
                            {
                                this._positive[num6] = num6 - num5;
                            }
                            break;
                        }
                        num5 -= num3;
                        num6 -= num3;
                    }
                    index -= num3;
                }
                goto Label_0084;
            }
        }
        //HACK:wzrd0
        internal bool IsMatch(string text, int index, int beglimit, int endlimit)
        {
            //if (!this._rightToLeft)
            //{
            //    return (((index >= beglimit) && ((endlimit - index) >= this._pattern.Length)) &&
            //        (0 == string.Compare(this._pattern, 0, text, index, this._pattern.Length, 
            //        this._caseInsensitive, this._culture)));
            //}
            //return (((index <= endlimit) && ((index - beglimit) >= this._pattern.Length)) &&
            //    (0 == string.Compare(this._pattern, 0, text, index - this._pattern.Length,
            //    this._pattern.Length, this._caseInsensitive, this._culture)));
            if (!this._rightToLeft)
            {
                return (((index >= beglimit) && ((endlimit - index) >= this._pattern.Length)) &&
                    (0 == string.Compare(this._pattern, 0, text, index, this._pattern.Length)));
            }
            return (((index <= endlimit) && ((index - beglimit) >= this._pattern.Length)) &&
                (0 == string.Compare(this._pattern, 0, text, index - this._pattern.Length,
                this._pattern.Length)));
        }

        internal int Scan(string text, int index, int beglimit, int endlimit)
        {
            int num;
            int num4;
            int num5;
            int num6;
            int length;
            int num8;
            int[] numArray;
            if (!this._rightToLeft)
            {
                length = this._pattern.Length;
                num4 = this._pattern.Length - 1;
                num5 = 0;
                num = (index + length) - 1;
                num8 = 1;
            }
            else
            {
                length = -this._pattern.Length;
                num4 = 0;
                num5 = -length - 1;
                num = index + length;
                num8 = -1;
            }
            char ch = this._pattern[num4];
        Label_005F:
            if ((num >= endlimit) || (num < beglimit))
            {
                return -1;
            }
            char c = text[num];
            if (this._caseInsensitive)
            {
                c = char.ToLower(c, this._culture);
            }
            if (c != ch)
            {
                if ((c & 0xff80) == 0)
                {
                    num6 = this._negativeASCII[c];
                }
                else if ((this._negativeUnicode != null) && ((numArray = this._negativeUnicode[c >> 8]) != null))
                {
                    num6 = numArray[c & '\x00ff'];
                }
                else
                {
                    num6 = length;
                }
                num += num6;
            }
            else
            {
                int num2 = num;
                int num3 = num4;
                do
                {
                    if (num3 == num5)
                    {
                        if (!this._rightToLeft)
                        {
                            return num2;
                        }
                        return (num2 + 1);
                    }
                    num3 -= num8;
                    num2 -= num8;
                    c = text[num2];
                    if (this._caseInsensitive)
                    {
                        c = char.ToLower(c, this._culture);
                    }
                }
                while (c == this._pattern[num3]);
                num6 = this._positive[num3];
                if ((c & 0xff80) == 0)
                {
                    num2 = (num3 - num4) + this._negativeASCII[c];
                }
                else if ((this._negativeUnicode != null) && ((numArray = this._negativeUnicode[c >> 8]) != null))
                {
                    num2 = (num3 - num4) + numArray[c & '\x00ff'];
                }
                else
                {
                    num += num6;
                    goto Label_005F;
                }
                if (this._rightToLeft ? (num2 < num6) : (num2 > num6))
                {
                    num6 = num2;
                }
                num += num6;
            }
            goto Label_005F;
        }

        public override string ToString()
        {
            return this._pattern;
        }
    }
}

