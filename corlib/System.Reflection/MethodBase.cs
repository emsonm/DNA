using System;
using System.Collections.Generic;
using System.Text;

namespace System.Reflection
{
    public abstract class MethodBase:MemberInfo
    {
        public override string Name
        {
            get { throw new NotImplementedException(); }
        }
        public abstract ParameterInfo[] GetParameters();

        internal void Invoke(object uninitializedInstance, object[] constructorParams)
        {
            throw new NotImplementedException();
        }
    }
}
