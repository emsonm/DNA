using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace System.Collections
{
    public sealed class Comparer : IComparer
    {
        public static readonly Comparer Default = new Comparer();
        public static readonly Comparer DefaultInvariant = new Comparer(CultureInfo.InvariantCulture);
      //  private CompareInfo m_compareInfo;

        private Comparer()
        {
        }

        public Comparer(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException();
            }
         //   this.m_compareInfo = culture.CompareInfo;
        }

        public int Compare(object a, object b)
        {
            if (a == b)
            {
                return 0;
            }
            if (a == null)
            {
                return -1;
            }
            if (b == null)
            {
                return 1;
            }
            //if (this.m_compareInfo != null)
            //{
            //    string str = a as string;
            //    string str2 = b as string;
            //    if ((str != null) && (str2 != null))
            //    {
            //        return this.m_compareInfo.Compare(str, str2);
            //    }
            //}
            IComparable comparable = a as IComparable;
            if (comparable != null)
            {
                return comparable.CompareTo(b);
            }
            IComparable comparable2 = b as IComparable;
            if (comparable2 == null)
            {
                throw new ArgumentException();
            }
            return -comparable2.CompareTo(a);
        }
    }

 

}
