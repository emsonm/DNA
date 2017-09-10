using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    //NotImplemented,Obsolete,MonoTODO

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class NotImplementedAttribute : Attribute
    {
        readonly string reason;
        public NotImplementedAttribute()
        {
            reason = String.Empty;
        }

        public NotImplementedAttribute(string reason)
        {
            this.reason = reason;
        }

        public string Reason
        {
            get { return reason; }
        }

    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ObsoleteAttribute : Attribute
    {
        readonly string reason;
        public ObsoleteAttribute()
        {
            reason = String.Empty;
        }

        public ObsoleteAttribute(string reason)
        {
            this.reason = reason;
        }

        public string Reason
        {
            get { return reason; }
        }

    }


    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class MonoTODOAttribute : Attribute
    {
        readonly string reason;
        public MonoTODOAttribute()
        {
            reason = String.Empty;
        }

        public MonoTODOAttribute(string reason)
        {
            this.reason = reason;
        }

        public string Reason
        {
            get { return reason; }
        }

    }
}
