using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    [Flags]
    public enum GenericUriParserOptions
    {
        AllowEmptyAuthority = 2,
        Default = 0,
        DontCompressPath = 128,
        DontConvertPathBackslashes = 64,
        DontUnescapePathDotsAndSlashes = 256,
        GenericAuthority = 1,
        Idn = 512,
        IriParsing = 1024,
        NoFragment = 32,
        NoPort = 8,
        NoQuery = 16,
        NoUserInfo = 4
    }

 

 

    public class GenericUriParser : UriParser
    {
        // Methods
        public GenericUriParser(GenericUriParserOptions options)
        {
        }
    }

 

}
