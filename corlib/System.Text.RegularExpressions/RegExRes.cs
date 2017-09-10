namespace System.Text.RegularExpressions
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    internal sealed class RegExRes
    {
        internal const int AlternationCantCapture = 0x22;
        internal const int AlternationCantHaveComment = 0x23;
        internal const int BadClassInCharRange = 13;
        internal const int BeginIndexNotNegative = 1;
        internal const int CapnumNotZero = 0x21;
        internal const int CountTooSmall = 0x26;
        internal const int EnumNotStarted = 0x27;
        private static string g_formattingError;
        private static string g_noResources;
        private static string g_resourceNotFound;
        internal const int IllegalCondition = 0x1b;
        internal const int IllegalEndEscape = 0x13;
        internal const int IllegalRange = 11;
        internal const int IncompleteSlashP = 30;
        internal const int InternalError = 10;
        internal const int InvalidGroupName = 0x20;
        internal const int LengthNotNegative = 2;
        internal const int MakeException = 0x1d;
        internal const int MalformedNameRef = 20;
        internal const int MalformedReference = 0x10;
        internal const int MalformedSlashP = 0x1f;
        internal const int MissingControl = 0x18;
        internal const int NestedQuantify = 8;
        internal const int NoResultOnFailed = 5;
        internal const int NotEnoughParens = 12;
        internal const int OnlyAllowedOnce = 0;
        internal const int QuantifyAfterNothing = 9;
        internal const int ReplacementError = 0x25;
        internal const int ReversedCharRange = 14;
        internal const int TooFewHex = 0x17;
        internal const int TooManyAlternates = 0x1c;
        internal const int TooManyParens = 7;
        internal const int UndefinedBackref = 0x15;
        internal const int UndefinedNameRef = 0x16;
        internal const int UndefinedReference = 15;
        internal const int UnexpectedOpcode = 4;
        internal const int UnimplementedState = 3;
        internal const int UnknownProperty = 0x24;
        internal const int UnrecognizedControl = 0x19;
        internal const int UnrecognizedEscape = 0x1a;
        internal const int UnrecognizedGrouping = 0x11;
        internal const int UnterminatedBracket = 6;
        internal const int UnterminatedComment = 0x12;

        static RegExRes()
        {
           
        }

        internal static string GetString(int name)
        {
            return GetString(name, new object[0]);
        }

        internal static string GetString(int name, object a0)
        {
            return GetString(name, new object[] { a0 });
        }

        internal static string GetString(int name, params object[] args)
        {
            return "";
        }

        internal static string GetString(int name, object a0, object a1)
        {
            return GetString(name, new object[] { a0, a1 });
        }

        internal static bool HasString(int name)
        {
            return false;
        }

     
    }
}

