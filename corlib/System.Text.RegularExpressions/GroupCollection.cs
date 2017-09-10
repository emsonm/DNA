namespace System.Text.RegularExpressions
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class GroupCollection : ICollection, IEnumerable
    {
        internal System.Collections.Generic.Dictionary<object,object> _captureMap;
        internal Group[] _groups;
        internal Match _match;

        internal GroupCollection(Match match, System.Collections.Generic.Dictionary<object,object> caps)
        {
            this._match = match;
            this._captureMap = caps;
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            int index = arrayIndex;
            for (int i = 0; i < this.Count; i++)
            {
                array.SetValue(this[i], index);
                index++;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new GroupEnumerator(this);
        }

        internal Group GetGroup(int groupnum)
        {
            if (this._captureMap != null)
            {
                object obj2 = this._captureMap[groupnum];
                if (obj2 == null)
                {
                    return Group._emptygroup;
                }
                return this.GetGroupImpl((int) obj2);
            }
            if ((groupnum < this._match._matchcount.Length) && (groupnum >= 0))
            {
                return this.GetGroupImpl(groupnum);
            }
            return Group._emptygroup;
        }

        internal Group GetGroupImpl(int groupnum)
        {
            if (groupnum == 0)
            {
                return this._match;
            }
            if (this._groups == null)
            {
                this._groups = new Group[this._match._matchcount.Length - 1];
                for (int i = 0; i < this._groups.Length; i++)
                {
                    this._groups[i] = new Group(this._match._text, this._match._matches[i + 1], this._match._matchcount[i + 1]);
                }
            }
            return this._groups[groupnum - 1];
        }

        public int Count
        {
            get
            {
                return this._match._matchcount.Length;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public Group this[int groupnum]
        {
            get
            {
                return this.GetGroup(groupnum);
            }
        }

        public Group this[string groupname]
        {
            get
            {
                if (this._match._regex == null)
                {
                    return Group._emptygroup;
                }
                return this.GetGroup(this._match._regex.GroupNumberFromName(groupname));
            }
        }

        public object SyncRoot
        {
            get
            {
                return this._match;
            }
        }
    }
}

