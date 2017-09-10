namespace System.Text.RegularExpressions
{
    using System;

    public class Match : Group
    {
        internal bool _balancing;
        internal static Match _empty = new Match(null, 1, string.Empty, 0, 0, 0);
        internal GroupCollection _groupcoll;
        internal int[] _matchcount;
        internal int[][] _matches;
        internal Regex _regex;
        internal int _textbeg;
        internal int _textend;
        internal int _textpos;
        internal int _textstart;

        internal Match(Regex regex, int capcount, string text, int begpos, int len, int startpos) : base(text, new int[2], 0)
        {
            this._regex = regex;
            this._matchcount = new int[capcount];
            this._matches = new int[capcount][];
            this._matches[0] = base._caps;
            this._textbeg = begpos;
            this._textend = begpos + len;
            this._textstart = startpos;
            this._balancing = false;
            if (((this._textbeg < 0) || (this._textstart < this._textbeg)) || ((this._textend < this._textstart) || (base._text.Length < this._textend)))
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        internal virtual void AddMatch(int cap, int i, int l)
        {
            if (this._matches[cap] == null)
            {
                this._matches[cap] = new int[2];
            }
            int num = this._matchcount[cap];
            if (((num * 2) + 2) > this._matches[cap].Length)
            {
                int[] numArray = this._matches[cap];
                int[] numArray2 = new int[num * 8];
                for (int j = 0; j < (num * 2); j++)
                {
                    numArray2[j] = numArray[j];
                }
                this._matches[cap] = numArray2;
            }
            this._matches[cap][num * 2] = i;
            this._matches[cap][(num * 2) + 1] = l;
            this._matchcount[cap] = num + 1;
        }

        internal virtual void BalanceMatch(int cap)
        {
            this._balancing = true;
            int num = this._matchcount[cap];
            int index = (num * 2) - 2;
            if (this._matches[cap][index] < 0)
            {
                index = -3 - this._matches[cap][index];
            }
            index -= 2;
            if ((index >= 0) && (this._matches[cap][index] < 0))
            {
                index = -3 - this._matches[cap][index];
            }
            this.AddMatch(cap, -3 - index, -4 - index);
        }

        internal virtual string GroupToStringImpl(int groupnum)
        {
            int num = this._matchcount[groupnum];
            if (num == 0)
            {
                return string.Empty;
            }
            int[] numArray = this._matches[groupnum];
            return base._text.Substring(numArray[(num - 1) * 2], numArray[(num * 2) - 1]);
        }

        internal virtual bool IsMatched(int cap)
        {
            return (((cap < this._matchcount.Length) && (this._matchcount[cap] > 0)) && (this._matches[cap][(this._matchcount[cap] * 2) - 1] != -2));
        }

        internal string LastGroupToStringImpl()
        {
            return this.GroupToStringImpl(this._matchcount.Length - 1);
        }

        internal virtual int MatchIndex(int cap)
        {
            int num = this._matches[cap][(this._matchcount[cap] * 2) - 2];
            if (num >= 0)
            {
                return num;
            }
            return this._matches[cap][-3 - num];
        }

        internal virtual int MatchLength(int cap)
        {
            int num = this._matches[cap][(this._matchcount[cap] * 2) - 1];
            if (num >= 0)
            {
                return num;
            }
            return this._matches[cap][-3 - num];
        }

        public Match NextMatch()
        {
            if (this._regex == null)
            {
                return this;
            }
            return this._regex.Run(false, base._length, base._text, this._textbeg, this._textend - this._textbeg, this._textpos);
        }

        internal virtual void RemoveMatch(int cap)
        {
            this._matchcount[cap]--;
        }

        internal virtual void Reset(Regex regex, string text, int textbeg, int textend, int textstart)
        {
            this._regex = regex;
            base._text = text;
            this._textbeg = textbeg;
            this._textend = textend;
            this._textstart = textstart;
            for (int i = 0; i < this._matchcount.Length; i++)
            {
                this._matchcount[i] = 0;
            }
            this._balancing = false;
        }

        public virtual string Result(string replacement)
        {
            if (replacement == null)
            {
                throw new ArgumentNullException();
            }
            if (this._regex == null)
            {
                throw new NotSupportedException(RegExRes.GetString(5));
            }
            return RegexParser.ParseReplacement(replacement, this._regex.caps, this._regex.capsize, this._regex.capnames, this._regex.roptions).Replacement(this);
        }

        public static Match Synchronized(Match inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException();
            }
            int length = inner._matchcount.Length;
            for (int i = 0; i < length; i++)
            {
                Group group = inner.Groups[i];
                Group.Synchronized(group);
            }
            return inner;
        }

        internal virtual void Tidy(int textpos)
        {
            int[] numArray = this._matches[0];
            base._index = numArray[0];
            base._length = numArray[1];
            this._textpos = textpos;
            base._capcount = this._matchcount[0];
            if (this._balancing)
            {
                for (int i = 0; i < this._matchcount.Length; i++)
                {
                    int num2 = this._matchcount[i] * 2;
                    int[] numArray2 = this._matches[i];
                    int index = 0;
                    index = 0;
                    while (index < num2)
                    {
                        if (numArray2[index] < 0)
                        {
                            break;
                        }
                        index++;
                    }
                    int num4 = index;
                    while (index < num2)
                    {
                        if (numArray2[index] < 0)
                        {
                            num4--;
                        }
                        else
                        {
                            if (index != num4)
                            {
                                numArray2[num4] = numArray2[index];
                            }
                            num4++;
                        }
                        index++;
                    }
                    this._matchcount[i] = num4 / 2;
                }
                this._balancing = false;
            }
        }

        public static Match Empty
        {
            get
            {
                return _empty;
            }
        }

        public virtual GroupCollection Groups
        {
            get
            {
                if (this._groupcoll == null)
                {
                    this._groupcoll = new GroupCollection(this, null);
                }
                return this._groupcoll;
            }
        }
    }
}

