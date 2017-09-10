namespace System.Text.RegularExpressions
{
    using System;
    using System.Globalization;
    using System.Text;

    internal sealed class RegexFCD
    {
        internal bool _earlyexit;
        internal int _fcDepth;
        internal RegexFC[] _fcStack = new RegexFC[0x20];
        internal int _intDepth;
        internal int[] _intStack = new int[0x20];
        internal bool _skipchild;
        internal const int AfterChild = 0x80;
        internal const int BeforeChild = 0x40;
        internal const int Beginning = 1;
        internal const int Bol = 2;
        internal const int Boundary = 0x40;
        internal const int ECMABoundary = 0x80;
        internal const int End = 0x20;
        internal const int EndZ = 0x10;
        internal const int Eol = 8;
        internal const int infinite = 0x7fffffff;
        internal const int Start = 4;

        private RegexFCD()
        {
        }

        internal static int AnchorFromType(int type)
        {
            switch (type)
            {
                case 14:
                    return 2;

                case 15:
                    return 8;

                case 0x10:
                    return 0x40;

                case 0x12:
                    return 1;

                case 0x13:
                    return 4;

                case 20:
                    return 0x10;

                case 0x15:
                    return 0x20;

                case 0x29:
                    return 0x80;
            }
            return 0;
        }

        internal static int Anchors(RegexTree tree)
        {
            RegexNode node2 = null;
            int num = 0;
            int num2 = 0;
            RegexNode node = tree._root;
        Label_000D:
            switch (node._type)
            {
                case 14:
                case 15:
                case 0x10:
                case 0x12:
                case 0x13:
                case 20:
                case 0x15:
                case 0x29:
                    return (num2 | AnchorFromType(node._type));

                case 0x11:
                case 0x16:
                case 0x18:
                case 0x1a:
                case 0x1b:
                case 0x1d:
                    return num2;

                case 0x17:
                case 30:
                case 0x1f:
                    break;

                case 0x19:
                    if (node.ChildCount() > 0)
                    {
                        node2 = node;
                        num = 0;
                    }
                    break;

                case 0x1c:
                case 0x20:
                    node = node.Child(0);
                    node2 = null;
                    goto Label_000D;

                default:
                    return num2;
            }
            if ((node2 == null) || (num >= node2.ChildCount()))
            {
                return num2;
            }
            node = node2.Child(num++);
            goto Label_000D;
        }

        internal void CalculateFC(int NodeType, RegexNode node, int CurIndex)
        {
            bool caseInsensitive = false;
            bool flag2 = false;
            if (NodeType <= 13)
            {
                if ((node._options & RegexOptions.IgnoreCase) != RegexOptions.None)
                {
                    caseInsensitive = true;
                }
                if ((node._options & RegexOptions.RightToLeft) != RegexOptions.None)
                {
                    flag2 = true;
                }
            }
            switch (NodeType)
            {
                case 3:
                case 6:
                    this.PushFC(new RegexFC(node._ch, false, node._m == 0, caseInsensitive));
                    return;

                case 4:
                case 7:
                    this.PushFC(new RegexFC(node._ch, true, node._m == 0, caseInsensitive));
                    return;

                case 5:
                case 8:
                    this.PushFC(new RegexFC(node._str, true, caseInsensitive));
                    return;

                case 9:
                case 10:
                    this.PushFC(new RegexFC(node._ch, NodeType == 10, false, caseInsensitive));
                    return;

                case 11:
                    this.PushFC(new RegexFC(node._str, (node._str2 != null) && (node._str2.Length != 0), caseInsensitive));
                    return;

                case 12:
                    if (node._str.Length != 0)
                    {
                        if (!flag2)
                        {
                            this.PushFC(new RegexFC(node._str[0], false, false, caseInsensitive));
                            return;
                        }
                        this.PushFC(new RegexFC(node._str[node._str.Length - 1], false, false, caseInsensitive));
                        return;
                    }
                    this.PushFC(new RegexFC(true));
                    return;

                case 13:
                    this.PushFC(new RegexFC("\0", true, false));
                    return;

                case 14:
                case 15:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 20:
                case 0x15:
                case 0x16:
                case 0x29:
                case 0x2a:
                    this.PushFC(new RegexFC(true));
                    return;

                case 0x17:
                    this.PushFC(new RegexFC(true));
                    return;

                case 0x58:
                case 0x59:
                case 90:
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x60:
                case 0x61:
                case 0x9c:
                case 0x9d:
                case 0x9e:
                case 0x9f:
                case 160:
                    return;

                case 0x5e:
                case 0x5f:
                    this.SkipChild();
                    this.PushFC(new RegexFC(true));
                    return;

                case 0x62:
                    if (CurIndex == 0)
                    {
                        this.SkipChild();
                    }
                    return;

                case 0x98:
                case 0xa1:
                    if (CurIndex != 0)
                    {
                        RegexFC fc = this.PopFC();
                        this.TopFC().AddFC(fc, false);
                    }
                    return;

                case 0x99:
                    if (CurIndex != 0)
                    {
                        RegexFC xfc = this.PopFC();
                        this.TopFC().AddFC(xfc, true);
                    }
                    if (!this.TopFC()._nullable)
                    {
                        this.EarlyExit();
                    }
                    return;

                case 0x9a:
                case 0x9b:
                    if (node._m == 0)
                    {
                        this.TopFC()._nullable = true;
                    }
                    return;

                case 0xa2:
                    if (CurIndex > 1)
                    {
                        RegexFC xfc3 = this.PopFC();
                        this.TopFC().AddFC(xfc3, false);
                    }
                    return;
            }
            throw new ArgumentException(RegExRes.GetString(4, NodeType.ToString()));
        }

        internal void EarlyExit()
        {
            this._earlyexit = true;
        }

        internal bool EmptyFC()
        {
            return (this._fcDepth == 0);
        }

        internal bool EmptyInt()
        {
            return (this._intDepth == 0);
        }

        internal static RegexPrefix FirstChars(RegexTree t)
        {
            RegexFC xfc = new RegexFCD().RegexFCFromRegexTree(t);
            if (xfc._nullable)
            {
                return null;
            }
            CultureInfo culture = ((t._options & RegexOptions.CultureInvariant) != RegexOptions.None) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
            return new RegexPrefix(xfc.GetFirstChars(culture), xfc.IsCaseInsensitive());
        }

        internal RegexFC PopFC()
        {
            return this._fcStack[--this._fcDepth];
        }

        internal int PopInt()
        {
            return this._intStack[--this._intDepth];
        }

        internal static RegexPrefix Prefix(RegexTree tree)
        {
            RegexNode node2 = null;
            int num2;
            int num = 0;
            RegexNode node = tree._root;
        Label_000B:
            num2 = node._type;
            switch (num2)
            {
                case 3:
                case 6:
                case 12:
                    num2 = node._type;
                    switch (num2)
                    {
                        case 3:
                        case 6:
                            if (node._m > 0)
                            {
                                StringBuilder builder = new StringBuilder();
                                builder.Append(node._ch, node._m);
                                return new RegexPrefix(builder.ToString(), RegexOptions.None != (node._options & RegexOptions.IgnoreCase));
                            }
                            goto Label_0151;
                    }
                    if (num2 != 12)
                    {
                        goto Label_0151;
                    }
                    return new RegexPrefix(node._str, RegexOptions.None != (node._options & RegexOptions.IgnoreCase));

                case 14:
                case 15:
                case 0x10:
                case 0x12:
                case 0x13:
                case 20:
                case 0x15:
                case 0x17:
                case 30:
                case 0x1f:
                case 0x29:
                    break;

                case 0x19:
                    if (node.ChildCount() > 0)
                    {
                        node2 = node;
                        num = 0;
                    }
                    break;

                case 0x1c:
                case 0x20:
                    node = node.Child(0);
                    node2 = null;
                    goto Label_000B;

                default:
                    return RegexPrefix.Empty;
            }
            if ((node2 == null) || (num >= node2.ChildCount()))
            {
                return RegexPrefix.Empty;
            }
            node = node2.Child(num++);
            goto Label_000B;
        Label_0151:
            return RegexPrefix.Empty;
        }

        internal void PushFC(RegexFC fc)
        {
            if (this._fcDepth >= this._fcStack.Length)
            {
                RegexFC[] destinationArray = new RegexFC[this._fcDepth * 2];
                Array.Copy(this._fcStack, 0, destinationArray, 0, this._fcDepth);
                this._fcStack = destinationArray;
            }
            this._fcStack[this._fcDepth++] = fc;
        }

        internal void PushInt(int I)
        {
            if (this._intDepth >= this._intStack.Length)
            {
                int[] destinationArray = new int[this._intDepth * 2];
                Array.Copy(this._intStack, 0, destinationArray, 0, this._intDepth);
                this._intStack = destinationArray;
            }
            this._intStack[this._intDepth++] = I;
        }

        internal RegexFC RegexFCFromRegexTree(RegexTree tree)
        {
            RegexNode node = tree._root;
            int curIndex = 0;
        Label_0009:
            if (node._children == null)
            {
                this.CalculateFC(node._type, node, 0);
            }
            else if ((curIndex < node._children.Count) && !this._earlyexit)
            {
                this.CalculateFC(node._type | 0x40, node, curIndex);
                if (!this._skipchild)
                {
                    node = (RegexNode) node._children[curIndex];
                    this.PushInt(curIndex);
                    curIndex = 0;
                }
                else
                {
                    curIndex++;
                    this._skipchild = false;
                }
                goto Label_0009;
            }
            this._earlyexit = false;
            if (!this.EmptyInt())
            {
                curIndex = this.PopInt();
                node = node._next;
                this.CalculateFC(node._type | 0x80, node, curIndex);
                curIndex++;
                goto Label_0009;
            }
            if (this.EmptyFC())
            {
                return new RegexFC("\0", true, false);
            }
            return this.PopFC();
        }

        internal static RegexPrefix ScanChars(RegexTree tree)
        {
            RegexNode node2 = null;
            int num = 0;
            string prefix = null;
            bool ci = false;
            RegexNode node = tree._root;
        Label_0010:
            switch (node._type)
            {
                case 3:
                case 6:
                    if (node._n == 0x7fffffff)
                    {
                        prefix = RegexCharClass.SetFromChar(node._ch);
                        ci = RegexOptions.None != (node._options & RegexOptions.IgnoreCase);
                        break;
                    }
                    return null;

                case 4:
                case 7:
                    if (node._n == 0x7fffffff)
                    {
                        prefix = RegexCharClass.SetInverseFromChar(node._ch);
                        ci = RegexOptions.None != (node._options & RegexOptions.IgnoreCase);
                        break;
                    }
                    return null;

                case 5:
                case 8:
                    if ((node._n == 0x7fffffff) && ((node._str2 == null) || (node._str2.Length == 0)))
                    {
                        prefix = node._str;
                        ci = RegexOptions.None != (node._options & RegexOptions.IgnoreCase);
                        break;
                    }
                    return null;

                case 14:
                case 15:
                case 0x10:
                case 0x12:
                case 0x13:
                case 20:
                case 0x15:
                case 0x17:
                case 30:
                case 0x1f:
                case 0x29:
                    break;

                case 0x19:
                    if (node.ChildCount() > 0)
                    {
                        node2 = node;
                        num = 0;
                    }
                    break;

                case 0x1c:
                case 0x20:
                    node = node.Child(0);
                    node2 = null;
                    goto Label_0010;

                default:
                    return null;
            }
            if (prefix != null)
            {
                return new RegexPrefix(prefix, ci);
            }
            if ((node2 == null) || (num >= node2.ChildCount()))
            {
                return null;
            }
            node = node2.Child(num++);
            goto Label_0010;
        }

        internal void SkipChild()
        {
            this._skipchild = true;
        }

        internal RegexFC TopFC()
        {
            return this._fcStack[this._fcDepth - 1];
        }
    }
}

