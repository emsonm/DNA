using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Reflection
{
    public /*abstract*/ class MethodInfo : MethodBase
    {
        private string name;
        private int numparams;
        public MethodInfo() { }
        public MethodInfo(string name, int numparams) {
            this.name = name;
            this.numparams = numparams;
        }
        public override string Name
        {
            get { return name; }
        }
        public override ParameterInfo[] GetParameters()
        {
            return new ParameterInfo[numparams];
        }
    }
}
