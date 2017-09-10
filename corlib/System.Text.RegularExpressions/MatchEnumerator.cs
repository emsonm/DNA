namespace System.Text.RegularExpressions
{
    using System;
    using System.Collections;

    internal class MatchEnumerator : IEnumerator
    {
        internal int _curindex;
        internal bool _done;
        internal System.Text.RegularExpressions.Match _match = null;
        internal MatchCollection _matchcoll;

        internal MatchEnumerator(MatchCollection matchcoll)
        {
            this._matchcoll = matchcoll;
        }

        public bool MoveNext()
        {
            if (this._done)
            {
                return false;
            }
            this._match = this._matchcoll.GetMatch(this._curindex++);
            if (this._match == null)
            {
                this._done = true;
                return false;
            }
            return true;
        }

        public void Reset()
        {
            this._curindex = 0;
        }

        public object Current
        {
            get
            {
                if (this._match == null)
                {
                    throw new InvalidOperationException();
                }
                return this._match;
            }
        }

        public System.Text.RegularExpressions.Match Match
        {
            get
            {
                return this._match;
            }
        }
    }
}

