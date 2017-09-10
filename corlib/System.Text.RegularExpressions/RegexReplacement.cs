namespace System.Text.RegularExpressions
{
    using System;
    using System.Collections;
    using System.Text;

    internal sealed class RegexReplacement
    {
        internal string _rep;
        internal int[] _rules;
        internal string[] _strings;
        internal const int LastGroup = -3;
        internal const int LeftPortion = -1;
        internal const int RightPortion = -2;
        internal const int Specials = 4;
        internal const int WholeString = -4;

        internal RegexReplacement(string rep, RegexNode concat, System.Collections.Generic.Dictionary<object,object> _caps)
        {
            this._rep = rep;
            if (concat.Type() != 0x19)
            {
                throw new ArgumentException(RegExRes.GetString(0x25));
            }
            StringBuilder builder = new StringBuilder();
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            for (int i = 0; i < concat.ChildCount(); i++)
            {
                RegexNode node = concat.Child(i);
                switch (node.Type())
                {
                    case 9:
                    {
                        builder.Append(node._ch);
                        continue;
                    }
                    case 12:
                    {
                        builder.Append(node._str);
                        continue;
                    }
                    case 13:
                    {
                        if (builder.Length > 0)
                        {
                            list2.Add(list.Count);
                            list.Add(builder.ToString());
                            builder.Length = 0;
                        }
                        int num = node._m;
                        if ((_caps != null) && (num >= 0))
                        {
                            num = (int) _caps[num];
                        }
                        list2.Add(-5 - num);
                        continue;
                    }
                }
                throw new ArgumentException(RegExRes.GetString(0x25));
            }
            if (builder.Length > 0)
            {
                list2.Add(list.Count);
                list.Add(builder.ToString());
            }
            this._strings = new string[list.Count];
            list.CopyTo(0, this._strings, 0, list.Count);
            this._rules = new int[list2.Count];
            for (int j = 0; j < list2.Count; j++)
            {
                this._rules[j] = (int) list2[j];
            }
        }

        internal string Replace(Regex regex, string input, int count, int startat)
        {
            StringBuilder builder;
            if (count < -1)
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((startat < 0) || (startat > input.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count == 0)
            {
                return input;
            }
            Match match = regex.Match(input, startat);
            if (!match.Success)
            {
                return input;
            }
            if (regex.RightToLeft)
            {
                ArrayList list = new ArrayList();
                int length = input.Length;
            Label_00BF:
                if ((match.Index + match.Length) != length)
                {
                    list.Add(input.Substring(match.Index + match.Length, (length - match.Index) - match.Length));
                }
                length = match.Index;
                for (int i = this._rules.Length - 1; i >= 0; i--)
                {
                    int index = this._rules[i];
                    if (index >= 0)
                    {
                        list.Add(this._strings[index]);
                    }
                    else
                    {
                        list.Add(match.GroupToStringImpl(-5 - index));
                    }
                }
                if (--count != 0)
                {
                    match = match.NextMatch();
                    if (match.Success)
                    {
                        goto Label_00BF;
                    }
                }
                builder = new StringBuilder();
                if (length > 0)
                {
                    builder.Append(input, 0, length);
                }
                for (int j = list.Count - 1; j >= 0; j--)
                {
                    builder.Append((string) list[j]);
                }
                goto Label_01AA;
            }
            builder = new StringBuilder();
            int startIndex = 0;
        Label_0048:
            if (match.Index != startIndex)
            {
                builder.Append(input, startIndex, match.Index - startIndex);
            }
            startIndex = match.Index + match.Length;
            this.ReplacementImpl(builder, match);
            if (--count != 0)
            {
                match = match.NextMatch();
                if (match.Success)
                {
                    goto Label_0048;
                }
            }
            if (startIndex < input.Length)
            {
                builder.Append(input, startIndex, input.Length - startIndex);
            }
        Label_01AA:
            return builder.ToString();
        }

        internal static string Replace(MatchEvaluator evaluator, Regex regex, string input, int count, int startat)
        {
            StringBuilder builder;
            if (evaluator == null)
            {
                throw new ArgumentNullException();
            }
            if (count < -1)
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((startat < 0) || (startat > input.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count == 0)
            {
                return input;
            }
            Match match = regex.Match(input, startat);
            if (!match.Success)
            {
                return input;
            }
            if (regex.RightToLeft)
            {
                ArrayList list = new ArrayList();
                int length = input.Length;
            Label_00CE:
                if ((match.Index + match.Length) != length)
                {
                    list.Add(input.Substring(match.Index + match.Length, (length - match.Index) - match.Length));
                }
                length = match.Index;
                list.Add(evaluator(match));
                if (--count != 0)
                {
                    match = match.NextMatch();
                    if (match.Success)
                    {
                        goto Label_00CE;
                    }
                }
                builder = new StringBuilder();
                if (length > 0)
                {
                    builder.Append(input, 0, length);
                }
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    builder.Append((string) list[i]);
                }
                goto Label_0177;
            }
            builder = new StringBuilder();
            int startIndex = 0;
        Label_0051:
            if (match.Index != startIndex)
            {
                builder.Append(input, startIndex, match.Index - startIndex);
            }
            startIndex = match.Index + match.Length;
            builder.Append(evaluator(match));
            if (--count != 0)
            {
                match = match.NextMatch();
                if (match.Success)
                {
                    goto Label_0051;
                }
            }
            if (startIndex < input.Length)
            {
                builder.Append(input, startIndex, input.Length - startIndex);
            }
        Label_0177:
            return builder.ToString();
        }

        internal string Replacement(Match match)
        {
            StringBuilder sb = new StringBuilder();
            this.ReplacementImpl(sb, match);
            return sb.ToString();
        }

        private void ReplacementImpl(StringBuilder sb, Match match)
        {
            for (int i = 0; i < this._rules.Length; i++)
            {
                int index = this._rules[i];
                if (index >= 0)
                {
                    sb.Append(this._strings[index]);
                }
                else if (index < -4)
                {
                    sb.Append(match.GroupToStringImpl(-5 - index));
                }
                else
                {
                    switch ((-5 - index))
                    {
                        case -4:
                            sb.Append(match.GetOriginalString());
                            break;

                        case -3:
                            sb.Append(match.LastGroupToStringImpl());
                            break;

                        case -2:
                            sb.Append(match.GetRightSubstring());
                            break;

                        case -1:
                            sb.Append(match.GetLeftSubstring());
                            break;
                    }
                }
            }
        }

        internal static string[] Split(Regex regex, string input, int count, int startat)
        {
            string[] strArray2;
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((startat < 0) || (startat > input.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count == 1)
            {
                return new string[] { input };
            }
            count--;
            Match match = regex.Match(input, startat);
            if (!match.Success)
            {
                return new string[] { input };
            }
            ArrayList list = new ArrayList();
            if (regex.RightToLeft)
            {
                int length = input.Length;
                do
                {
                    list.Add(input.Substring(match.Index + match.Length, (length - match.Index) - match.Length));
                    length = match.Index;
                    if (--count == 0)
                    {
                        break;
                    }
                    match = match.NextMatch();
                }
                while (match.Success);
                list.Add(input.Substring(0, length));
                list.Reverse(0, list.Count);
                goto Label_0153;
            }
            int startIndex = 0;
        Label_0064:
            list.Add(input.Substring(startIndex, match.Index - startIndex));
            startIndex = match.Index + match.Length;
            for (int i = 1; match.IsMatched(i); i++)
            {
                list.Add(match.Groups[i].ToString());
            }
            if (--count != 0)
            {
                match = match.NextMatch();
                if (match.Success)
                {
                    goto Label_0064;
                }
            }
            list.Add(input.Substring(startIndex, input.Length - startIndex));
        Label_0153:
            strArray2 = new string[list.Count];
            list.CopyTo(0, strArray2, 0, list.Count);
            return strArray2;
        }

        internal string Pattern
        {
            get
            {
                return this._rep;
            }
        }
    }
}

