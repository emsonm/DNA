// Copyright (c) 2012 DotNetAnywhere
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if !LOCALTEST

using System.Runtime.CompilerServices;
using System.Globalization;
using System.Threading;
namespace System {
	public struct UInt32 : IFormattable, IComparable, IComparable<uint>, IEquatable<uint> {
		public const uint MaxValue = 0xffffffff;
		public const uint MinValue = 0;

#pragma warning disable 0169, 0649
        internal uint m_value;
#pragma warning restore 0169, 0649

        public override bool Equals(object obj) {
			return (obj is uint) && ((uint)obj).m_value == this.m_value;
		}

		public override int GetHashCode() {
			return (int)this.m_value;
		}

        #region Parse methods

        internal static bool Parse(string s, bool tryParse, out uint result, out Exception exc)
        {
            uint val = 0;
            int len;
            int i;
            bool digits_seen = false;
            bool has_negative_sign = false;

            result = 0;
            exc = null;

            if (s == null)
            {
                if (!tryParse)
                    exc = new ArgumentNullException("s");
                return false;
            }

            len = s.Length;

            char c;
            for (i = 0; i < len; i++)
            {
                c = s[i];
                if (!Char.IsWhiteSpace(c))
                    break;
            }

            if (i == len)
            {
                if (!tryParse)
                    exc = ParseHelper.GetFormatException();
                return false;
            }

            if (s[i] == '+')
                i++;
            else
                if (s[i] == '-')
                {
                    i++;
                    has_negative_sign = true;
                }

            for (; i < len; i++)
            {
                c = s[i];

                if (c >= '0' && c <= '9')
                {
                    uint d = (uint)(c - '0');

                    if ((val > MaxValue / 10) || (val == (MaxValue / 10) && d > (MaxValue % 10)))
                    {
                        if (!tryParse)
                            exc = new OverflowException(String.Format("Value is too large"));
                        return false;
                    }
                    val = (val * 10) + d;
                    digits_seen = true;
                }
                else if (!ParseHelper.ProcessTrailingWhitespace(tryParse, s, i, ref exc))
                {
                    return false;
                }
            }
            if (!digits_seen)
            {
                if (!tryParse)
                    exc = ParseHelper.GetFormatException();
                return false;
            }

            // -0 is legal but other negative values are not
            if (has_negative_sign && (val > 0))
            {
                if (!tryParse)
                    exc = new OverflowException(
                        String.Format("Negative number"));
                return false;
            }

            result = val;
            return true;
        }

        internal static bool Parse(string s, NumberStyles style, IFormatProvider provider, bool tryParse, out uint result, out Exception exc)
        {
            result = 0;
            exc = null;

            if (s == null)
            {
                if (!tryParse)
                    exc = new ArgumentNullException("s");
                return false;
            }

            if (s.Length == 0)
            {
                if (!tryParse)
                    exc = ParseHelper.GetFormatException();
                return false;
            }

            NumberFormatInfo nfi = null;
            if (provider != null)
            {
                Type typeNFI = typeof(NumberFormatInfo);
                nfi = (NumberFormatInfo)provider.GetFormat(typeNFI);
            }
            if (nfi == null)
                nfi = Thread.CurrentThread.CurrentCulture.NumberFormat;

            if (!ParseHelper.CheckStyle(style, tryParse, ref exc))
                return false;

            bool AllowCurrencySymbol = (style & NumberStyles.AllowCurrencySymbol) != 0;
            bool AllowHexSpecifier = (style & NumberStyles.AllowHexSpecifier) != 0;
            bool AllowThousands = (style & NumberStyles.AllowThousands) != 0;
            bool AllowDecimalPoint = (style & NumberStyles.AllowDecimalPoint) != 0;
            bool AllowParentheses = (style & NumberStyles.AllowParentheses) != 0;
            bool AllowTrailingSign = (style & NumberStyles.AllowTrailingSign) != 0;
            bool AllowLeadingSign = (style & NumberStyles.AllowLeadingSign) != 0;
            bool AllowTrailingWhite = (style & NumberStyles.AllowTrailingWhite) != 0;
            bool AllowLeadingWhite = (style & NumberStyles.AllowLeadingWhite) != 0;

            int pos = 0;

            if (AllowLeadingWhite && !ParseHelper.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
                return false;

            bool foundOpenParentheses = false;
            bool negative = false;
            bool foundSign = false;
            bool foundCurrency = false;

            // Pre-number stuff
            if (AllowParentheses && s[pos] == '(')
            {
                foundOpenParentheses = true;
                foundSign = true;
                negative = true; // MS always make the number negative when there parentheses
                // even when NumberFormatInfo.NumberNegativePattern != 0!!!
                pos++;
                if (AllowLeadingWhite && !ParseHelper.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
                    return false;

                if (s.Substring(pos, nfi.NegativeSign.Length) == nfi.NegativeSign)
                {
                    if (!tryParse)
                        exc = ParseHelper.GetFormatException();
                    return false;
                }
                if (s.Substring(pos, nfi.PositiveSign.Length) == nfi.PositiveSign)
                {
                    if (!tryParse)
                        exc = ParseHelper.GetFormatException();
                    return false;
                }
            }

            if (AllowLeadingSign && !foundSign)
            {
                // Sign + Currency
                ParseHelper.FindSign(ref pos, s, nfi, ref foundSign, ref negative);
                if (foundSign)
                {
                    if (AllowLeadingWhite && !ParseHelper.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
                        return false;
                    if (AllowCurrencySymbol)
                    {
                        ParseHelper.FindCurrency(ref pos, s, nfi, ref foundCurrency);
                        if (foundCurrency && AllowLeadingWhite &&
                                !ParseHelper.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
                            return false;
                    }
                }
            }

            if (AllowCurrencySymbol && !foundCurrency)
            {
                // Currency + sign
                ParseHelper.FindCurrency(ref pos, s, nfi, ref foundCurrency);
                if (foundCurrency)
                {
                    if (AllowLeadingWhite && !ParseHelper.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
                        return false;
                    if (foundCurrency)
                    {
                        if (!foundSign && AllowLeadingSign)
                        {
                            ParseHelper.FindSign(ref pos, s, nfi, ref foundSign, ref negative);
                            if (foundSign && AllowLeadingWhite &&
                                    !ParseHelper.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
                                return false;
                        }
                    }
                }
            }

            uint number = 0;
            int nDigits = 0;
            bool decimalPointFound = false;
            uint digitValue;
            char hexDigit;

            // Number stuff
            // Just the same as Int32, but this one adds instead of substract
            do
            {

                if (!ParseHelper.ValidDigit(s[pos], AllowHexSpecifier))
                {
                    if (AllowThousands && ParseHelper.FindOther(ref pos, s, nfi.NumberGroupSeparator))
                        continue;
                    else
                        if (!decimalPointFound && AllowDecimalPoint &&
                            ParseHelper.FindOther(ref pos, s, nfi.NumberDecimalSeparator))
                        {
                            decimalPointFound = true;
                            continue;
                        }
                    break;
                }
                else if (AllowHexSpecifier)
                {
                    nDigits++;
                    hexDigit = s[pos++];
                    if (Char.IsDigit(hexDigit))
                        digitValue = (uint)(hexDigit - '0');
                    else if (Char.IsLower(hexDigit))
                        digitValue = (uint)(hexDigit - 'a' + 10);
                    else
                        digitValue = (uint)(hexDigit - 'A' + 10);

                    if (tryParse)
                    {
                        ulong l = number * 16 + digitValue;

                        if (l > MaxValue)
                            return false;
                        number = (uint)l;
                    }
                    else
                        number = checked(number * 16 + digitValue);
                }
                else if (decimalPointFound)
                {
                    nDigits++;
                    // Allows decimal point as long as it's only 
                    // followed by zeroes.
                    if (s[pos++] != '0')
                    {
                        if (!tryParse)
                            exc = new OverflowException(String.Format("Value too large or too small."));
                        return false;
                    }
                }
                else
                {
                    nDigits++;

                    try
                    {
                        number = checked(number * 10 + (uint)(s[pos++] - '0'));
                    }
                    catch (OverflowException)
                    {
                        if (!tryParse)
                            exc = new OverflowException(String.Format("Value too large or too small."));
                        return false;
                    }
                }
            } while (pos < s.Length);

            // Post number stuff
            if (nDigits == 0)
            {
                if (!tryParse)
                    exc = ParseHelper.GetFormatException();
                return false;
            }

            if (AllowTrailingSign && !foundSign)
            {
                // Sign + Currency
                ParseHelper.FindSign(ref pos, s, nfi, ref foundSign, ref negative);
                if (foundSign)
                {
                    if (AllowTrailingWhite && !ParseHelper.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
                        return false;
                    if (AllowCurrencySymbol)
                        ParseHelper.FindCurrency(ref pos, s, nfi, ref foundCurrency);
                }
            }

            if (AllowCurrencySymbol && !foundCurrency)
            {
                // Currency + sign
                ParseHelper.FindCurrency(ref pos, s, nfi, ref foundCurrency);
                if (foundCurrency)
                {
                    if (AllowTrailingWhite && !ParseHelper.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
                        return false;
                    if (!foundSign && AllowTrailingSign)
                        ParseHelper.FindSign(ref pos, s, nfi, ref foundSign, ref negative);
                }
            }

            if (AllowTrailingWhite && pos < s.Length && !ParseHelper.JumpOverWhite(ref pos, s, false, tryParse, ref exc))
                return false;

            if (foundOpenParentheses)
            {
                if (pos >= s.Length || s[pos++] != ')')
                {
                    if (!tryParse)
                        exc = ParseHelper.GetFormatException();
                    return false;
                }
                if (AllowTrailingWhite && pos < s.Length && !ParseHelper.JumpOverWhite(ref pos, s, false, tryParse, ref exc))
                    return false;
            }

            if (pos < s.Length && s[pos] != '\u0000')
            {
                if (!tryParse)
                    exc = ParseHelper.GetFormatException();
                return false;
            }

            // -0 is legal but other negative values are not
            if (negative && (number > 0))
            {
                if (!tryParse)
                    exc = new OverflowException(
                        String.Format("Negative number"));
                return false;
            }

            result = number;

            return true;
        }

        [CLSCompliant(false)]
        public static uint Parse(string s)
        {
            Exception exc;
            uint res;

            if (!Parse(s, false, out res, out exc))
                throw exc;

            return res;
        }

        [CLSCompliant(false)]
        public static uint Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            Exception exc;
            uint res;

            if (!Parse(s, style, provider, false, out res, out exc))
                throw exc;

            return res;
        }

        [CLSCompliant(false)]
        public static uint Parse(string s, IFormatProvider provider)
        {
            return Parse(s, NumberStyles.Integer, provider);
        }

        [CLSCompliant(false)]
        public static uint Parse(string s, NumberStyles style)
        {
            return Parse(s, style, null);
        }

        public static bool TryParse(string s, out uint result)
        {
            Exception exc;
            if (!Parse(s, true, out result, out exc))
            {
                result = 0;
                return false;
            }

            return true;
        }

        [CLSCompliant(false)]
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out uint result)
        {
            Exception exc;
            if (!Parse(s, style, provider, true, out result, out exc))
            {
                result = 0;
                return false;
            }

            return true;
        }

        #endregion

		#region ToString methods

		public override string ToString() {
			return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(this.m_value));
		}

		public string ToString(IFormatProvider formatProvider) {
			return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(this.m_value), formatProvider);
		}

		public string ToString(string format) {
			return ToString(format, null);
		}

		public string ToString(string format, IFormatProvider formatProvider) {
			NumberFormatInfo nfi = NumberFormatInfo.GetInstance(formatProvider);
			return NumberFormatter.NumberToString(format, m_value, nfi);
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj) {
			if (obj == null) {
				return 1;
			}
			if (!(obj is uint)) {
				throw new ArgumentException();
			}
			return this.CompareTo((uint)obj);
		}

		#endregion

		#region IComparable<uint> Members

		public int CompareTo(uint x) {
			return (this.m_value > x) ? 1 : ((this.m_value < x) ? -1 : 0);
		}

		#endregion

		#region IEquatable<uint> Members

		public bool Equals(uint x) {
			return this.m_value == x;
		}

		#endregion

	}
}

#endif
