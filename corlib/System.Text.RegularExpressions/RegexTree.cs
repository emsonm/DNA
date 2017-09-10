namespace System.Text.RegularExpressions
{
    using System;
    using System.Collections;

    internal sealed class RegexTree
    {
        internal System.Collections.Generic.Dictionary<object,object> _capnames;
        internal object[] _capnumlist;
        internal System.Collections.Generic.Dictionary<object,object> _caps;
        internal string[] _capslist;
        internal int _captop;
        internal RegexOptions _options;
        internal RegexNode _root;

        internal RegexTree(RegexNode root, System.Collections.Generic.Dictionary<object,object> caps, 
            object[] capnumlist, int captop, System.Collections.Generic.Dictionary<object,object> capnames, string[] capslist, RegexOptions opts)
        {
            this._root = root;
            this._caps = caps;
            this._capnumlist = capnumlist;
            this._capnames = capnames;
            this._capslist = capslist;
            this._captop = captop;
            this._options = opts;
        }
    }
}

