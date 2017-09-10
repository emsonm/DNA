

namespace System.Text
{
    [Serializable]
    public sealed class EncodingInfo
    {
        readonly int codepage;
        Encoding encoding;

        internal EncodingInfo(int cp)
        {
            codepage = cp;
        }

        public int CodePage
        {
            get { return codepage; }
        }

        [MonoTODO]
        public string DisplayName
        {
            get { return Name; }
        }

        public string Name
        {
            get
            {
                if (encoding == null)
                    encoding = GetEncoding();
                return encoding.WebName;
            }
        }

        public override bool Equals(object value)
        {
            EncodingInfo ei = value as EncodingInfo;
            return ei != null &&
                ei.codepage == codepage;
        }

        public override int GetHashCode()
        {
            return codepage;
        }

        public Encoding GetEncoding()
        {
            return Encoding.GetEncoding(codepage);
        }
    }
}
