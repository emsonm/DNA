namespace System.Text.RegularExpressions
{
    using System;
    using System.Globalization;

    internal sealed class RegexInterpreter : RegexRunner
    {
        internal const int infinite = 0x7fffffff;
        internal int runanchors;
        internal RegexBoyerMoore runbmPrefix;
        internal bool runci;
        internal RegexCode runcode;
        internal int runcodepos;
        internal int[] runcodes;
        internal CultureInfo runculture;
        internal RegexPrefix runfcPrefix;
        internal int runoperator;
        internal bool runrtl;
        internal RegexPrefix runscPrefix;
        internal string[] runstrings;

        internal RegexInterpreter(RegexCode code, CultureInfo culture)
        {
            this.runcode = code;
            this.runcodes = code._codes;
            this.runstrings = code._strings;
            this.runfcPrefix = code._fcPrefix;
            this.runscPrefix = code._scPrefix;
            this.runbmPrefix = code._bmPrefix;
            this.runanchors = code._anchors;
            this.runculture = culture;
        }

        private void Advance()
        {
            this.Advance(0);
        }

        private void Advance(int i)
        {
            this.runcodepos += i + 1;
            this.SetOperator(this.runcodes[this.runcodepos]);
        }

        private void Backtrack()
        {
            int index = base.runtrack[base.runtrackpos++];
            if (index < 0)
            {
                index = -index;
                this.SetOperator(this.runcodes[index] | 0x100);
            }
            else
            {
                this.SetOperator(this.runcodes[index] | 0x80);
            }
            if (index < this.runcodepos)
            {
                base.EnsureStorage();
            }
            this.runcodepos = index;
        }

        private void Backwardnext()
        {
            base.runtextpos += this.runrtl ? 1 : -1;
        }

        private int Bump()
        {
            if (!this.runrtl)
            {
                return 1;
            }
            return -1;
        }

        private char CharAt(int j)
        {
            return base.runtext[j];
        }

        protected override bool FindFirstChar()
        {
            int num;
            if ((this.runanchors & 0x35) != 0)
            {
                if (!this.runcode._rightToLeft)
                {
                    if ((((this.runanchors & 1) != 0) && (base.runtextpos > base.runtextbeg)) || (((this.runanchors & 4) != 0) && (base.runtextpos > base.runtextstart)))
                    {
                        base.runtextpos = base.runtextend;
                        return false;
                    }
                    if (((this.runanchors & 0x10) != 0) && (base.runtextpos < (base.runtextend - 1)))
                    {
                        base.runtextpos = base.runtextend - 1;
                    }
                    else if (((this.runanchors & 0x20) != 0) && (base.runtextpos < base.runtextend))
                    {
                        base.runtextpos = base.runtextend;
                    }
                }
                else
                {
                    if (((((this.runanchors & 0x20) != 0) && (base.runtextpos < base.runtextend)) || (((this.runanchors & 0x10) != 0) && ((base.runtextpos < (base.runtextend - 1)) || ((base.runtextpos == (base.runtextend - 1)) && (this.CharAt(base.runtextpos) != '\n'))))) || (((this.runanchors & 4) != 0) && (base.runtextpos < base.runtextstart)))
                    {
                        base.runtextpos = base.runtextbeg;
                        return false;
                    }
                    if (((this.runanchors & 1) != 0) && (base.runtextpos > base.runtextbeg))
                    {
                        base.runtextpos = base.runtextbeg;
                    }
                }
                if (this.runbmPrefix != null)
                {
                    return this.runbmPrefix.IsMatch(base.runtext, base.runtextpos, base.runtextbeg, base.runtextend);
                }
            }
            else if (this.runbmPrefix != null)
            {
                base.runtextpos = this.runbmPrefix.Scan(base.runtext, base.runtextpos, base.runtextbeg, base.runtextend);
                if (base.runtextpos == -1)
                {
                    base.runtextpos = this.runcode._rightToLeft ? base.runtextbeg : base.runtextend;
                    return false;
                }
                return true;
            }
            if (this.runfcPrefix == null)
            {
                return true;
            }
            this.runrtl = this.runcode._rightToLeft;
            this.runci = this.runfcPrefix.CaseInsensitive;
            string prefix = this.runfcPrefix.Prefix;
            if (RegexCharClass.IsSingleton(prefix))
            {
                char ch = RegexCharClass.SingletonChar(prefix);
                for (num = this.Forwardchars(); num > 0; num--)
                {
                    if (ch == this.Forwardcharnext())
                    {
                        this.Backwardnext();
                        return true;
                    }
                }
            }
            else
            {
                for (num = this.Forwardchars(); num > 0; num--)
                {
                    if (RegexCharClass.CharInSet(this.Forwardcharnext(), prefix, string.Empty))
                    {
                        this.Backwardnext();
                        return true;
                    }
                }
            }
            return false;
        }

        private char Forwardcharnext()
        {
            char c = this.runrtl ? base.runtext[--base.runtextpos] : base.runtext[base.runtextpos++];
            if (!this.runci)
            {
                return c;
            }
            return char.ToLower(c, this.runculture);
        }

        private int Forwardchars()
        {
            if (!this.runrtl)
            {
                return (base.runtextend - base.runtextpos);
            }
            return (base.runtextpos - base.runtextbeg);
        }

        protected override void Go()
        {
            this.Goto(0);
        Label_0007:
            switch (this.Operator())
            {
                case 0:
                {
                    int num12 = this.Operand(1);
                    if (this.Forwardchars() < num12)
                    {
                        goto Label_0EA3;
                    }
                    char ch = (char) this.Operand(0);
                    while (num12-- > 0)
                    {
                        if (this.Forwardcharnext() != ch)
                        {
                            goto Label_0EA3;
                        }
                    }
                    this.Advance(2);
                    goto Label_0007;
                }
                case 1:
                {
                    int num13 = this.Operand(1);
                    if (this.Forwardchars() < num13)
                    {
                        goto Label_0EA3;
                    }
                    char ch2 = (char) this.Operand(0);
                    while (num13-- > 0)
                    {
                        if (this.Forwardcharnext() == ch2)
                        {
                            goto Label_0EA3;
                        }
                    }
                    this.Advance(2);
                    goto Label_0007;
                }
                case 2:
                {
                    int num14 = this.Operand(2);
                    if (this.Forwardchars() < num14)
                    {
                        goto Label_0EA3;
                    }
                    string set = this.runstrings[this.Operand(0)];
                    string category = this.runstrings[this.Operand(1)];
                    while (num14-- > 0)
                    {
                        if (!RegexCharClass.CharInSet(this.Forwardcharnext(), set, category))
                        {
                            goto Label_0EA3;
                        }
                    }
                    this.Advance(3);
                    goto Label_0007;
                }
                case 3:
                {
                    int num15 = this.Operand(1);
                    if (num15 > this.Forwardchars())
                    {
                        num15 = this.Forwardchars();
                    }
                    char ch3 = (char) this.Operand(0);
                    int num16 = num15;
                    while (num16 > 0)
                    {
                        if (this.Forwardcharnext() != ch3)
                        {
                            this.Backwardnext();
                            break;
                        }
                        num16--;
                    }
                    if (num15 > num16)
                    {
                        this.Track((num15 - num16) - 1, this.Textpos() - this.Bump());
                    }
                    this.Advance(2);
                    goto Label_0007;
                }
                case 4:
                {
                    int num17 = this.Operand(1);
                    if (num17 > this.Forwardchars())
                    {
                        num17 = this.Forwardchars();
                    }
                    char ch4 = (char) this.Operand(0);
                    int num18 = num17;
                    while (num18 > 0)
                    {
                        if (this.Forwardcharnext() == ch4)
                        {
                            this.Backwardnext();
                            break;
                        }
                        num18--;
                    }
                    if (num17 > num18)
                    {
                        this.Track((num17 - num18) - 1, this.Textpos() - this.Bump());
                    }
                    this.Advance(2);
                    goto Label_0007;
                }
                case 5:
                {
                    int num19 = this.Operand(2);
                    if (num19 > this.Forwardchars())
                    {
                        num19 = this.Forwardchars();
                    }
                    string str3 = this.runstrings[this.Operand(0)];
                    string str4 = this.runstrings[this.Operand(1)];
                    int num20 = num19;
                    while (num20 > 0)
                    {
                        if (!RegexCharClass.CharInSet(this.Forwardcharnext(), str3, str4))
                        {
                            this.Backwardnext();
                            break;
                        }
                        num20--;
                    }
                    if (num19 > num20)
                    {
                        this.Track((num19 - num20) - 1, this.Textpos() - this.Bump());
                    }
                    this.Advance(3);
                    goto Label_0007;
                }
                case 6:
                case 7:
                {
                    int num25 = this.Operand(1);
                    if (num25 > this.Forwardchars())
                    {
                        num25 = this.Forwardchars();
                    }
                    if (num25 > 0)
                    {
                        this.Track(num25 - 1, this.Textpos());
                    }
                    this.Advance(2);
                    goto Label_0007;
                }
                case 8:
                {
                    int num26 = this.Operand(2);
                    if (num26 > this.Forwardchars())
                    {
                        num26 = this.Forwardchars();
                    }
                    if (num26 > 0)
                    {
                        this.Track(num26 - 1, this.Textpos());
                    }
                    this.Advance(3);
                    goto Label_0007;
                }
                case 9:
                    if ((this.Forwardchars() < 1) || (this.Forwardcharnext() != ((char) this.Operand(0))))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance(1);
                    goto Label_0007;

                case 10:
                    if ((this.Forwardchars() < 1) || (this.Forwardcharnext() == ((char) this.Operand(0))))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance(1);
                    goto Label_0007;

                case 11:
                    if ((this.Forwardchars() < 1) || !RegexCharClass.CharInSet(this.Forwardcharnext(), this.runstrings[this.Operand(0)], this.runstrings[this.Operand(1)]))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance(2);
                    goto Label_0007;

                case 12:
                    if (!this.Stringmatch(this.runstrings[this.Operand(0)]))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance(1);
                    goto Label_0007;

                case 13:
                {
                    int cap = this.Operand(0);
                    if (!base.IsMatched(cap))
                    {
                        if ((base.runregex.roptions & RegexOptions.ECMAScript) == RegexOptions.None)
                        {
                            goto Label_0EA3;
                        }
                        goto Label_0A06;
                    }
                    if (this.Refmatch(base.MatchIndex(cap), base.MatchLength(cap)))
                    {
                        goto Label_0A06;
                    }
                    goto Label_0EA3;
                }
                case 14:
                    if ((this.Leftchars() > 0) && (this.CharAt(this.Textpos() - 1) != '\n'))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 15:
                    if ((this.Rightchars() > 0) && (this.CharAt(this.Textpos()) != '\n'))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 0x10:
                    if (!base.IsBoundary(this.Textpos(), base.runtextbeg, base.runtextend))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 0x11:
                    if (base.IsBoundary(this.Textpos(), base.runtextbeg, base.runtextend))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 0x12:
                    if (this.Leftchars() > 0)
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 0x13:
                    if (this.Textpos() != this.Textstart())
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 20:
                    if ((this.Rightchars() > 1) || ((this.Rightchars() == 1) && (this.CharAt(this.Textpos()) != '\n')))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 0x15:
                    if (this.Rightchars() > 0)
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 0x16:
                    goto Label_0EA3;

                case 0x17:
                    this.Track(this.Textpos());
                    this.Advance(1);
                    goto Label_0007;

                case 0x18:
                    this.Stackframe(1);
                    if ((this.Textpos() - this.Stacked(0)) == 0)
                    {
                        this.Track2(this.Stacked(0));
                        this.Advance(1);
                    }
                    else
                    {
                        this.Track(this.Stacked(0), this.Textpos());
                        this.Stack(this.Textpos());
                        this.Goto(this.Operand(0));
                    }
                    goto Label_0007;

                case 0x19:
                    this.Stackframe(1);
                    if ((this.Textpos() - this.Stacked(0)) == 0)
                    {
                        this.Track2(this.Stacked(0));
                        break;
                    }
                    this.Track(this.Stacked(0), this.Textpos());
                    break;

                case 0x1a:
                    this.Stack(-1, this.Operand(0));
                    this.Track();
                    this.Advance(1);
                    goto Label_0007;

                case 0x1b:
                    this.Stack(this.Textpos(), this.Operand(0));
                    this.Track();
                    this.Advance(1);
                    goto Label_0007;

                case 0x1c:
                {
                    this.Stackframe(2);
                    int num4 = this.Stacked(0);
                    int num5 = this.Stacked(1);
                    int num6 = this.Textpos() - num4;
                    if ((num5 < this.Operand(1)) && ((num6 != 0) || (num5 < 0)))
                    {
                        this.Track(num4);
                        this.Stack(this.Textpos(), num5 + 1);
                        this.Goto(this.Operand(0));
                    }
                    else
                    {
                        this.Track2(num4, num5);
                        this.Advance(2);
                    }
                    goto Label_0007;
                }
                case 0x1d:
                {
                    this.Stackframe(2);
                    int num7 = this.Stacked(0);
                    int num8 = this.Stacked(1);
                    if (num8 >= 0)
                    {
                        this.Track(num7, num8, this.Textpos());
                        this.Advance(2);
                    }
                    else
                    {
                        this.Track2(num7);
                        this.Stack(this.Textpos(), num8 + 1);
                        this.Goto(this.Operand(0));
                    }
                    goto Label_0007;
                }
                case 30:
                    this.Stack(-1);
                    this.Track();
                    this.Advance();
                    goto Label_0007;

                case 0x1f:
                    this.Stack(this.Textpos());
                    this.Track();
                    this.Advance();
                    goto Label_0007;

                case 0x20:
                    if ((this.Operand(1) != -1) && !base.IsMatched(this.Operand(1)))
                    {
                        goto Label_0EA3;
                    }
                    this.Stackframe(1);
                    if (this.Operand(1) != -1)
                    {
                        base.TransferCapture(this.Operand(0), this.Operand(1), this.Stacked(0), this.Textpos());
                    }
                    else
                    {
                        base.Capture(this.Operand(0), this.Stacked(0), this.Textpos());
                    }
                    this.Track(this.Stacked(0));
                    this.Advance(2);
                    goto Label_0007;

                case 0x21:
                    this.Stackframe(1);
                    this.Track(this.Stacked(0));
                    this.Textto(this.Stacked(0));
                    this.Advance();
                    goto Label_0007;

                case 0x22:
                    this.Stack(this.Trackpos(), base.Crawlpos());
                    this.Track();
                    this.Advance();
                    goto Label_0007;

                case 0x23:
                    this.Stackframe(2);
                    this.Trackto(this.Stacked(0));
                    while (base.Crawlpos() != this.Stacked(1))
                    {
                        base.Uncapture();
                    }
                    goto Label_0EA3;

                case 0x24:
                    this.Stackframe(2);
                    this.Trackto(this.Stacked(0));
                    this.Track(this.Stacked(1));
                    this.Advance();
                    goto Label_0007;

                case 0x25:
                    if (!base.IsMatched(this.Operand(0)))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance(1);
                    goto Label_0007;

                case 0x26:
                    this.Goto(this.Operand(0));
                    goto Label_0007;

                case 40:
                    return;

                case 0x29:
                    if (!base.IsECMABoundary(this.Textpos(), base.runtextbeg, base.runtextend))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 0x2a:
                    if (base.IsECMABoundary(this.Textpos(), base.runtextbeg, base.runtextend))
                    {
                        goto Label_0EA3;
                    }
                    this.Advance();
                    goto Label_0007;

                case 0x83:
                case 0x84:
                {
                    this.Trackframe(2);
                    int num21 = this.Tracked(0);
                    int newpos = this.Tracked(1);
                    this.Textto(newpos);
                    if (num21 > 0)
                    {
                        this.Track(num21 - 1, newpos - this.Bump());
                    }
                    this.Advance(2);
                    goto Label_0007;
                }
                case 0x85:
                {
                    this.Trackframe(2);
                    int num23 = this.Tracked(0);
                    int num24 = this.Tracked(1);
                    this.Textto(num24);
                    if (num23 > 0)
                    {
                        this.Track(num23 - 1, num24 - this.Bump());
                    }
                    this.Advance(3);
                    goto Label_0007;
                }
                case 0x86:
                {
                    this.Trackframe(2);
                    int num27 = this.Tracked(1);
                    this.Textto(num27);
                    if (this.Forwardcharnext() != ((char) this.Operand(0)))
                    {
                        goto Label_0EA3;
                    }
                    int num28 = this.Tracked(0);
                    if (num28 > 0)
                    {
                        this.Track(num28 - 1, num27 + this.Bump());
                    }
                    this.Advance(2);
                    goto Label_0007;
                }
                case 0x87:
                {
                    this.Trackframe(2);
                    int num29 = this.Tracked(1);
                    this.Textto(num29);
                    if (this.Forwardcharnext() == ((char) this.Operand(0)))
                    {
                        goto Label_0EA3;
                    }
                    int num30 = this.Tracked(0);
                    if (num30 > 0)
                    {
                        this.Track(num30 - 1, num29 + this.Bump());
                    }
                    this.Advance(2);
                    goto Label_0007;
                }
                case 0x88:
                {
                    this.Trackframe(2);
                    int num31 = this.Tracked(1);
                    this.Textto(num31);
                    if (!RegexCharClass.CharInSet(this.Forwardcharnext(), this.runstrings[this.Operand(0)], this.runstrings[this.Operand(1)]))
                    {
                        goto Label_0EA3;
                    }
                    int num32 = this.Tracked(0);
                    if (num32 > 0)
                    {
                        this.Track(num32 - 1, num31 + this.Bump());
                    }
                    this.Advance(3);
                    goto Label_0007;
                }
                case 0x97:
                    this.Trackframe(1);
                    this.Textto(this.Tracked(0));
                    this.Goto(this.Operand(0));
                    goto Label_0007;

                case 0x98:
                    this.Trackframe(2);
                    this.Stackframe(1);
                    this.Textto(this.Tracked(1));
                    this.Track2(this.Tracked(0));
                    this.Advance(1);
                    goto Label_0007;

                case 0x99:
                {
                    this.Trackframe(2);
                    int num3 = this.Tracked(1);
                    this.Track2(this.Tracked(0));
                    this.Stack(num3);
                    this.Textto(num3);
                    this.Goto(this.Operand(0));
                    goto Label_0007;
                }
                case 0x9a:
                    this.Stackframe(2);
                    goto Label_0EA3;

                case 0x9b:
                    this.Stackframe(2);
                    goto Label_0EA3;

                case 0x9c:
                    this.Trackframe(1);
                    this.Stackframe(2);
                    if (this.Stacked(1) <= 0)
                    {
                        this.Stack(this.Tracked(0), this.Stacked(1) - 1);
                        goto Label_0EA3;
                    }
                    this.Textto(this.Stacked(0));
                    this.Track2(this.Tracked(0), this.Stacked(1) - 1);
                    this.Advance(2);
                    goto Label_0007;

                case 0x9d:
                {
                    this.Trackframe(3);
                    int num9 = this.Tracked(0);
                    int num10 = this.Tracked(2);
                    if ((this.Tracked(1) > this.Operand(1)) || (num10 == num9))
                    {
                        this.Stack(this.Tracked(0), this.Tracked(1));
                        goto Label_0EA3;
                    }
                    this.Textto(num10);
                    this.Stack(num10, this.Tracked(1) + 1);
                    this.Track2(num9);
                    this.Goto(this.Operand(0));
                    goto Label_0007;
                }
                case 0x9e:
                case 0x9f:
                    this.Stackframe(1);
                    goto Label_0EA3;

                case 160:
                    this.Trackframe(1);
                    this.Stack(this.Tracked(0));
                    base.Uncapture();
                    if ((this.Operand(0) != -1) && (this.Operand(1) != -1))
                    {
                        base.Uncapture();
                    }
                    goto Label_0EA3;

                case 0xa1:
                    this.Trackframe(1);
                    this.Stack(this.Tracked(0));
                    goto Label_0EA3;

                case 0xa2:
                    this.Stackframe(2);
                    goto Label_0EA3;

                case 0xa4:
                    this.Trackframe(1);
                    while (base.Crawlpos() != this.Tracked(0))
                    {
                        base.Uncapture();
                    }
                    goto Label_0EA3;

                case 280:
                    this.Trackframe(1);
                    this.Stack(this.Tracked(0));
                    goto Label_0EA3;

                case 0x119:
                    this.Stackframe(1);
                    this.Trackframe(1);
                    this.Stack(this.Tracked(0));
                    goto Label_0EA3;

                case 0x11c:
                    this.Trackframe(2);
                    this.Stack(this.Tracked(0), this.Tracked(1));
                    goto Label_0EA3;

                case 0x11d:
                    this.Trackframe(1);
                    this.Stackframe(2);
                    this.Stack(this.Tracked(0), this.Stacked(1) - 1);
                    goto Label_0EA3;

                default:
                    throw new Exception(RegExRes.GetString(3));
            }
            this.Advance(1);
            goto Label_0007;
        Label_0A06:
            this.Advance(1);
            goto Label_0007;
        Label_0EA3:
            this.Backtrack();
            goto Label_0007;
        }

        private void Goto(int newpos)
        {
            if (newpos < this.runcodepos)
            {
                base.EnsureStorage();
            }
            this.SetOperator(this.runcodes[newpos]);
            this.runcodepos = newpos;
        }

        protected override void InitTrackCount()
        {
            base.runtrackcount = this.runcode._trackcount;
        }

        private int Leftchars()
        {
            return (base.runtextpos - base.runtextbeg);
        }

        private int Operand(int i)
        {
            return this.runcodes[(this.runcodepos + i) + 1];
        }

        private int Operator()
        {
            return this.runoperator;
        }

        private bool Refmatch(int index, int len)
        {
            int runtextpos;
            if (!this.runrtl)
            {
                if ((base.runtextend - base.runtextpos) < len)
                {
                    return false;
                }
                runtextpos = base.runtextpos + len;
            }
            else
            {
                if ((base.runtextpos - base.runtextbeg) < len)
                {
                    return false;
                }
                runtextpos = base.runtextpos;
            }
            int num3 = index + len;
            int num = len;
            if (this.runci)
            {
                while (num-- != 0)
                {
                    if (char.ToLower(base.runtext[--num3], this.runculture) != char.ToLower(base.runtext[--runtextpos], this.runculture))
                    {
                        return false;
                    }
                }
            }
            else
            {
                while (num-- != 0)
                {
                    if (base.runtext[--num3] != base.runtext[--runtextpos])
                    {
                        return false;
                    }
                }
            }
            if (!this.runrtl)
            {
                runtextpos += len;
            }
            base.runtextpos = runtextpos;
            return true;
        }

        private int Rightchars()
        {
            return (base.runtextend - base.runtextpos);
        }

        private void SetOperator(int op)
        {
            this.runci = 0 != (op & 0x200);
            this.runrtl = 0 != (op & 0x40);
            this.runoperator = op & -577;
        }

        private void Stack(int I1)
        {
            base.runstack[--base.runstackpos] = I1;
        }

        private void Stack(int I1, int I2)
        {
            base.runstack[--base.runstackpos] = I1;
            base.runstack[--base.runstackpos] = I2;
        }

        private int Stacked(int i)
        {
            return base.runstack[(base.runstackpos - i) - 1];
        }

        private void Stackframe(int framesize)
        {
            base.runstackpos += framesize;
        }

        private bool Stringmatch(string str)
        {
            int num;
            int runtextpos;
            if (!this.runrtl)
            {
                if ((base.runtextend - base.runtextpos) < (num = str.Length))
                {
                    return false;
                }
                runtextpos = base.runtextpos + num;
            }
            else
            {
                if ((base.runtextpos - base.runtextbeg) < (num = str.Length))
                {
                    return false;
                }
                runtextpos = base.runtextpos;
            }
            if (this.runci)
            {
                while (num != 0)
                {
                    if (str[--num] != char.ToLower(base.runtext[--runtextpos], this.runculture))
                    {
                        return false;
                    }
                }
            }
            else
            {
                while (num != 0)
                {
                    if (str[--num] != base.runtext[--runtextpos])
                    {
                        return false;
                    }
                }
            }
            if (!this.runrtl)
            {
                runtextpos += str.Length;
            }
            base.runtextpos = runtextpos;
            return true;
        }

        private int Textpos()
        {
            return base.runtextpos;
        }

        private int Textstart()
        {
            return base.runtextstart;
        }

        private void Textto(int newpos)
        {
            base.runtextpos = newpos;
        }

        private void Track()
        {
            base.runtrack[--base.runtrackpos] = this.runcodepos;
        }

        private void Track(int I1)
        {
            base.runtrack[--base.runtrackpos] = I1;
            base.runtrack[--base.runtrackpos] = this.runcodepos;
        }

        private void Track(int I1, int I2)
        {
            base.runtrack[--base.runtrackpos] = I1;
            base.runtrack[--base.runtrackpos] = I2;
            base.runtrack[--base.runtrackpos] = this.runcodepos;
        }

        private void Track(int I1, int I2, int I3)
        {
            base.runtrack[--base.runtrackpos] = I1;
            base.runtrack[--base.runtrackpos] = I2;
            base.runtrack[--base.runtrackpos] = I3;
            base.runtrack[--base.runtrackpos] = this.runcodepos;
        }

        private void Track2(int I1)
        {
            base.runtrack[--base.runtrackpos] = I1;
            base.runtrack[--base.runtrackpos] = -this.runcodepos;
        }

        private void Track2(int I1, int I2)
        {
            base.runtrack[--base.runtrackpos] = I1;
            base.runtrack[--base.runtrackpos] = I2;
            base.runtrack[--base.runtrackpos] = -this.runcodepos;
        }

        private int Tracked(int i)
        {
            return base.runtrack[(base.runtrackpos - i) - 1];
        }

        private void Trackframe(int framesize)
        {
            base.runtrackpos += framesize;
        }

        private int Trackpos()
        {
            return (base.runtrack.Length - base.runtrackpos);
        }

        private void Trackto(int newpos)
        {
            base.runtrackpos = base.runtrack.Length - newpos;
        }
    }
}

