namespace System.Text.RegularExpressions
{
    using System;
    using System.Globalization;

    internal sealed class RegexFC
    {
        internal bool _caseInsensitive;
        internal RegexCharClass _cc;
        internal bool _nullable;

        internal RegexFC(bool nullable)
        {
            this._cc = new RegexCharClass();
            this._nullable = nullable;
        }

        internal RegexFC(string set, bool nullable, bool caseInsensitive)
        {
            this._cc = new RegexCharClass();
            this._cc.AddSet(set);
            this._nullable = nullable;
            this._caseInsensitive = caseInsensitive;
        }

        internal RegexFC(char ch, bool not, bool nullable, bool caseInsensitive)
        {
            this._cc = new RegexCharClass();
            if (not)
            {
                if (ch > '\0')
                {
                    this._cc.AddRange('\0', (char) (ch - '\x0001'));
                }
                if (ch < 0xffff)
                {
                    this._cc.AddRange((char) (ch + '\x0001'), (char)0xffff);
                }
            }
            else
            {
                this._cc.AddRange(ch, ch);
            }
            this._caseInsensitive = caseInsensitive;
            this._nullable = nullable;
        }

        internal void AddFC(RegexFC fc, bool concatenate)
        {
            if (concatenate)
            {
                if (!this._nullable)
                {
                    return;
                }
                if (!fc._nullable)
                {
                    this._nullable = false;
                }
            }
            else if (fc._nullable)
            {
                this._nullable = true;
            }
            this._caseInsensitive |= fc._caseInsensitive;
            this._cc.AddCharClass(fc._cc);
        }

        internal string GetFirstChars(CultureInfo culture)
        {
            return this._cc.ToSetCi(this._caseInsensitive, culture);
        }

        internal bool IsCaseInsensitive()
        {
            return this._caseInsensitive;
        }
    }
}

