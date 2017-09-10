namespace System.Text.RegularExpressions
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;

    internal sealed class RegexCharClass
    {
        internal bool _canonical;
        internal StringBuilder _categories;
        internal static System.Collections.Generic.Dictionary<string,object> _definedCategories = new System.Collections.Generic.Dictionary<string,object>(0x1f);
        private static readonly LC[] _lcTable = new LC[] { 
            new LC('A', 'Z', 1, 0x20), new LC('\x00c0', '\x00de', 1, 0x20), new LC('Ā', 'Į', 2, 0), new LC('İ', 'İ', 0, 0x69), new LC('Ĳ', 'Ķ', 2, 0), new LC('Ĺ', 'Ň', 3, 0), new LC('Ŋ', 'Ŷ', 2, 0), new LC('Ÿ', 'Ÿ', 0, 0xff), new LC('Ź', 'Ž', 3, 0), new LC('Ɓ', 'Ɓ', 0, 0x253), new LC('Ƃ', 'Ƅ', 2, 0), new LC('Ɔ', 'Ɔ', 0, 0x254), new LC('Ƈ', 'Ƈ', 0, 0x188), new LC('Ɖ', 'Ɗ', 1, 0xcd), new LC('Ƌ', 'Ƌ', 0, 0x18c), new LC('Ǝ', 'Ə', 1, 0xca), 
            new LC('Ɛ', 'Ɛ', 0, 0x25b), new LC('Ƒ', 'Ƒ', 0, 0x192), new LC('Ɠ', 'Ɠ', 0, 0x260), new LC('Ɣ', 'Ɣ', 0, 0x263), new LC('Ɩ', 'Ɩ', 0, 0x269), new LC('Ɨ', 'Ɨ', 0, 0x268), new LC('Ƙ', 'Ƙ', 0, 0x199), new LC('Ɯ', 'Ɯ', 0, 0x26f), new LC('Ɲ', 'Ɲ', 0, 0x272), new LC('Ơ', 'Ƥ', 2, 0), new LC('Ƨ', 'Ƨ', 0, 0x1a8), new LC('Ʃ', 'Ʃ', 0, 0x283), new LC('Ƭ', 'Ƭ', 0, 0x1ad), new LC('Ʈ', 'Ʈ', 0, 0x288), new LC('Ư', 'Ư', 0, 0x1b0), new LC('Ʊ', 'Ʋ', 1, 0xd9), 
            new LC('Ƴ', 'Ƶ', 3, 0), new LC('Ʒ', 'Ʒ', 0, 0x292), new LC('Ƹ', 'Ƹ', 0, 0x1b9), new LC('Ƽ', 'Ƽ', 0, 0x1bd), new LC('Ǆ', 'ǅ', 0, 0x1c6), new LC('Ǉ', 'ǈ', 0, 0x1c9), new LC('Ǌ', 'ǋ', 0, 460), new LC('Ǎ', 'Ǜ', 3, 0), new LC('Ǟ', 'Ǯ', 2, 0), new LC('Ǳ', 'ǲ', 0, 0x1f3), new LC('Ǵ', 'Ǵ', 0, 0x1f5), new LC('Ǻ', 'Ȗ', 2, 0), new LC('Ά', 'Ά', 0, 940), new LC('Έ', 'Ί', 1, 0x25), new LC('Ό', 'Ό', 0, 0x3cc), new LC('Ύ', 'Ώ', 1, 0x3f), 
            new LC('Α', 'Ϋ', 1, 0x20), new LC('Ϣ', 'Ϯ', 2, 0), new LC('Ё', 'Џ', 1, 80), new LC('А', 'Я', 1, 0x20), new LC('Ѡ', 'Ҁ', 2, 0), new LC('Ґ', 'Ҿ', 2, 0), new LC('Ӂ', 'Ӄ', 3, 0), new LC('Ӈ', 'Ӈ', 0, 0x4c8), new LC('Ӌ', 'Ӌ', 0, 0x4cc), new LC('Ӑ', 'Ӫ', 2, 0), new LC('Ӯ', 'Ӵ', 2, 0), new LC('Ӹ', 'Ӹ', 0, 0x4f9), new LC('Ա', 'Ֆ', 1, 0x30), new LC('Ⴀ', 'Ⴥ', 1, 0x30), new LC('Ḁ', 'Ỹ', 2, 0), new LC('Ἀ', 'Ἇ', 1, -8), 
            new LC('Ἐ', '἟', 1, -8), new LC('Ἠ', 'Ἧ', 1, -8), new LC('Ἰ', 'Ἷ', 1, -8), new LC('Ὀ', 'Ὅ', 1, -8), new LC('Ὑ', 'Ὑ', 0, 0x1f51), new LC('Ὓ', 'Ὓ', 0, 0x1f53), new LC('Ὕ', 'Ὕ', 0, 0x1f55), new LC('Ὗ', 'Ὗ', 0, 0x1f57), new LC('Ὠ', 'Ὧ', 1, -8), new LC('ᾈ', 'ᾏ', 1, -8), new LC('ᾘ', 'ᾟ', 1, -8), new LC('ᾨ', 'ᾯ', 1, -8), new LC('Ᾰ', 'Ᾱ', 1, -8), new LC('Ὰ', 'Ά', 1, -74), new LC('ᾼ', 'ᾼ', 0, 0x1fb3), new LC('Ὲ', 'Ή', 1, -86), 
            new LC('ῌ', 'ῌ', 0, 0x1fc3), new LC('Ῐ', 'Ῑ', 1, -8), new LC('Ὶ', 'Ί', 1, -100), new LC('Ῠ', 'Ῡ', 1, -8), new LC('Ὺ', 'Ύ', 1, -112), new LC('Ῥ', 'Ῥ', 0, 0x1fe5), new LC('Ὸ', 'Ό', 1, -128), new LC('Ὼ', 'Ώ', 1, -126), new LC('ῼ', 'ῼ', 0, 0x1ff3), new LC('Ⅰ', 'Ⅿ', 1, 0x10), new LC('Ⓐ', 'ⓐ', 1, 0x1a), new LC((char)0xff21, (char)0xff3a, 1, 0x20)
         };
        internal bool _negate;
        private static readonly string[][] _propTable = new string[][] { 
            new string[] { "_xmlC", "-/0;A[_`a{\x00aa\x00ab\x00b2\x00b4\x00b5\x00b6\x00b9\x00bb\x00bc\x00bf\x00c0\x00d7\x00d8\x00f7\x00f8ƪƫƻƼƾǄǶǺȘɐʩΆ·Έ΋Ό΍Ύ΢ΣϏϐϗϚϛϜϝϞϟϠϡϢϳЁЍЎѐёѝў҂ҐӀӁӅӇӉӋӍӐӬӮӶӸӺԱ՗աֈ٠٪۰ۺ०॰০ৰ৴৺੦ੰ૦૰୦୰௧௳౦౰೦೰൦൰๐๚໐໚༠༴Ⴀ჆აჷḀẜẠỺἀ἖Ἐ἞ἠ὆Ὀ὎ὐ὘Ὑ὚Ὓ὜Ὕ὞Ὗ὾ᾀ᾵ᾶ᾽ι᾿ῂ῅ῆ῍ῐ῔ῖ῜ῠ῭ῲ῵ῶ´⁰ⁱ⁴⁺ⁿ₊₨₩ℂ℃ℇ℈ℊ℔ℕ℞℠℣ℤ℥Ω℧ℨ℩KℲℳℵ⅓Ↄ①⒜Ⓐ⓫❶➔〇〈〡〪㊀㊊㍲㍵㍶㍷㎅㎊㎍㎑㎙㎟㎩㎪㎭㎮㎰㎴㎹㎺㎿㏀㏁㏂㏃㏆㏇㏈㏉㏘㏙㏞" }, new string[] { "_xmlD", "0:٠٪۰ۺ०॰০ৰ੦ੰ૦૰୦୰௧௰౦౰೦೰൦൰๐๚໐໚༠༪⁰ⁱ⁴⁺₀₊" }, new string[] { "_xmlI", ":;A[_`a{\x00a8\x00a9\x00aa\x00ab\x00af\x00b0\x00b4\x00b6\x00b8\x00b9\x00ba\x00bb\x00c0\x00d7\x00d8\x00f7\x00f8ǶǺȘɐʩʰ˟ˠ˪ʹ͵ͺͻ΄·Έ΋Ό΍Ύ΢ΣϏϐϗϚϛϜϝϞϟϠϡϢϴЁЍЎѐёѝў҂ҐӅӇӉӋӍӐӬӮӶӸӺԱ՗ՙ՚աֈא׫װ׳ءػـًٱڸںڿۀۏې۔ەۖۥۧअऺक़ॢঅ঍এ঑ও঩প঱ল঳শ঺ড়৞য়ৢৰ৲ਅ਋ਏ਑ਓ਩ਪ਱ਲ਴ਵ਷ਸ਺ਖ਼੝ਫ਼੟અઌઍ઎એ઒ઓ઩પ઱લ઴વ઺ૠૡଅ଍ଏ଑ଓ଩ପ଱ଲ଴ଶ଺ଡ଼୞ୟୢஅ஋எ஑ஒ஖ங஛ஜ஝ஞ஠ண஥ந஫மஶஷ஺అ఍ఎ఑ఒ఩పఴవ఺ౠౢಅ಍ಎ಑ಒ಩ಪ಴ವ಺ೞ೟ೠೢഅ഍എ഑ഒഩപഺൠൢกัาิเ็๏๐๚๜ກ຃ຄ຅ງຉຊ຋ຍຎດຘນຠມ຤ລ຦ວຨສຬອຯະັາິຽ຾ເ໅ໜໞ༘༚ཀ཈ཉཪႠ჆აჷᄀᅚᅟᆣᆨᇺḀẜẠỺἀ἖Ἐ἞ἠ὆Ὀ὎ὐ὘Ὑ὚Ὓ὜Ὕ὞Ὗ὾ᾀ᾵ᾶ῅ῆ῔ῖ῜῝῰ῲ῵ῶ῿ⁿ₀₨₩ℂ℃ℇ℈ℊ℔ℕ℞℠℣ℤ℥Ω℧ℨ℩KℲℳℹⒶ⓪ぁゕ゛ゟァ・ーヿㄅㄭㄱ㆏㆒ㆠ㉠㉼㊊㊱㋐㋿㌀㍘㍱㍷㍻㎕㎙㎟㎩㎮㎰㏂㏃㏆㏇㏘㏙㏞一丁龥龦가각힣힤豈" }, new string[] { "_xmlW", "#%&'*,0:<?@[^{|}~\x007f\x00a2\x00ab\x00ac\x00ad\x00ae\x00b7\x00b8\x00bb\x00bc\x00bf\x00c0;Ϳ·Έ՚ՠ։֊־ֿ׀ׁ׃ׄ׳׵،؍؛؜؟ؠ۔ەऽा॰ॱઽાଽାຯະ༄༓༺༾྅྆჻ჼ  ‰‽⁅⁇⁪⁰⁽ⁿ₍₏〈⌫　〄々〇〈〒〔〝〰〱・ー\ud800\ud801\udb7f\udb81\udbff\udc01\udfff\ue001豈﴾﵀︰︳︵﹅﹐﹓﹔﹟﹣﹤﹨﹩﹪﹫﻿＀！＃％＆＇＊，０：＜？＠［＾｛｜｝～｡ｦ" }, new string[] { "IsAlphabeticPresentationForms", "ﬀﭐ" }, new string[] { "IsArabic", "؀܀" }, new string[] { "IsArabicPresentationForms-A", "ﭐ︀" }, new string[] { "IsArabicPresentationForms-B", "ﹰ＀" }, new string[] { "IsArmenian", "԰֐" }, new string[] { "IsArrows", "←∀" }, new string[] { "IsBasicLatin", "\0\x0080" }, new string[] { "IsBengali", "ঀ਀" }, new string[] { "IsBlockElements", "▀■" }, new string[] { "IsBopomofo", "㄀㄰" }, new string[] { "IsBoxDrawing", "─▀" }, new string[] { "IsCJKCompatibility", "㌀㐀" }, 
            new string[] { "IsCJKCompatibilityForms", "︰﹐" }, new string[] { "IsCJKCompatibilityIdeographs", "豈ﬀ" }, new string[] { "IsCJKSymbolsandPunctuation", "　぀" }, new string[] { "IsCJKUnifiedIdeographs", "一ꀀ" }, new string[] { "IsCombiningDiacriticalMarks", "̀Ͱ" }, new string[] { "IsCombiningHalfMarks", "︠︰" }, new string[] { "IsCombiningMarksforSymbols", "⃐℀" }, new string[] { "IsControlPictures", "␀⑀" }, new string[] { "IsCurrencySymbols", "₠⃐" }, new string[] { "IsCyrillic", "ЀԀ" }, new string[] { "IsDevanagari", "ऀঀ" }, new string[] { "IsDingbats", "✀⟀" }, new string[] { "IsEnclosedAlphanumerics", "①─" }, new string[] { "IsEnclosedCJKLettersandMonths", "㈀㌀" }, new string[] { "IsGeneralPunctuation", " ⁰" }, new string[] { "IsGeometricShapes", "■☀" }, 
            new string[] { "IsGeorgian", "Ⴀᄀ" }, new string[] { "IsGreek", "ͰЀ" }, new string[] { "IsGreekExtended", "ἀ " }, new string[] { "IsGujarati", "઀଀" }, new string[] { "IsGurmukhi", "਀઀" }, new string[] { "IsHalfwidthandFullwidthForms", "＀￰" }, new string[] { "IsHangulCompatibilityJamo", "㄰㆐" }, new string[] { "IsHangulJamo", "ᄀሀ" }, new string[] { "IsHangulSyllables", "가힤" }, new string[] { "IsHebrew", "֐؀" }, new string[] { "IsHighPrivateUseSurrogates", "\udb80\udc00" }, new string[] { "IsHighSurrogates", "\ud800\udb80" }, new string[] { "IsHiragana", "぀゠" }, new string[] { "IsIPAExtensions", "ɐʰ" }, new string[] { "IsKanbun", "㆐ㆠ" }, new string[] { "IsKannada", "ಀഀ" }, 
            new string[] { "IsKatakana", "゠㄀" }, new string[] { "IsLao", "຀ༀ" }, new string[] { "IsLatin-1Supplement", "\x0080Ā" }, new string[] { "IsLatinExtended-A", "Āƀ" }, new string[] { "IsLatinExtendedAdditional", "Ḁἀ" }, new string[] { "IsLatinExtended-B", "ƀɐ" }, new string[] { "IsLetterlikeSymbols", "℀⅐" }, new string[] { "IsLowSurrogates", "\udc00\ue000" }, new string[] { "IsMalayalam", "ഀ඀" }, new string[] { "IsMathematicalOperators", "∀⌀" }, new string[] { "IsMiscellaneousSymbols", "☀✀" }, new string[] { "IsMiscellaneousTechnical", "⌀␀" }, new string[] { "IsNumberForms", "⅐←" }, new string[] { "IsOpticalCharacterRecognition", "⑀①" }, new string[] { "IsOriya", "଀஀" }, new string[] { "IsPrivateUse", "豈" }, 
            new string[] { "IsSmallFormVariants", "﹐ﹰ" }, new string[] { "IsSpacingModifierLetters", "ʰ̀" }, new string[] { "IsSuperscriptsandSubscripts", "⁰₠" }, new string[] { "IsTamil", "஀ఀ" }, new string[] { "IsTelugu", "ఀಀ" }, new string[] { "IsThai", "฀຀" }, new string[] { "IsTibetan", "ༀ࿀" }
         };
        internal ArrayList _rangelist;
        internal const string Any = "\0";
        internal static readonly RegexCharClass AnyClass = new RegexCharClass("\0");
        internal const string ECMADigit = "0:";
        internal const string ECMASpace = "\t\x000e !";
        internal const string ECMAWord = "0:A[_`a{İı";
        internal const string Empty = "";
        internal static readonly RegexCharClass EmptyClass = new RegexCharClass(string.Empty);
        internal const char GroupChar = '\0';
        internal const char Lastchar = '￿';
        internal const int LowercaseAdd = 1;
        internal const int LowercaseBad = 3;
        internal const int LowercaseBor = 2;
        internal const int LowercaseSet = 0;
        internal const string NotECMADigit = "\00:";
        internal const string NotECMASpace = "\0\t\x000e !";
        internal const string NotECMAWord = "\00:A[_`a{İı";
        internal static readonly string NotSpace = NegateCategory(Space);
        internal const short NotSpaceConst = -100;
        internal static readonly string NotWord;
        internal const char Nullchar = '\0';
        internal static readonly string Space = 'd'.ToString();
        internal const short SpaceConst = 100;
        internal static readonly string Word;

        static RegexCharClass()
        {
            char[] chArray = new char[9];
            StringBuilder builder = new StringBuilder(11);
            builder.Append('\0');
            chArray[0] = '\0';
            chArray[1] = '\x000f';
            _definedCategories["Cc"] = chArray[1].ToString();
            chArray[2] = '\x0010';
            _definedCategories["Cf"] = chArray[2].ToString();
            chArray[3] = '\x001e';
            _definedCategories["Cn"] = chArray[3].ToString();
            chArray[4] = '\x0012';
            _definedCategories["Co"] = chArray[4].ToString();
            chArray[5] = '\x0011';
            _definedCategories["Cs"] = chArray[5].ToString();
            chArray[6] = '\0';
            _definedCategories["C"] = new string(chArray, 0, 7);
            chArray[1] = '\x0002';
            _definedCategories["Ll"] = chArray[1].ToString();
            chArray[2] = '\x0004';
            _definedCategories["Lm"] = chArray[2].ToString();
            chArray[3] = '\x0005';
            _definedCategories["Lo"] = chArray[3].ToString();
            chArray[4] = '\x0003';
            _definedCategories["Lt"] = chArray[4].ToString();
            chArray[5] = '\x0001';
            _definedCategories["Lu"] = chArray[5].ToString();
            _definedCategories["L"] = new string(chArray, 0, 7);
            builder.Append(new string(chArray, 1, 5));
            chArray[1] = '\a';
            _definedCategories["Mc"] = chArray[1].ToString();
            chArray[2] = '\b';
            _definedCategories["Me"] = chArray[2].ToString();
            chArray[3] = '\x0006';
            _definedCategories["Mn"] = chArray[3].ToString();
            chArray[4] = '\0';
            _definedCategories["M"] = new string(chArray, 0, 5);
            builder.Append(chArray[1]);
            builder.Append(chArray[3]);
            chArray[1] = '\t';
            _definedCategories["Nd"] = chArray[1].ToString();
            chArray[2] = '\n';
            _definedCategories["Nl"] = chArray[2].ToString();
            chArray[3] = '\v';
            _definedCategories["No"] = chArray[3].ToString();
            _definedCategories["N"] = new string(chArray, 0, 5);
            builder.Append(new string(chArray, 1, 3));
            chArray[1] = '\x0013';
            _definedCategories["Pc"] = chArray[1].ToString();
            chArray[2] = '\x0014';
            _definedCategories["Pd"] = chArray[2].ToString();
            chArray[3] = '\x0016';
            _definedCategories["Pe"] = chArray[3].ToString();
            chArray[4] = '\x0019';
            _definedCategories["Po"] = chArray[4].ToString();
            chArray[5] = '\x0015';
            _definedCategories["Ps"] = chArray[5].ToString();
            chArray[6] = '\x0018';
            _definedCategories["Pi"] = chArray[6].ToString();
            chArray[7] = '\x0017';
            _definedCategories["Pf"] = chArray[7].ToString();
            chArray[8] = '\0';
            _definedCategories["P"] = new string(chArray, 0, 9);
            builder.Append(chArray[1]);
            chArray[1] = '\x001b';
            _definedCategories["Sc"] = chArray[1].ToString();
            chArray[2] = '\x001c';
            _definedCategories["Sk"] = chArray[2].ToString();
            chArray[3] = '\x001a';
            _definedCategories["Sm"] = chArray[3].ToString();
            chArray[4] = '\x001d';
            _definedCategories["So"] = chArray[4].ToString();
            chArray[5] = '\0';
            _definedCategories["S"] = new string(chArray, 0, 6);
            chArray[1] = '\r';
            _definedCategories["Zl"] = chArray[1].ToString();
            chArray[2] = '\x000e';
            _definedCategories["Zp"] = chArray[2].ToString();
            chArray[3] = '\f';
            _definedCategories["Zs"] = chArray[3].ToString();
            chArray[4] = '\0';
            _definedCategories["Z"] = new string(chArray, 0, 5);
            builder.Append('\0');
            Word = builder.ToString();
            NotWord = NegateCategory(Word);
        }

        internal RegexCharClass()
        {
            this._rangelist = new ArrayList();
            this._canonical = true;
            this._categories = new StringBuilder();
        }

        internal RegexCharClass(string set)
        {
            this._rangelist = new ArrayList((set.Length + 1) / 2);
            this._canonical = true;
            this._categories = new StringBuilder();
            this.AddSet(set);
        }

        internal RegexCharClass(char first, char last)
        {
            this._rangelist = new ArrayList(1);
            this._rangelist.Add(new SingleRange(first, last));
            this._canonical = true;
            this._categories = new StringBuilder();
        }

        internal void AddCategory(string category)
        {
            this._categories.Append(category);
        }

        internal void AddCategoryFromName(string categoryName, bool invert, bool caseInsensitive, string pattern)
        {
            object obj2 = _definedCategories[categoryName];
            if (obj2 != null)
            {
                string category = (string) obj2;
                if (caseInsensitive && (categoryName.Equals("Lu") || categoryName.Equals("Lt")))
                {
                    category = (string) _definedCategories["Ll"];
                }
                if (invert)
                {
                    category = NegateCategory(category);
                }
                this._categories.Append(category);
            }
            else
            {
                this.AddSet(SetFromProperty(categoryName, invert, pattern));
            }
        }

        internal void AddCharClass(RegexCharClass cc)
        {
            if ((this._canonical && (this.RangeCount() > 0)) && ((cc.RangeCount() > 0) && (cc.Range(cc.RangeCount() - 1)._last <= this.Range(this.RangeCount() - 1)._last)))
            {
                this._canonical = false;
            }
            for (int i = 0; i < cc.RangeCount(); i++)
            {
                this._rangelist.Add(cc.Range(i));
            }
            this._categories.Append(cc._categories.ToString());
        }

        internal void AddLowercase(CultureInfo culture)
        {
            this._canonical = false;
            int num = 0;
            int count = this._rangelist.Count;
            while (num < count)
            {
                SingleRange range = (SingleRange) this._rangelist[num];
                if (range._first == range._last)
                {
                    range._first = range._last = char.ToLower(range._first, culture);
                }
                else
                {
                    this.AddLowercaseImpl(range._first, range._last, culture);
                }
                num++;
            }
        }

        internal void AddLowercaseImpl(char chMin, char chMax, CultureInfo culture)
        {
            char ch;
            char ch2;
            LC lc;
            if (chMin == chMax)
            {
                chMin = char.ToLower(chMin, culture);
                if (chMin != chMax)
                {
                    this.AddRange(chMin, chMin);
                }
                return;
            }
            int index = 0;
            int length = _lcTable.Length;
            while (index < length)
            {
                int num3 = (index + length) / 2;
                if (_lcTable[num3]._chMax < chMin)
                {
                    index = num3 + 1;
                }
                else
                {
                    length = num3;
                }
            }
            if (index < _lcTable.Length)
            {
                goto Label_00FD;
            }
            return;
        Label_00E7:
            if ((ch < chMin) || (ch2 > chMax))
            {
                this.AddRange(ch, ch2);
            }
            index++;
        Label_00FD:
            if ((index < _lcTable.Length) && ((lc = _lcTable[index])._chMin <= chMax))
            {
                if ((ch = lc._chMin) < chMin)
                {
                    ch = chMin;
                }
                if ((ch2 = lc._chMax) > chMax)
                {
                    ch2 = chMax;
                }
                switch (lc._lcOp)
                {
                    case 0:
                        ch = (char) lc._data;
                        ch2 = (char) lc._data;
                        goto Label_00E7;

                    case 1:
                        ch = (char) (ch + ((char) lc._data));
                        ch2 = (char) (ch2 + ((char) lc._data));
                        goto Label_00E7;

                    case 2:
                        ch = (char) (ch | '\x0001');
                        ch2 = (char) (ch2 | '\x0001');
                        goto Label_00E7;

                    case 3:
                        ch = (char) (ch + ((char) (ch & '\x0001')));
                        ch2 = (char) (ch2 + ((char) (ch2 & '\x0001')));
                        goto Label_00E7;
                }
                goto Label_00E7;
            }
        }

        internal void AddRange(char first, char last)
        {
            this._rangelist.Add(new SingleRange(first, last));
            if ((this._canonical && (this._rangelist.Count > 0)) && (first <= ((SingleRange) this._rangelist[this._rangelist.Count - 1])._last))
            {
                this._canonical = false;
            }
        }

        internal void AddSet(string set)
        {
            if ((this._canonical && (this.RangeCount() > 0)) && ((set.Length > 0) && (set[0] <= this.Range(this.RangeCount() - 1)._last)))
            {
                this._canonical = false;
            }
            int num = 0;
            while (num < (set.Length - 1))
            {
                this._rangelist.Add(new SingleRange(set[num], (char) (set[num + 1] - '\x0001')));
                num += 2;
            }
            if (num < set.Length)
            {
                this._rangelist.Add(new SingleRange(set[num], (char)0xffff));
            }
        }

        private void Canonicalize()
        {
            char ch;
            this._canonical = true;
            this._rangelist.Sort(0, this._rangelist.Count, new SingleRangeComparer());
            if (this._rangelist.Count <= 1)
            {
                return;
            }
            bool flag = false;
            int num = 1;
            int index = 0;
        Label_003B:
            ch = ((SingleRange) this._rangelist[index])._last;
        Label_0052:
            if ((num == this._rangelist.Count) || (ch == 0xffff))
            {
                flag = true;
            }
            else
            {
                SingleRange range;
                if ((range = (SingleRange) this._rangelist[num])._first <= (ch + '\x0001'))
                {
                    if (ch < range._last)
                    {
                        ch = range._last;
                    }
                    num++;
                    goto Label_0052;
                }
            }
            ((SingleRange) this._rangelist[index])._last = ch;
            index++;
            if (!flag)
            {
                if (index < num)
                {
                    this._rangelist[index] = this._rangelist[num];
                }
                num++;
                goto Label_003B;
            }
            this._rangelist.RemoveRange(index, this._rangelist.Count - index);
        }

        internal static string CategoryUnion(string catI, string catJ)
        {
            return (catI + catJ);
        }

        internal static bool CharInCategory(char ch, string category)
        {
            if (category.Length != 0)
            {
                UnicodeCategory unicodeCategory = char.GetUnicodeCategory(ch);
                int i = 0;
                while (i < category.Length)
                {
                    int num2 = (short) category[i];
                    if (num2 == 0)
                    {
                        if (CharInCategoryGroup(ch, unicodeCategory, category, ref i))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (num2 > 0)
                        {
                            if (num2 != 100)
                            {
                                num2--;
                                if ((int)unicodeCategory == num2)
                                {
                                    return true;
                                }
                                goto Label_0074;
                            }
                            if (char.IsWhiteSpace(ch))
                            {
                                return true;
                            }
                            i++;
                            continue;
                        }
                        if (num2 == -100)
                        {
                            if (!char.IsWhiteSpace(ch))
                            {
                                return true;
                            }
                            i++;
                            continue;
                        }
                        num2 = -num2;
                        num2--;
                        if ((int)unicodeCategory != num2)
                        {
                            return true;
                        }
                    }
                Label_0074:
                    i++;
                }
            }
            return false;
        }

        private static bool CharInCategoryGroup(char ch, UnicodeCategory chcategory, string category, ref int i)
        {
            i++;
            int num = (short) category[i];
            if (num > 0)
            {
                bool flag = false;
                while (num != 0)
                {
                    if (!flag)
                    {
                        num--;
                        if ((int)chcategory == num)
                        {
                            flag = true;
                        }
                    }
                    i++;
                    num = (short) category[i];
                }
                return flag;
            }
            bool flag2 = true;
            while (num != 0)
            {
                if (flag2)
                {
                    num = -num;
                    num--;
                    if ((int)chcategory == num)
                    {
                        flag2 = false;
                    }
                }
                i++;
                num = (short) category[i];
            }
            return flag2;
        }

        internal static bool CharInSet(char ch, string set, string category)
        {
            bool flag = CharInSetInternal(ch, set, category);
            if (((set.Length >= 2) && (set[0] == '\0')) && (set[1] == '\0'))
            {
                return !flag;
            }
            return flag;
        }

        internal static bool CharInSetInternal(char ch, string set, string category)
        {
            int num = 0;
            int length = set.Length;
            while (num != length)
            {
                int num3 = (num + length) / 2;
                if (ch < set[num3])
                {
                    length = num3;
                }
                else
                {
                    num = num3 + 1;
                }
            }
            return (((num & 1) != 0) || CharInCategory(ch, category));
        }

        internal static RegexCharClass CreateFromCategory(string categoryName, bool invert, bool caseInsensitive, string pattern)
        {
            RegexCharClass class2 = new RegexCharClass();
            class2.AddCategoryFromName(categoryName, invert, caseInsensitive, pattern);
            return class2;
        }

        internal static bool IsECMAWordChar(char ch)
        {
            return CharInSet(ch, "0:A[_`a{İı", string.Empty);
        }

        internal static bool IsSingleton(string set)
        {
            return ((set.Length == 2) && (set[0] == (set[1] - '\x0001')));
        }

        internal static bool IsWordChar(char ch)
        {
            return CharInCategory(ch, Word);
        }

        internal static string NegateCategory(string category)
        {
            if (category == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < category.Length; i++)
            {
                short num2 = (short) category[i];
                builder.Append((char) ((ushort) -num2));
            }
            return builder.ToString();
        }

        private SingleRange Range(int i)
        {
            return (SingleRange) this._rangelist[i];
        }

        private int RangeCount()
        {
            return this._rangelist.Count;
        }

        internal static string SetFromChar(char ch)
        {
            StringBuilder builder = new StringBuilder(2);
            builder.Append(ch);
            if (ch != 0xffff)
            {
                builder.Append((char) (ch + '\x0001'));
            }
            return builder.ToString();
        }

        internal static string SetFromProperty(string capname, bool invert, string pattern)
        {
            int num = 0;
            int length = _propTable.GetLength(0);
            while (num != length)
            {
                int index = (num + length) / 2;
                //HACK:wzrd0
                int num4 = string.Compare(capname, _propTable[index][0]);//, false, CultureInfo.InvariantCulture);
                if (num4 < 0)
                {
                    length = index;
                }
                else
                {
                    if (num4 > 0)
                    {
                        num = index + 1;
                        continue;
                    }
                    string set = _propTable[index][1];
                    if (!invert)
                    {
                        return set;
                    }
                    return SetInverse(set);
                }
            }
            throw new ArgumentException();
        }

        internal static string SetInverse(string set)
        {
            if ((set.Length == 0) || (set[0] != '\0'))
            {
                return ("\0" + set);
            }
            if (set.Length == 1)
            {
                return "";
            }
            return set.Substring(1, set.Length - 1);
        }

        internal static string SetInverseFromChar(char ch)
        {
            StringBuilder builder = new StringBuilder(3);
            if (ch != '\0')
            {
                builder.Append('\0');
                builder.Append(ch);
            }
            if (ch != 0xffff)
            {
                builder.Append((char) (ch + '\x0001'));
            }
            return builder.ToString();
        }

        internal static int SetSize(string set)
        {
            int num2 = 0;
            int num = 0;
            while (num < (set.Length - 1))
            {
                num2 += set[num + 1] - set[num];
                num += 2;
            }
            if (num < set.Length)
            {
                num2 += 0x10000 - set[num];
            }
            return num2;
        }

        internal static string SetUnion(string setI, string setJ)
        {
            int num3;
            string str;
            if (setI.Equals("") || setJ.Equals("\0"))
            {
                return setJ;
            }
            if (setJ.Equals("") || setI.Equals("\0"))
            {
                return setI;
            }
            if (setI == setJ)
            {
                return setI;
            }
            int startIndex = 0;
            int num2 = 0;
            StringBuilder builder = new StringBuilder(setI.Length + setJ.Length);
        Label_005B:
            if (num2 == setJ.Length)
            {
                builder.Append(setI, startIndex, setI.Length - startIndex);
                goto Label_0136;
            }
            if (startIndex == setI.Length)
            {
                builder.Append(setJ, num2, setJ.Length - num2);
                goto Label_0136;
            }
            if (setJ[num2] > setI[startIndex])
            {
                num3 = startIndex;
                startIndex = num2;
                num2 = num3;
                str = setI;
                setI = setJ;
                setJ = str;
            }
            builder.Append(setJ[num2++]);
            if (num2 == setJ.Length)
            {
                goto Label_0136;
            }
            char ch = setJ[num2++];
        Label_00E8:
            while ((startIndex < setI.Length) && (setI[startIndex] <= ch))
            {
                startIndex++;
            }
            if ((startIndex & 1) == 0)
            {
                builder.Append(ch);
                goto Label_005B;
            }
            if (startIndex != setI.Length)
            {
                ch = setI[startIndex++];
                num3 = startIndex;
                startIndex = num2;
                num2 = num3;
                str = setI;
                setI = setJ;
                setJ = str;
                goto Label_00E8;
            }
        Label_0136:
            return builder.ToString();
        }

        internal static char SingletonChar(string set)
        {
            return set[0];
        }

        internal string ToSet()
        {
            StringBuilder builder;
            if (!this._canonical)
            {
                this.Canonicalize();
            }
            if (this._negate)
            {
                builder = new StringBuilder((this._rangelist.Count * 2) + 2);
                builder.Append('\0');
                builder.Append('\0');
            }
            else
            {
                builder = new StringBuilder(this._rangelist.Count * 2);
            }
            for (int i = 0; i < this._rangelist.Count; i++)
            {
                builder.Append(((SingleRange) this._rangelist[i])._first);
                if (((SingleRange) this._rangelist[i])._last != 0xffff)
                {
                    builder.Append((char) (((SingleRange) this._rangelist[i])._last + '\x0001'));
                }
            }
            return builder.ToString();
        }

        internal string ToSetCi(bool caseInsensitive, CultureInfo culture)
        {
            if (caseInsensitive)
            {
                this.AddLowercase(culture);
            }
            return this.ToSet();
        }

        internal string Category
        {
            get
            {
                return this._categories.ToString();
            }
        }

        internal bool Negate
        {
            get
            {
                return this._negate;
            }
            set
            {
                this._negate = value;
            }
        }

        private sealed class LC
        {
            internal char _chMax;
            internal char _chMin;
            internal int _data;
            internal int _lcOp;

            internal LC(char chMin, char chMax, int lcOp, int data)
            {
                this._chMin = chMin;
                this._chMax = chMax;
                this._lcOp = lcOp;
                this._data = data;
            }
        }

        private sealed class SingleRange
        {
            internal char _first;
            internal char _last;

            internal SingleRange(char first, char last)
            {
                this._first = first;
                this._last = last;
            }
        }

        private sealed class SingleRangeComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (((RegexCharClass.SingleRange) x)._first < ((RegexCharClass.SingleRange) y)._first)
                {
                    return -1;
                }
                if (((RegexCharClass.SingleRange) x)._first <= ((RegexCharClass.SingleRange) y)._first)
                {
                    return 0;
                }
                return 1;
            }
        }
    }
}

