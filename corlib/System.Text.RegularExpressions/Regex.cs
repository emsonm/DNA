namespace System.Text.RegularExpressions
{
    using System;
    using System.Collections;
    using System.Globalization;

    public class Regex
    {
        protected internal System.Collections.Generic.Dictionary<object,object> capnames;
        protected internal System.Collections.Generic.Dictionary<object,object> caps;
        protected internal int capsize;
        protected internal string[] capslist;
        internal RegexCode code;
        internal const int MaxOptionShift = 9;
        protected internal string pattern;
        internal bool refsInitialized;
        protected internal RegexOptions roptions;

        protected Regex()
        {
            this.refsInitialized = false;
        }

        public Regex(string pattern) : this(pattern, RegexOptions.None)
        {
        }

        public Regex(string pattern, RegexOptions options)
        {
            this.refsInitialized = false;
            if (pattern == null)
            {
                throw new ArgumentNullException();
            }
            if ((options < RegexOptions.None) || ((((int) options) >> 9) != 0))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (((options & RegexOptions.ECMAScript) != RegexOptions.None) && ((options & ~(RegexOptions.CultureInvariant | RegexOptions.ECMAScript | RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase)) != RegexOptions.None))
            {
                throw new ArgumentOutOfRangeException();
            }
            string text1 = options + ":" + pattern;
            this.pattern = pattern;
            this.roptions = options;
            RegexTree t = RegexParser.Parse(pattern, this.roptions);
            this.capnames = t._capnames;
            this.capslist = t._capslist;
            this.code = RegexWriter.Write(t);
            this.caps = this.code._caps;
            this.capsize = this.code._capsize;
        }

        public static string Escape(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException();
            }
            return RegexParser.Escape(str);
        }

        public string[] GetGroupNames()
        {
            string[] strArray;
            if (this.capslist == null)
            {
                int capsize = this.capsize;
                strArray = new string[capsize];
                for (int i = 0; i < capsize; i++)
                {
                    strArray[i] = i.ToString();// Convert.ToString(i);
                }
                return strArray;
            }
            strArray = new string[this.capslist.Length];
            Array.Copy(this.capslist, 0, strArray, 0, this.capslist.Length);
            return strArray;
        }

        public int[] GetGroupNumbers()
        {
            int[] numArray;
            if (this.caps == null)
            {
                int capsize = this.capsize;
                numArray = new int[capsize];
                for (int i = 0; i < capsize; i++)
                {
                    numArray[i] = i;
                }
                return numArray;
            }
            numArray = new int[this.caps.Count];
            IDictionaryEnumerator enumerator = this.caps.GetEnumerator();
            while (enumerator.MoveNext())
            {
                numArray[(int) enumerator.Value] = (int) enumerator.Key;
            }
            return numArray;
        }

        public string GroupNameFromNumber(int i)
        {
            if (this.capslist == null)
            {
                if ((i >= 0) && (i < this.capsize))
                {
                    return i.ToString();
                }
                return string.Empty;
            }
            if (this.caps != null)
            {
                object obj2 = this.caps[i];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                i = (int) obj2;
            }
            return this.capslist[i];
        }

        public int GroupNumberFromName(string name)
        {
            int num = -1;
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            if (this.capnames != null)
            {
                object obj2 = this.capnames[name];
                if (obj2 == null)
                {
                    return -1;
                }
                return (int) obj2;
            }
            num = 0;
            for (int i = 0; i < name.Length; i++)
            {
                char ch = name[i];
                if ((ch > '9') || (ch < '0'))
                {
                    return -1;
                }
                num *= 10;
                num += ch - '0';
            }
            if ((num >= 0) && (num < this.capsize))
            {
                return num;
            }
            return -1;
        }

        public bool IsMatch(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return (null == this.Run(true, -1, input, 0, input.Length, this.UseOptionR() ? input.Length : 0));
        }

        public bool IsMatch(string input, int startat)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return (null == this.Run(true, -1, input, 0, input.Length, startat));
        }

        public static bool IsMatch(string input, string pattern)
        {
            return new Regex(pattern).IsMatch(input);
        }

        public static bool IsMatch(string input, string pattern, RegexOptions options)
        {
            return new Regex(pattern, options).IsMatch(input);
        }

        public System.Text.RegularExpressions.Match Match(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return this.Run(false, -1, input, 0, input.Length, this.UseOptionR() ? input.Length : 0);
        }

        public System.Text.RegularExpressions.Match Match(string input, int startat)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return this.Run(false, -1, input, 0, input.Length, startat);
        }

        public static System.Text.RegularExpressions.Match Match(string input, string pattern)
        {
            return new Regex(pattern).Match(input);
        }

        public System.Text.RegularExpressions.Match Match(string input, int beginning, int length)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return this.Run(false, -1, input, beginning, length, this.UseOptionR() ? (beginning + length) : beginning);
        }

        public static System.Text.RegularExpressions.Match Match(string input, string pattern, RegexOptions options)
        {
            return new Regex(pattern, options).Match(input);
        }

        public MatchCollection Matches(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return new MatchCollection(this, input, 0, input.Length, this.UseOptionR() ? input.Length : 0);
        }

        public MatchCollection Matches(string input, int startat)
        {
            return new MatchCollection(this, input, 0, input.Length, startat);
        }

        public static MatchCollection Matches(string input, string pattern)
        {
            return new Regex(pattern).Matches(input);
        }

        public static MatchCollection Matches(string input, string pattern, RegexOptions options)
        {
            return new Regex(pattern, options).Matches(input);
        }

        public string Replace(string input, string replacement)
        {
            if ((input == null) || (replacement == null))
            {
                throw new ArgumentNullException();
            }
            return this.Replace(input, replacement, -1, this.UseOptionR() ? input.Length : 0);
        }

        public string Replace(string input, MatchEvaluator evaluator)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return this.Replace(input, evaluator, -1, this.UseOptionR() ? input.Length : 0);
        }

        public string Replace(string input, string replacement, int count)
        {
            if ((input == null) || (replacement == null))
            {
                throw new ArgumentNullException();
            }
            return this.Replace(input, replacement, count, this.UseOptionR() ? input.Length : 0);
        }

        public static string Replace(string input, string pattern, string replacement)
        {
            return new Regex(pattern).Replace(input, replacement);
        }

        public static string Replace(string input, string pattern, MatchEvaluator evaluator)
        {
            return new Regex(pattern).Replace(input, evaluator);
        }

        public string Replace(string input, MatchEvaluator evaluator, int count)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return this.Replace(input, evaluator, count, this.UseOptionR() ? input.Length : 0);
        }

        public string Replace(string input, string replacement, int count, int startat)
        {
            if ((input == null) || (replacement == null))
            {
                throw new ArgumentNullException();
            }
            return RegexParser.ParseReplacement(replacement, this.caps, this.capsize, this.capnames, this.roptions).Replace(this, input, count, startat);
        }

        public static string Replace(string input, string pattern, string replacement, RegexOptions options)
        {
            return new Regex(pattern, options).Replace(input, replacement);
        }

        public static string Replace(string input, string pattern, MatchEvaluator evaluator, RegexOptions options)
        {
            return new Regex(pattern, options).Replace(input, evaluator);
        }

        public string Replace(string input, MatchEvaluator evaluator, int count, int startat)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return RegexReplacement.Replace(evaluator, this, input, count, startat);
        }

        internal System.Text.RegularExpressions.Match Run(bool quick, int prevlen, string input, int beginning, int length, int startat)
        {
            RegexRunner runner = null;
            if ((startat < 0) || (startat > input.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((length < 0) || (length > input.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            runner = new RegexInterpreter(this.code, this.UseOptionInvariant() ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);
            return runner.Scan(this, input, beginning, beginning + length, startat, prevlen, quick);
        }

        public string[] Split(string input)
        {
            return this.Split(input, 0, this.UseOptionR() ? input.Length : 0);
        }

        public string[] Split(string input, int count)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return RegexReplacement.Split(this, input, count, this.UseOptionR() ? input.Length : 0);
        }

        public static string[] Split(string input, string pattern)
        {
            return new Regex(pattern).Split(input);
        }

        public string[] Split(string input, int count, int startat)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }
            return RegexReplacement.Split(this, input, count, startat);
        }

        public static string[] Split(string input, string pattern, RegexOptions options)
        {
            return new Regex(pattern, options).Split(input);
        }

        public override string ToString()
        {
            return this.pattern;
        }

        public static string Unescape(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException();
            }
            return RegexParser.Unescape(str);
        }

        protected bool UseOptionC()
        {
            return ((this.roptions & RegexOptions.Compiled) != RegexOptions.None);
        }

        internal bool UseOptionInvariant()
        {
            return ((this.roptions & RegexOptions.CultureInvariant) != RegexOptions.None);
        }

        protected bool UseOptionR()
        {
            return ((this.roptions & RegexOptions.RightToLeft) != RegexOptions.None);
        }

        public RegexOptions Options
        {
            get
            {
                return this.roptions;
            }
        }

        public bool RightToLeft
        {
            get
            {
                return this.UseOptionR();
            }
        }
    }
}

