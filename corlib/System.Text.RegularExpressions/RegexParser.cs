namespace System.Text.RegularExpressions
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using System.Collections.Generic;

    internal sealed class RegexParser
    {
        internal RegexNode _alternation;
        internal int _autocap;
        internal int _capcount;
        internal ArrayList _capnamelist;
        internal System.Collections.Generic.Dictionary<object,object> _capnames;
        internal object[] _capnumlist;
        internal System.Collections.Generic.Dictionary<object,object> _caps;
        internal int _capsize;
        internal int _captop;
        internal static readonly byte[] _category = new byte[] { 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 0, 2, 2, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            2, 0, 0, 3, 4, 0, 0, 0, 4, 4, 5, 5, 0, 0, 4, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 0, 4, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 4, 0, 0, 0
         };
        internal RegexNode _concatenation;
        internal CultureInfo _culture;
        internal int _currentPos;
        internal RegexNode _group;
        internal bool _ignoreNextParen = false;
        internal RegexOptions _options;
        internal ArrayList _optionsStack;
        internal string _pattern;
        internal RegexNode _stack;
        internal RegexNode _unit;
        internal const byte E = 1;
        internal const int infinite = 0x7fffffff;
        internal const byte Q = 5;
        internal const byte S = 4;
        internal const byte X = 2;
        internal const byte Z = 3;

        private RegexParser(CultureInfo culture)
        {
            this._culture = culture;
            this._optionsStack = new ArrayList();
            this._caps = new System.Collections.Generic.Dictionary<object,object>();
        }

        internal void AddAlternate()
        {
            if ((this._group.Type() == 0x22) || (this._group.Type() == 0x21))
            {
                this._group.AddChild(this._concatenation.ReverseLeft());
            }
            else
            {
                this._alternation.AddChild(this._concatenation.ReverseLeft());
            }
            this._concatenation = new RegexNode(0x19, this._options);
        }

        internal void AddConcatenate()
        {
            this._concatenation.AddChild(this._unit);
            this._unit = null;
        }

        internal void AddConcatenate(int pos, int cch)
        {
            if (cch != 0)
            {
                RegexNode node;
                if (cch > 1)
                {
                    string str = this._pattern.Substring(pos, cch);
                    if (this.UseOptionI())
                    {
                        str = str.ToLower(this._culture);
                    }
                    node = new RegexNode(12, this._options, str);
                }
                else
                {
                    char c = this._pattern[pos];
                    if (this.UseOptionI())
                    {
                        c = char.ToLower(c, this._culture);
                    }
                    node = new RegexNode(9, this._options, c);
                }
                this._concatenation.AddChild(node);
            }
        }

        internal void AddConcatenate(bool lazy, int min, int max)
        {
            this._concatenation.AddChild(this._unit.MakeQuantifier(lazy, min, max));
            this._unit = null;
        }

        internal void AddGroup()
        {
            if ((this._group.Type() == 0x22) || (this._group.Type() == 0x21))
            {
                this._group.AddChild(this._concatenation.ReverseLeft());
                if (((this._group.Type() == 0x21) && (this._group.ChildCount() > 2)) || (this._group.ChildCount() > 3))
                {
                    throw this.MakeException(RegExRes.GetString(0x1c));
                }
            }
            else
            {
                this._alternation.AddChild(this._concatenation.ReverseLeft());
                this._group.AddChild(this._alternation);
            }
            this._unit = this._group;
        }

        internal void AddUnitNode(RegexNode node)
        {
            this._unit = node;
        }

        internal void AddUnitNotone(char ch)
        {
            if (this.UseOptionI())
            {
                ch = char.ToLower(ch, this._culture);
            }
            this._unit = new RegexNode(10, this._options, ch);
        }

        internal void AddUnitOne(char ch)
        {
            if (this.UseOptionI())
            {
                ch = char.ToLower(ch, this._culture);
            }
            this._unit = new RegexNode(9, this._options, ch);
        }

        internal void AddUnitSet(RegexCharClass cc)
        {
            this._unit = new RegexNode(11, this._options, cc.ToSetCi(this.UseOptionI(), this._culture), cc.Category);
        }

        internal void AddUnitType(int type)
        {
            this._unit = new RegexNode(type, this._options);
        }

        internal void AssignNameSlots()
        {
            if (this._capnames != null)
            {
                for (int i = 0; i < this._capnamelist.Count; i++)
                {
                    while (this.IsCaptureSlot(this._autocap))
                    {
                        this._autocap++;
                    }
                    string str = (string) this._capnamelist[i];
                    int pos = (int) this._capnames[str];
                    this._capnames[str] = this._autocap;
                    this.NoteCaptureSlot(this._autocap, pos);
                    this._autocap++;
                }
            }
            if (this._capcount < this._captop)
            {
                this._capnumlist = new object[this._capcount];
                int num3 = 0;
                IDictionaryEnumerator enumerator = this._caps.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this._capnumlist[num3++] = enumerator.Key;
                }
                //
                Array.Sort(this._capnumlist, 0, this._capnumlist.Length, null);
            }
            if ((this._capnames != null) || (this._capnumlist != null))
            {
                ArrayList list;
                int num4;
                int num5 = 0;
                if (this._capnames == null)
                {
                    list = null;
                    this._capnames = new System.Collections.Generic.Dictionary<object,object>();
                    this._capnamelist = new ArrayList();
                    num4 = -1;
                }
                else
                {
                    list = this._capnamelist;
                    this._capnamelist = new ArrayList();
                    num4 = (int) this._capnames[list[0]];
                }
                for (int j = 0; j < this._capcount; j++)
                {
                    int num7 = (this._capnumlist == null) ? j : ((int) this._capnumlist[j]);
                    if (num4 == num7)
                    {
                        this._capnamelist.Add((string) list[num5++]);
                        num4 = (num5 == list.Count) ? -1 : ((int) this._capnames[list[num5]]);
                    }
                    else
                    {
                        string str2 = num7.ToString();// Convert.ToString(num7);
                        this._capnamelist.Add(str2);
                        this._capnames[str2] = num7;
                    }
                }
            }
        }

        internal int CaptureSlotFromName(string capname)
        {
            return (int) this._capnames[capname];
        }

        internal char CharAt(int i)
        {
            return this._pattern[i];
        }

        internal int CharsRight()
        {
            return (this._pattern.Length - this._currentPos);
        }

        internal void CountCaptures()
        {
            this.NoteCaptureSlot(0, 0);
            this._autocap = 1;
            while (this.CharsRight() > 0)
            {
                int pos = this.Textpos();
                switch (this.RightCharNext())
                {
                    case '(':
                        if (((this.CharsRight() < 2) || (this.RightChar(1) != '#')) || (this.RightChar() != '?'))
                        {
                            break;
                        }
                        this.LeftNext();
                        this.ScanBlank();
                        goto Label_01C6;

                    case ')':
                    {
                        if (!this.EmptyOptionsStack())
                        {
                            this.PopOptions();
                        }
                        continue;
                    }
                    case '#':
                    {
                        if (this.UseOptionX())
                        {
                            this.LeftNext();
                            this.ScanBlank();
                        }
                        continue;
                    }
                    case '[':
                    {
                        this.ScanCharClass(false, true);
                        continue;
                    }
                    case '\\':
                    {
                        if (this.CharsRight() > 0)
                        {
                            this.RightNext();
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                this.PushOptions();
                if ((this.CharsRight() > 0) && (this.RightChar() == '?'))
                {
                    this.RightNext();
                    if ((this.CharsRight() > 1) && ((this.RightChar() == '<') || (this.RightChar() == '\'')))
                    {
                        this.RightNext();
                        char ch = this.RightChar();
                        if ((ch != '0') && RegexCharClass.IsWordChar(ch))
                        {
                            if ((ch >= '1') && (ch <= '9'))
                            {
                                this.NoteCaptureSlot(this.ScanDecimal(), pos);
                            }
                            else
                            {
                                this.NoteCaptureName(this.ScanCapname(), pos);
                            }
                        }
                        goto Label_01C6;
                    }
                    this.ScanOptions();
                    if (this.CharsRight() <= 0)
                    {
                        goto Label_01C6;
                    }
                    if (this.RightChar() == ')')
                    {
                        this.RightNext();
                        this.PopKeepOptions();
                        goto Label_01C6;
                    }
                    if (this.RightChar() != '(')
                    {
                        goto Label_01C6;
                    }
                    this._ignoreNextParen = true;
                    continue;
                }
                if (!this.UseOptionN() && !this._ignoreNextParen)
                {
                    this.NoteCaptureSlot(this._autocap++, pos);
                }
            Label_01C6:
                this._ignoreNextParen = false;
            }
            this.AssignNameSlots();
        }

        internal bool EmptyOptionsStack()
        {
            return (this._optionsStack.Count == 0);
        }

        internal bool EmptyStack()
        {
            return (this._stack == null);
        }

        internal static string Escape(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!IsMetachar(input[i]))
                {
                    continue;
                }
                StringBuilder builder = new StringBuilder();
                char ch = input[i];
                builder.Append(input, 0, i);
                do
                {
                    builder.Append('\\');
                    switch (ch)
                    {
                        case '\t':
                            ch = 't';
                            break;

                        case '\n':
                            ch = 'n';
                            break;

                        case '\f':
                            ch = 'f';
                            break;

                        case '\r':
                            ch = 'r';
                            break;
                    }
                    builder.Append(ch);
                    i++;
                    int startIndex = i;
                    while (i < input.Length)
                    {
                        ch = input[i];
                        if (IsMetachar(ch))
                        {
                            break;
                        }
                        i++;
                    }
                    builder.Append(input, startIndex, i - startIndex);
                }
                while (i < input.Length);
                return builder.ToString();
            }
            return input;
        }

        internal static int HexDigit(char ch)
        {
            int num;
            if (((num = ch - '0') <= 9) && (num >= 0))
            {
                return num;
            }
            if (((num = ch - 'a') <= 5) && (num >= 0))
            {
                return (num + 10);
            }
            if (((num = ch - 'A') <= 5) && (num >= 0))
            {
                return (num + 10);
            }
            return -1;
        }

        internal bool IsCaptureName(string capname)
        {
            if (this._capnames == null)
            {
                return false;
            }
            return this._capnames.ContainsKey(capname);
        }

        internal bool IsCaptureSlot(int i)
        {
            if (this._caps != null)
            {
                return this._caps.ContainsKey(i);
            }
            return ((i >= 0) && (i < this._capsize));
        }

        internal static bool IsMetachar(char ch)
        {
            return ((ch <= '|') && (_category[ch] >= 1));
        }

        internal bool IsOnlyTopOption(RegexOptions option)
        {
            if (((option != RegexOptions.RightToLeft) && (option != RegexOptions.Compiled)) && (option != RegexOptions.CultureInvariant))
            {
                return (option == RegexOptions.ECMAScript);
            }
            return true;
        }

        internal static bool IsQuantifier(char ch)
        {
            return ((ch <= '{') && (_category[ch] >= 5));
        }

        internal static bool IsSpace(char ch)
        {
            return ((ch <= ' ') && (_category[ch] == 2));
        }

        internal static bool IsSpecial(char ch)
        {
            return ((ch <= '|') && (_category[ch] >= 4));
        }

        internal static bool IsStopperX(char ch)
        {
            return ((ch <= '|') && (_category[ch] >= 2));
        }

        internal bool IsTrueQuantifier()
        {
            int num = this.CharsRight();
            if (num == 0)
            {
                return false;
            }
            int i = this.Textpos();
            char index = this.CharAt(i);
            if (index == '{')
            {
                int num3 = i;
                while (((--num > 0) && ((index = this.CharAt(++num3)) >= '0')) && (index <= '9'))
                {
                }
                if ((num == 0) || ((num3 - i) == 1))
                {
                    return false;
                }
                if (index == '}')
                {
                    return true;
                }
                if (index != ',')
                {
                    return false;
                }
                while (((--num > 0) && ((index = this.CharAt(++num3)) >= '0')) && (index <= '9'))
                {
                }
                return ((num > 0) && (index == '}'));
            }
            return ((index <= '{') && (_category[index] >= 5));
        }

        internal void LeftNext()
        {
            this._currentPos--;
        }

        internal ArgumentException MakeException()
        {
            return new ArgumentException();
        }

        internal ArgumentException MakeException(string message)
        {
            return new ArgumentException(RegExRes.GetString(0x1d, this._pattern, message), this._pattern);
        }

        internal void NoteCaptureName(string name, int pos)
        {
            if (this._capnames == null)
            {
                this._capnames = new System.Collections.Generic.Dictionary<object,object>();
                this._capnamelist = new ArrayList();
            }
            if (!this._capnames.ContainsKey(name))
            {
                this._capnames.Add(name, pos);
                this._capnamelist.Add(name);
            }
        }

        internal void NoteCaptures(System.Collections.Generic.Dictionary<object,object> caps, 
            int capsize, System.Collections.Generic.Dictionary<object,object> capnames)
        {
            this._caps = caps;
            this._capsize = capsize;
            this._capnames = capnames;
        }

        internal void NoteCaptureSlot(int i, int pos)
        {
            if (!this._caps.ContainsKey(i))
            {
                this._caps.Add(i, pos);
                this._capcount++;
                if (this._captop <= i)
                {
                    this._captop = i + 1;
                }
            }
        }

        internal static RegexOptions OptionFromCode(char ch)
        {
            if ((ch >= 'A') && (ch <= 'Z'))
            {
                ch = (char) (ch + ' ');
            }
            switch (ch)
            {
                case 'c':
                    return RegexOptions.Compiled;

                case 'e':
                    return RegexOptions.ECMAScript;

                case 'i':
                    return RegexOptions.IgnoreCase;

                case 'm':
                    return RegexOptions.Multiline;

                case 'n':
                    return RegexOptions.ExplicitCapture;

                case 'r':
                    return RegexOptions.RightToLeft;

                case 's':
                    return RegexOptions.Singleline;

                case 'x':
                    return RegexOptions.IgnorePatternWhitespace;
            }
            return RegexOptions.None;
        }

        internal static RegexTree Parse(string re, RegexOptions op)
        {
            string[] strArray;
            RegexParser parser = new RegexParser(((op & RegexOptions.CultureInvariant) != RegexOptions.None) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture) {
                _options = op
            };
            parser.SetPattern(re);
            parser.CountCaptures();
            parser.Reset(op);
            RegexNode root = parser.ScanRegex();
            if (parser._capnamelist == null)
            {
                strArray = null;
            }
            else
            {
                strArray = new string[parser._capnamelist.Count];
                parser._capnamelist.CopyTo(0, strArray, 0, parser._capnamelist.Count);
            }
            return new RegexTree(root, parser._caps, parser._capnumlist, parser._captop, parser._capnames, strArray, op);
        }

        internal string ParseProperty()
        {
            if (this.CharsRight() < 3)
            {
                throw this.MakeException(RegExRes.GetString(30));
            }
            if (this.RightCharNext() != '{')
            {
                throw this.MakeException(RegExRes.GetString(0x1f));
            }
            string str = this.ScanCapname();
            if ((this.CharsRight() == 0) || (this.RightCharNext() != '}'))
            {
                throw this.MakeException(RegExRes.GetString(30));
            }
            return str;
        }

        internal static RegexReplacement ParseReplacement(string rep, System.Collections.Generic.Dictionary<object,object> caps,
            int capsize, System.Collections.Generic.Dictionary<object,object> capnames, RegexOptions op)
        {
            RegexParser parser = new RegexParser(((op & RegexOptions.CultureInvariant) != RegexOptions.None) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);
            parser.NoteCaptures(caps, capsize, capnames);
            parser.SetPattern(rep);
            return new RegexReplacement(rep, parser.ScanReplacement(), caps);
        }

        internal void PopGroup()
        {
            this._concatenation = this._stack;
            this._alternation = this._concatenation._next;
            this._group = this._alternation._next;
            this._stack = this._group._next;
            if ((this._group.Type() == 0x22) && (this._group.ChildCount() == 0))
            {
                if (this._unit == null)
                {
                    throw this.MakeException(RegExRes.GetString(0x1b));
                }
                this._group.AddChild(this._unit);
                this._unit = null;
            }
        }

        internal void PopKeepOptions()
        {
            this._optionsStack.RemoveAt(this._optionsStack.Count - 1);
        }

        internal void PopOptions()
        {
            this._options = (RegexOptions) this._optionsStack[this._optionsStack.Count - 1];
            this._optionsStack.RemoveAt(this._optionsStack.Count - 1);
        }

        internal void PushGroup()
        {
            this._group._next = this._stack;
            this._alternation._next = this._group;
            this._concatenation._next = this._alternation;
            this._stack = this._concatenation;
        }

        internal void PushOptions()
        {
            this._optionsStack.Add(this._options);
        }

        internal void Reset(RegexOptions topopts)
        {
            this._currentPos = 0;
            this._autocap = 1;
            this._ignoreNextParen = false;
            if (this._optionsStack.Count > 0)
            {
                object obj2 = this._optionsStack[this._optionsStack.Count - 1];
                this._optionsStack.Clear();
                this._optionsStack.Add(obj2);
            }
            this._options = topopts;
            this._stack = null;
        }

        internal char RightChar()
        {
            return this._pattern[this._currentPos];
        }

        internal char RightChar(int i)
        {
            return this._pattern[this._currentPos + i];
        }

        internal char RightCharNext()
        {
            return this._pattern[this._currentPos++];
        }

        internal char RightNext()
        {
            return this._pattern[this._currentPos++];
        }

        internal RegexNode ScanBackslash()
        {
            char ch;
            RegexCharClass class2;
            if (this.CharsRight() == 0)
            {
                throw this.MakeException(RegExRes.GetString(0x13));
            }
            switch ((ch = this.RightChar()))
            {
                case 'S':
                    this.RightNext();
                    if (this.UseOptionE())
                    {
                        return new RegexNode(11, this._options, "\0\t\x000e !", string.Empty);
                    }
                    return new RegexNode(11, this._options, string.Empty, RegexCharClass.NotSpace);

                case 'W':
                    this.RightNext();
                    if (this.UseOptionE())
                    {
                        return new RegexNode(11, this._options, "\00:A[_`a{İı", string.Empty);
                    }
                    return new RegexNode(11, this._options, string.Empty, RegexCharClass.NotWord);

                case 'Z':
                case 'A':
                case 'B':
                case 'G':
                case 'b':
                case 'z':
                    this.RightNext();
                    return new RegexNode(this.TypeFromCode(ch), this._options);

                case 'D':
                    this.RightNext();
                    if (!this.UseOptionE())
                    {
                        class2 = RegexCharClass.CreateFromCategory("Nd", true, false, this._pattern);
                        return new RegexNode(11, this._options, string.Empty, class2.Category);
                    }
                    return new RegexNode(11, this._options, "\00:", string.Empty);

                case 'P':
                case 'p':
                    this.RightNext();
                    class2 = RegexCharClass.CreateFromCategory(this.ParseProperty(), ch != 'p', this.UseOptionI(), this._pattern);
                    return new RegexNode(11, this._options, class2.ToSetCi(this.UseOptionI(), this._culture), class2.Category);

                case 'd':
                    this.RightNext();
                    if (!this.UseOptionE())
                    {
                        class2 = RegexCharClass.CreateFromCategory("Nd", false, false, this._pattern);
                        return new RegexNode(11, this._options, string.Empty, class2.Category);
                    }
                    return new RegexNode(11, this._options, "0:", string.Empty);

                case 's':
                    this.RightNext();
                    if (this.UseOptionE())
                    {
                        return new RegexNode(11, this._options, "\t\x000e !", string.Empty);
                    }
                    return new RegexNode(11, this._options, string.Empty, RegexCharClass.Space);

                case 'w':
                    this.RightNext();
                    if (this.UseOptionE())
                    {
                        return new RegexNode(11, this._options, "0:A[_`a{İı", string.Empty);
                    }
                    return new RegexNode(11, this._options, string.Empty, RegexCharClass.Word);
            }
            return this.ScanBasicBackslash();
        }

        internal RegexNode ScanBasicBackslash()
        {
            if (this.CharsRight() == 0)
            {
                throw this.MakeException(RegExRes.GetString(0x13));
            }
            bool flag = false;
            char ch2 = '\0';
            int pos = this.Textpos();
            char ch = this.RightChar();
            if (ch == 'k')
            {
                if (this.CharsRight() >= 2)
                {
                    this.RightNext();
                    ch = this.RightCharNext();
                    switch (ch)
                    {
                        case '<':
                        case '\'':
                            flag = true;
                            ch2 = (ch == '\'') ? '\'' : '>';
                            break;
                    }
                }
                if (!flag)
                {
                    throw this.MakeException(RegExRes.GetString(20));
                }
                ch = this.RightChar();
            }
            else if (((ch == '<') || (ch == '\'')) && (this.CharsRight() > 1))
            {
                flag = true;
                ch2 = (ch == '\'') ? '\'' : '>';
                this.RightNext();
                ch = this.RightChar();
            }
            if ((flag && (ch >= '0')) && (ch <= '9'))
            {
                int i = this.ScanDecimal();
                if ((this.CharsRight() > 0) && (this.RightCharNext() == ch2))
                {
                    if (!this.IsCaptureSlot(i))
                    {
                        throw this.MakeException(RegExRes.GetString(0x15, i.ToString()));
                    }
                    return new RegexNode(13, this._options, i);
                }
            }
            else if ((flag || (ch < '1')) || (ch > '9'))
            {
                if (flag && RegexCharClass.IsWordChar(ch))
                {
                    string capname = this.ScanCapname();
                    if ((this.CharsRight() > 0) && (this.RightCharNext() == ch2))
                    {
                        if (!this.IsCaptureName(capname))
                        {
                            throw this.MakeException(RegExRes.GetString(0x16, capname));
                        }
                        return new RegexNode(13, this._options, this.CaptureSlotFromName(capname));
                    }
                }
            }
            else if (!this.UseOptionE())
            {
                int num6 = this.ScanDecimal();
                if (this.IsCaptureSlot(num6))
                {
                    return new RegexNode(13, this._options, num6);
                }
                if (num6 <= 9)
                {
                    throw this.MakeException(RegExRes.GetString(0x15, num6.ToString()));
                }
            }
            else
            {
                int m = -1;
                int num4 = ch - '0';
                int num5 = this.Textpos() - 1;
                while (num4 <= this._captop)
                {
                    if (this.IsCaptureSlot(num4) && ((this._caps == null) || (((int) this._caps[num4]) < num5)))
                    {
                        m = num4;
                    }
                    this.RightNext();
                    if (((this.CharsRight() == 0) || ((ch = this.RightChar()) < '0')) || (ch > '9'))
                    {
                        break;
                    }
                    num4 = (num4 * 10) + (ch - '0');
                }
                if (m >= 0)
                {
                    return new RegexNode(13, this._options, m);
                }
            }
            this.Textto(pos);
            ch = this.ScanCharEscape();
            if (this.UseOptionI())
            {
                ch = char.ToLower(ch, this._culture);
            }
            return new RegexNode(9, this._options, ch);
        }

        internal void ScanBlank()
        {
            if (this.UseOptionX())
            {
                while (true)
                {
                    while ((this.CharsRight() > 0) && IsSpace(this.RightChar()))
                    {
                        this.RightNext();
                    }
                    if (this.CharsRight() == 0)
                    {
                        return;
                    }
                    if (this.RightChar() == '#')
                    {
                        while ((this.CharsRight() > 0) && (this.RightChar() != '\n'))
                        {
                            this.RightNext();
                        }
                    }
                    else
                    {
                        if (((this.CharsRight() < 3) || (this.RightChar(2) != '#')) || ((this.RightChar(1) != '?') || (this.RightChar() != '(')))
                        {
                            return;
                        }
                        while ((this.CharsRight() > 0) && (this.RightChar() != ')'))
                        {
                            this.RightNext();
                        }
                        if (this.CharsRight() == 0)
                        {
                            throw this.MakeException(RegExRes.GetString(0x12));
                        }
                        this.RightNext();
                    }
                }
            }
            while (((this.CharsRight() >= 3) && (this.RightChar(2) == '#')) && ((this.RightChar(1) == '?') && (this.RightChar() == '(')))
            {
                while ((this.CharsRight() > 0) && (this.RightChar() != ')'))
                {
                    this.RightNext();
                }
                if (this.CharsRight() == 0)
                {
                    throw this.MakeException(RegExRes.GetString(0x12));
                }
                this.RightNext();
            }
        }

        internal string ScanCapname()
        {
            int startIndex = this.Textpos();
            while (this.CharsRight() > 0)
            {
                if (!RegexCharClass.IsWordChar(this.RightCharNext()))
                {
                    this.LeftNext();
                    break;
                }
            }
            return this._pattern.Substring(startIndex, this.Textpos() - startIndex);
        }

        internal RegexCharClass ScanCharClass(bool caseInsensitive)
        {
            return this.ScanCharClass(caseInsensitive, false);
        }

        internal RegexCharClass ScanCharClass(bool caseInsensitive, bool scanOnly)
        {
            bool flag2;
            char last = '\0';
            char first = '\0';
            RegexCharClass class2 = scanOnly ? null : new RegexCharClass();
            if ((this.CharsRight() > 0) && (this.RightChar() == '^'))
            {
                this.RightNext();
                flag2 = false;
                if (!scanOnly)
                {
                    class2.Negate = true;
                }
            }
            bool flag = false;
            for (flag2 = true; this.CharsRight() > 0; flag2 = false)
            {
                char ch3 = last = this.RightCharNext();
                switch (ch3)
                {
                    case '[':
                        if (((this.CharsRight() > 0) && (this.RightChar() == ':')) && !flag)
                        {
                            int pos = this.Textpos();
                            this.RightNext();
                            this.ScanCapname();
                            if (((this.CharsRight() < 2) || (this.RightCharNext() != ':')) || (this.RightCharNext() != ']'))
                            {
                                this.Textto(pos);
                            }
                        }
                        goto Label_0360;

                    case '\\':
                        if (this.CharsRight() <= 0)
                        {
                            goto Label_0360;
                        }
                        ch3 = last = this.RightCharNext();
                        if (ch3 > 'W')
                        {
                            goto Label_00C7;
                        }
                        if (ch3 > 'P')
                        {
                            break;
                        }
                        switch (ch3)
                        {
                            case 'D':
                                goto Label_0148;

                            case 'P':
                                goto Label_02B8;
                        }
                        goto Label_0300;

                    case ']':
                        if (flag2)
                        {
                            goto Label_0360;
                        }
                        this.LeftNext();
                        return class2;

                    default:
                        goto Label_0360;
                }
                switch (ch3)
                {
                    case 'S':
                    {
                        if (!scanOnly)
                        {
                            if (flag)
                            {
                                throw this.MakeException(RegExRes.GetString(13, last.ToString()));
                            }
                            if (this.UseOptionE())
                            {
                                class2.AddSet("\0\t\x000e !");
                            }
                            else
                            {
                                class2.AddCategory(RegexCharClass.NotSpace);
                            }
                        }
                        continue;
                    }
                    case 'W':
                    {
                        if (!scanOnly)
                        {
                            if (flag)
                            {
                                throw this.MakeException(RegExRes.GetString(13, last.ToString()));
                            }
                            if (this.UseOptionE())
                            {
                                class2.AddSet("\00:A[_`a{İı");
                            }
                            else
                            {
                                class2.AddCategory(RegexCharClass.NotWord);
                            }
                        }
                        continue;
                    }
                    default:
                        goto Label_0300;
                }
            Label_00C7:
                if (ch3 <= 'p')
                {
                    switch (ch3)
                    {
                        case 'd':
                            goto Label_00F8;

                        case 'p':
                            goto Label_02B8;
                    }
                }
                else
                {
                    switch (ch3)
                    {
                        case 's':
                            goto Label_0198;

                        case 'w':
                            goto Label_0228;
                    }
                }
                goto Label_0300;
            Label_00F8:
                if (!scanOnly)
                {
                    if (flag)
                    {
                        throw this.MakeException(RegExRes.GetString(13, last.ToString()));
                    }
                    if (this.UseOptionE())
                    {
                        class2.AddSet("0:");
                    }
                    else
                    {
                        class2.AddCategoryFromName("Nd", false, false, this._pattern);
                    }
                }
                continue;
            Label_0148:
                if (!scanOnly)
                {
                    if (flag)
                    {
                        throw this.MakeException(RegExRes.GetString(13, last.ToString()));
                    }
                    if (this.UseOptionE())
                    {
                        class2.AddSet("\00:");
                    }
                    else
                    {
                        class2.AddCategoryFromName("Nd", true, false, this._pattern);
                    }
                }
                continue;
            Label_0198:
                if (!scanOnly)
                {
                    if (flag)
                    {
                        throw this.MakeException(RegExRes.GetString(13, last.ToString()));
                    }
                    if (this.UseOptionE())
                    {
                        class2.AddSet("\t\x000e !");
                    }
                    else
                    {
                        class2.AddCategory(RegexCharClass.Space);
                    }
                }
                continue;
            Label_0228:
                if (!scanOnly)
                {
                    if (flag)
                    {
                        throw this.MakeException(RegExRes.GetString(13, last.ToString()));
                    }
                    if (this.UseOptionE())
                    {
                        class2.AddSet("0:A[_`a{İı");
                    }
                    else
                    {
                        class2.AddCategory(RegexCharClass.Word);
                    }
                }
                continue;
            Label_02B8:
                if (!scanOnly)
                {
                    if (flag)
                    {
                        throw this.MakeException(RegExRes.GetString(13, last.ToString()));
                    }
                    class2.AddCategoryFromName(this.ParseProperty(), last != 'p', caseInsensitive, this._pattern);
                }
                else
                {
                    this.ParseProperty();
                }
                continue;
            Label_0300:
                this.LeftNext();
                last = this.ScanCharEscape();
            Label_0360:
                if (flag)
                {
                    flag = false;
                    if (!scanOnly)
                    {
                        if (first > last)
                        {
                            throw this.MakeException(RegExRes.GetString(14));
                        }
                        class2.AddRange(first, last);
                    }
                }
                else if (((this.CharsRight() >= 2) && (this.RightChar() == '-')) && (this.RightChar(1) != ']'))
                {
                    first = last;
                    flag = true;
                    this.RightNext();
                }
                else if (!scanOnly)
                {
                    class2.AddRange(last, last);
                }
            }
            return class2;
        }

        internal char ScanCharEscape()
        {
            char ch = this.RightCharNext();
            if ((ch >= '0') && (ch <= '7'))
            {
                this.LeftNext();
                return this.ScanOctal();
            }
            switch (ch)
            {
                case 'a':
                    return '\a';

                case 'b':
                    return '\b';

                case 'c':
                    return this.ScanControl();

                case 'e':
                    return '\x001b';

                case 'f':
                    return '\f';

                case 'n':
                    return '\n';

                case 'r':
                    return '\r';

                case 't':
                    return '\t';

                case 'u':
                    return this.ScanHex(4);

                case 'v':
                    return '\v';

                case 'x':
                    return this.ScanHex(2);
            }
            if (!this.UseOptionE() && RegexCharClass.IsWordChar(ch))
            {
                throw this.MakeException(RegExRes.GetString(0x1a, ch.ToString()));
            }
            return ch;
        }

        internal char ScanControl()
        {
            if (this.CharsRight() <= 0)
            {
                throw this.MakeException(RegExRes.GetString(0x18));
            }
            char ch = this.RightCharNext();
            if ((ch >= 'a') && (ch <= 'z'))
            {
                ch = (char) (ch - ' ');
            }
            if (((ch = (char) (ch - '@')) >= ' ') || (ch < '\0'))
            {
                throw this.MakeException(RegExRes.GetString(0x19));
            }
            return ch;
        }

        internal int ScanDecimal()
        {
            int num2;
            int num = 0;
            while (((this.CharsRight() > 0) && ((num2 = this.RightChar() - '0') <= 9)) && (num2 >= 0))
            {
                this.RightNext();
                if ((num > 0xccccccc) || ((num == 0xccccccc) && (num2 > 7)))
                {
                    num = 0x7fffffff;
                }
                num *= 10;
                num += num2;
            }
            return num;
        }

        internal RegexNode ScanDollar()
        {
            if (this.CharsRight() != 0)
            {
                bool flag;
                int pos = this.Textpos();
                char ch = this.RightChar();
                if ((ch == '{') && (this.CharsRight() > 1))
                {
                    flag = true;
                    this.RightNext();
                    ch = this.RightChar();
                }
                else
                {
                    flag = false;
                }
                if ((ch >= '0') && (ch <= '9'))
                {
                    int i = this.ScanDecimal();
                    if ((!flag || ((this.CharsRight() > 0) && (this.RightCharNext() == '}'))) && this.IsCaptureSlot(i))
                    {
                        return new RegexNode(13, this._options, i);
                    }
                }
                else if (flag && RegexCharClass.IsWordChar(ch))
                {
                    string capname = this.ScanCapname();
                    if (((this.CharsRight() > 0) && (this.RightCharNext() == '}')) && this.IsCaptureName(capname))
                    {
                        return new RegexNode(13, this._options, this.CaptureSlotFromName(capname));
                    }
                }
                else if (!flag)
                {
                    int m = 1;
                    switch (ch)
                    {
                        case '$':
                            this.RightNext();
                            return new RegexNode(9, this._options, '$');

                        case '&':
                            m = 0;
                            break;

                        case '\'':
                            m = -2;
                            break;

                        case '+':
                            m = -3;
                            break;

                        case '_':
                            m = -4;
                            break;

                        case '`':
                            m = -1;
                            break;
                    }
                    if (m != 1)
                    {
                        this.RightNext();
                        return new RegexNode(13, this._options, m);
                    }
                }
                this.Textto(pos);
            }
            return new RegexNode(9, this._options, '$');
        }

        internal RegexNode ScanGroupOpen()
        {
            int num;
            char ch = '\0';
            char ch2 = '>';
            if (((this.CharsRight() == 0) || (this.RightChar() != '?')) || ((this.RightChar() == '?') && (this.RightChar(1) == ')')))
            {
                if (!this.UseOptionN() && !this._ignoreNextParen)
                {
                    return new RegexNode(0x1c, this._options, this._autocap++, -1);
                }
                this._ignoreNextParen = false;
                return new RegexNode(0x1d, this._options);
            }
            this.RightNext();
            if (this.CharsRight() == 0)
            {
                goto Label_04C6;
            }
            switch ((ch = this.RightCharNext()))
            {
                case '\'':
                    ch2 = '\'';
                    break;

                case '(':
                {
                    int num4 = this.Textpos();
                    ch = this.RightChar();
                    if ((ch < '0') || (ch > '9'))
                    {
                        if (RegexCharClass.IsWordChar(ch))
                        {
                            string capname = this.ScanCapname();
                            if ((this.IsCaptureName(capname) && (this.CharsRight() > 0)) && (this.RightCharNext() == ')'))
                            {
                                return new RegexNode(0x21, this._options, this.CaptureSlotFromName(capname));
                            }
                        }
                        num = 0x22;
                        this.Textto(num4 - 1);
                        this._ignoreNextParen = true;
                        int num6 = this.CharsRight();
                        if ((num6 >= 3) && (this.RightChar(1) == '?'))
                        {
                            char ch3 = this.RightChar(2);
                            switch (ch3)
                            {
                                case '#':
                                    throw this.MakeException(RegExRes.GetString(0x23));

                                case '\'':
                                    throw this.MakeException(RegExRes.GetString(0x22));
                            }
                            if (((num6 >= 4) && (ch3 == '<')) && ((this.RightChar(3) != '!') && (this.RightChar(3) != '=')))
                            {
                                throw this.MakeException(RegExRes.GetString(0x22));
                            }
                        }
                        goto Label_04B9;
                    }
                    int i = this.ScanDecimal();
                    if ((this.CharsRight() <= 0) || (this.RightCharNext() != ')'))
                    {
                        throw this.MakeException(RegExRes.GetString(0x10, i.ToString()));
                    }
                    if (!this.IsCaptureSlot(i))
                    {
                        throw this.MakeException(RegExRes.GetString(15, i.ToString()));
                    }
                    return new RegexNode(0x21, this._options, i);
                }
                case '!':
                    this._options &= ~RegexOptions.RightToLeft;
                    num = 0x1f;
                    goto Label_04B9;

                case ':':
                    num = 0x1d;
                    goto Label_04B9;

                case '<':
                    break;

                case '=':
                    this._options &= ~RegexOptions.RightToLeft;
                    num = 30;
                    goto Label_04B9;

                case '>':
                    num = 0x20;
                    goto Label_04B9;

                default:
                    this.LeftNext();
                    num = 0x1d;
                    this.ScanOptions();
                    if (this.CharsRight() == 0)
                    {
                        goto Label_04C6;
                    }
                    ch = this.RightCharNext();
                    if (ch == ')')
                    {
                        return null;
                    }
                    if (ch != ':')
                    {
                        goto Label_04C6;
                    }
                    goto Label_04B9;
            }
            if (this.CharsRight() == 0)
            {
                goto Label_04C6;
            }
            char ch4 = ch = this.RightCharNext();
            if (ch4 != '!')
            {
                if (ch4 != '=')
                {
                    this.LeftNext();
                    int num2 = -1;
                    int num3 = -1;
                    bool flag = false;
                    if ((ch >= '0') && (ch <= '9'))
                    {
                        num2 = this.ScanDecimal();
                        if (!this.IsCaptureSlot(num2))
                        {
                            num2 = -1;
                        }
                        if (((this.CharsRight() > 0) && (this.RightChar() != ch2)) && (this.RightChar() != '-'))
                        {
                            throw this.MakeException(RegExRes.GetString(0x20));
                        }
                        if (num2 == 0)
                        {
                            throw this.MakeException(RegExRes.GetString(0x21));
                        }
                    }
                    else if (RegexCharClass.IsWordChar(ch))
                    {
                        string str = this.ScanCapname();
                        if (this.IsCaptureName(str))
                        {
                            num2 = this.CaptureSlotFromName(str);
                        }
                        if (((this.CharsRight() > 0) && (this.RightChar() != ch2)) && (this.RightChar() != '-'))
                        {
                            throw this.MakeException(RegExRes.GetString(0x20));
                        }
                    }
                    else
                    {
                        if (ch != '-')
                        {
                            throw this.MakeException(RegExRes.GetString(0x20));
                        }
                        flag = true;
                    }
                    if (((num2 != -1) || flag) && ((this.CharsRight() > 0) && (this.RightChar() == '-')))
                    {
                        this.RightNext();
                        ch = this.RightChar();
                        if ((ch >= '0') && (ch <= '9'))
                        {
                            num3 = this.ScanDecimal();
                            if (!this.IsCaptureSlot(num3))
                            {
                                throw this.MakeException();
                            }
                            if ((this.CharsRight() > 0) && (this.RightChar() != ch2))
                            {
                                throw this.MakeException(RegExRes.GetString(0x20));
                            }
                        }
                        else
                        {
                            if (!RegexCharClass.IsWordChar(ch))
                            {
                                throw this.MakeException(RegExRes.GetString(0x20));
                            }
                            string str2 = this.ScanCapname();
                            if (!this.IsCaptureName(str2))
                            {
                                throw this.MakeException(RegExRes.GetString(0x16, str2));
                            }
                            num3 = this.CaptureSlotFromName(str2);
                            if ((this.CharsRight() > 0) && (this.RightChar() != ch2))
                            {
                                throw this.MakeException(RegExRes.GetString(0x20));
                            }
                        }
                    }
                    if (((num2 != -1) || (num3 != -1)) && ((this.CharsRight() > 0) && (this.RightCharNext() == ch2)))
                    {
                        return new RegexNode(0x1c, this._options, num2, num3);
                    }
                    goto Label_04C6;
                }
                if (ch2 == '\'')
                {
                    goto Label_04C6;
                }
                this._options |= RegexOptions.RightToLeft;
                num = 30;
            }
            else
            {
                if (ch2 == '\'')
                {
                    goto Label_04C6;
                }
                this._options |= RegexOptions.RightToLeft;
                num = 0x1f;
            }
        Label_04B9:
            return new RegexNode(num, this._options);
        Label_04C6:
            throw this.MakeException(RegExRes.GetString(0x11));
        }

        internal char ScanHex(int c)
        {
            int num = 0;
            if (this.CharsRight() >= c)
            {
                int num2;
                while ((c > 0) && ((num2 = HexDigit(this.RightCharNext())) >= 0))
                {
                    num *= 0x10;
                    num += num2;
                    c--;
                }
            }
            if (c > 0)
            {
                throw this.MakeException(RegExRes.GetString(0x17));
            }
            return (char) num;
        }

        internal char ScanOctal()
        {
            int num;
            int num3 = 3;
            if (num3 > this.CharsRight())
            {
                num3 = this.CharsRight();
            }
            int num2 = 0;
            while (((num3 > 0) && ((num = this.RightChar() - '0') <= 7)) && (num >= 0))
            {
                this.RightNext();
                num2 *= 8;
                num2 += num;
                if (this.UseOptionE() && (num2 >= 0x20))
                {
                    break;
                }
                num3--;
            }
            num2 &= 0x7f;
            return (char) num2;
        }

        internal void ScanOptions()
        {
            bool flag = false;
            while (this.CharsRight() > 0)
            {
                char ch = this.RightChar();
                switch (ch)
                {
                    case '-':
                        flag = true;
                        break;

                    case '+':
                        flag = false;
                        break;

                    default:
                    {
                        RegexOptions option = OptionFromCode(ch);
                        if ((option == RegexOptions.None) || this.IsOnlyTopOption(option))
                        {
                            return;
                        }
                        if (flag)
                        {
                            this._options &= ~option;
                        }
                        else
                        {
                            this._options |= option;
                        }
                        break;
                    }
                }
                this.RightNext();
            }
        }

        internal RegexNode ScanRegex()
        {
            char ch = '@';
            bool flag = false;
            this.StartGroup(new RegexNode(0x1c, this._options, 0, -1));
        Label_0436:
            while (this.CharsRight() > 0)
            {
                int num2;
                RegexNode node;
                bool flag2 = flag;
                flag = false;
                this.ScanBlank();
                int pos = this.Textpos();
                if (!this.UseOptionX())
                {
                    goto Label_006F;
                }
                while ((this.CharsRight() > 0) && (!IsStopperX(ch = this.RightChar()) || ((ch == '{') && !this.IsTrueQuantifier())))
                {
                    this.RightNext();
                }
                goto Label_0094;
            Label_0068:
                this.RightNext();
            Label_006F:
                if ((this.CharsRight() > 0) && (!IsSpecial(ch = this.RightChar()) || ((ch == '{') && !this.IsTrueQuantifier())))
                {
                    goto Label_0068;
                }
            Label_0094:
                num2 = this.Textpos();
                this.ScanBlank();
                if (this.CharsRight() == 0)
                {
                    ch = '!';
                }
                else if (IsSpecial(ch = this.RightChar()))
                {
                    flag = IsQuantifier(ch);
                    this.RightNext();
                }
                else
                {
                    ch = ' ';
                }
                if (pos < num2)
                {
                    int cch = (num2 - pos) - (flag ? 1 : 0);
                    flag2 = false;
                    if (cch > 0)
                    {
                        this.AddConcatenate(pos, cch);
                    }
                    if (flag)
                    {
                        this.AddUnitOne(this.CharAt(num2 - 1));
                    }
                }
                switch (ch)
                {
                    case ' ':
                    {
                        continue;
                    }
                    case '!':
                        goto Label_0442;

                    case '$':
                        this.AddUnitType(this.UseOptionM() ? 15 : 20);
                        goto Label_02D4;

                    case '(':
                    {
                        this.PushOptions();
                        node = this.ScanGroupOpen();
                        if (node != null)
                        {
                            break;
                        }
                        this.PopKeepOptions();
                        continue;
                    }
                    case ')':
                        if (this.EmptyStack())
                        {
                            throw this.MakeException(RegExRes.GetString(7));
                        }
                        goto Label_0218;

                    case '*':
                    case '+':
                    case '?':
                    case '{':
                        if (this.Unit() == null)
                        {
                            throw this.MakeException(flag2 ? RegExRes.GetString(8, ch.ToString()) : RegExRes.GetString(9));
                        }
                        this.LeftNext();
                        goto Label_02D4;

                    case '.':
                        if (!this.UseOptionS())
                        {
                            goto Label_028C;
                        }
                        this.AddUnitSet(RegexCharClass.AnyClass);
                        goto Label_02D4;

                    case '[':
                        this.AddUnitSet(this.ScanCharClass(this.UseOptionI()));
                        if ((this.CharsRight() == 0) || (this.RightCharNext() != ']'))
                        {
                            throw this.MakeException(RegExRes.GetString(6));
                        }
                        goto Label_02D4;

                    case '\\':
                        this.AddUnitNode(this.ScanBackslash());
                        goto Label_02D4;

                    case '^':
                        this.AddUnitType(this.UseOptionM() ? 14 : 0x12);
                        goto Label_02D4;

                    case '|':
                    {
                        this.AddAlternate();
                        continue;
                    }
                    default:
                        throw this.MakeException(RegExRes.GetString(10));
                }
                this.PushGroup();
                this.StartGroup(node);
                continue;
            Label_0218:
                this.AddGroup();
                this.PopGroup();
                this.PopOptions();
                if (this.Unit() != null)
                {
                    goto Label_02D4;
                }
                continue;
            Label_028C:
                this.AddUnitNotone('\n');
            Label_02D4:
                this.ScanBlank();
                if ((this.CharsRight() == 0) || !(flag = this.IsTrueQuantifier()))
                {
                    this.AddConcatenate();
                }
                else
                {
                    ch = this.RightCharNext();
                    while (this.Unit() != null)
                    {
                        int num4;
                        int num5;
                        bool flag3;
                        switch (ch)
                        {
                            case '*':
                                num4 = 0;
                                num5 = 0x7fffffff;
                                goto Label_03E4;

                            case '+':
                                num4 = 1;
                                num5 = 0x7fffffff;
                                goto Label_03E4;

                            case '?':
                                num4 = 0;
                                num5 = 1;
                                goto Label_03E4;

                            case '{':
                                pos = this.Textpos();
                                num5 = num4 = this.ScanDecimal();
                                if (((pos < this.Textpos()) && (this.CharsRight() > 0)) && (this.RightChar() == ','))
                                {
                                    this.RightNext();
                                    if ((this.CharsRight() != 0) && (this.RightChar() != '}'))
                                    {
                                        break;
                                    }
                                    num5 = 0x7fffffff;
                                }
                                goto Label_03AA;

                            default:
                                throw this.MakeException(RegExRes.GetString(10));
                        }
                        num5 = this.ScanDecimal();
                    Label_03AA:
                        if (((pos == this.Textpos()) || (this.CharsRight() == 0)) || (this.RightCharNext() != '}'))
                        {
                            this.AddConcatenate();
                            this.Textto(pos - 1);
                            goto Label_0436;
                        }
                    Label_03E4:
                        this.ScanBlank();
                        if ((this.CharsRight() == 0) || (this.RightChar() != '?'))
                        {
                            flag3 = false;
                        }
                        else
                        {
                            this.RightNext();
                            flag3 = true;
                        }
                        if (num4 > num5)
                        {
                            throw this.MakeException(RegExRes.GetString(11));
                        }
                        this.AddConcatenate(flag3, num4, num5);
                    }
                }
            }
        Label_0442:
            if (!this.EmptyStack())
            {
                throw this.MakeException(RegExRes.GetString(12));
            }
            this.AddGroup();
            return this.Unit();
        }

        internal RegexNode ScanReplacement()
        {
            this._concatenation = new RegexNode(0x19, this._options);
            while (true)
            {
                int num;
                do
                {
                    num = this.CharsRight();
                    if (num == 0)
                    {
                        return this._concatenation;
                    }
                    int pos = this.Textpos();
                    while ((num > 0) && (this.RightChar() != '$'))
                    {
                        this.RightNext();
                        num--;
                    }
                    this.AddConcatenate(pos, this.Textpos() - pos);
                }
                while (num <= 0);
                if (this.RightCharNext() == '$')
                {
                    this.AddUnitNode(this.ScanDollar());
                }
                this.AddConcatenate();
            }
        }

        internal void SetPattern(string Re)
        {
            if (Re == null)
            {
                Re = string.Empty;
            }
            this._pattern = Re;
            this._currentPos = 0;
        }

        internal void StartGroup(RegexNode openGroup)
        {
            this._group = openGroup;
            this._alternation = new RegexNode(0x18, this._options);
            this._concatenation = new RegexNode(0x19, this._options);
        }

        internal int Textpos()
        {
            return this._currentPos;
        }

        internal void Textto(int pos)
        {
            this._currentPos = pos;
        }

        internal int TypeFromCode(char ch)
        {
            switch (ch)
            {
                case 'A':
                    return 0x12;

                case 'B':
                    if (this.UseOptionE())
                    {
                        return 0x2a;
                    }
                    return 0x11;

                case 'G':
                    return 0x13;

                case 'Z':
                    return 20;

                case 'b':
                    if (!this.UseOptionE())
                    {
                        return 0x10;
                    }
                    return 0x29;

                case 'z':
                    return 0x15;
            }
            return 0x16;
        }

        internal static string Unescape(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\')
                {
                    StringBuilder builder = new StringBuilder();
                    RegexParser parser = new RegexParser(CultureInfo.InvariantCulture);
                    parser.SetPattern(input);
                    builder.Append(input, 0, i);
                    do
                    {
                        i++;
                        parser.Textto(i);
                        if (i < input.Length)
                        {
                            builder.Append(parser.ScanCharEscape());
                        }
                        i = parser.Textpos();
                        int startIndex = i;
                        while ((i < input.Length) && (input[i] != '\\'))
                        {
                            i++;
                        }
                        builder.Append(input, startIndex, i - startIndex);
                    }
                    while (i < input.Length);
                    return builder.ToString();
                }
            }
            return input;
        }

        internal RegexNode Unit()
        {
            return this._unit;
        }

        internal bool UseOptionE()
        {
            return ((this._options & RegexOptions.ECMAScript) != RegexOptions.None);
        }

        internal bool UseOptionI()
        {
            return ((this._options & RegexOptions.IgnoreCase) != RegexOptions.None);
        }

        internal bool UseOptionM()
        {
            return ((this._options & RegexOptions.Multiline) != RegexOptions.None);
        }

        internal bool UseOptionN()
        {
            return ((this._options & RegexOptions.ExplicitCapture) != RegexOptions.None);
        }

        internal bool UseOptionS()
        {
            return ((this._options & RegexOptions.Singleline) != RegexOptions.None);
        }

        internal bool UseOptionX()
        {
            return ((this._options & RegexOptions.IgnorePatternWhitespace) != RegexOptions.None);
        }
    }
}

