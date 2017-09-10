using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class CLSCompliantAttribute : Attribute
    {
        private bool m_compliant;

        public CLSCompliantAttribute(bool isCompliant)
        {
            this.m_compliant = isCompliant;
        }

        public bool IsCompliant
        {
            get
            {
                return this.m_compliant;
            }
        }
    }

 

}
