#if !LOCALTEST

using System.Reflection;
using System.Runtime.CompilerServices;
namespace System {
	public abstract class Delegate {

		// These must be the same as defined in the interpreter
		// If/when reflection is implemented, this IntPtr may change to MethodInfo
		private object targetObj = null;
		private IntPtr targetMethod = IntPtr.Zero;
		protected Delegate pNext = null;
        private MethodInfo _methodBase;

		public override bool Equals(object obj) {
			Delegate d = obj as Delegate;
			if (d == null) {
				return false;
			}
			return d.targetObj == this.targetObj && d.targetMethod.Equals(this.targetMethod);
		}

		public override int GetHashCode() {
			int ret = targetMethod.GetHashCode();
			if (targetObj != null) {
				ret ^= targetObj.GetHashCode();
			}
			return ret;
		}

		public static Delegate Combine(Delegate a, Delegate b) {
			if (a == null) {
				return b;
			} else if (b == null) {
				return a;
			}

			if (a.GetType() != b.GetType()) {
				throw new ArgumentException("Incompatible delegate types");
			}

			return a.CombineImpl(b);
		}

		protected virtual Delegate CombineImpl(Delegate d) {
			throw new MulticastNotSupportedException();
		}

		public static Delegate Remove(Delegate source, Delegate value) {
			if (source == null) {
				return null;
			}
			return source.RemoveImpl(value);
		}

		protected virtual Delegate RemoveImpl(Delegate d) {
			if (d.Equals(this)) {
				return null;
			}
			return this;
		}

        public MethodInfo Method
        {
            get
            {
                return this.GetMethodImpl();
            }
        }

        protected virtual MethodInfo GetMethodImpl()
        {
            if (this._methodBase == null)
            {
                this._methodBase = new MethodInfo(this.GetMethodNameImpl(), this.GetParameterCountImpl());
                //RuntimeMethodHandle methodHandle = this.FindMethodHandle();
                //RuntimeTypeHandle declaringType = methodHandle.GetDeclaringType();
                //if ((declaringType.IsGenericTypeDefinition() || declaringType.HasInstantiation()) && ((methodHandle.GetAttributes() & MethodAttributes.Static) == MethodAttributes.ReuseSlot))
                //{
                //    if (!(this._methodPtrAux == IntPtr.Zero))
                //    {
                //        declaringType = base.GetType().GetMethod("Invoke").GetParameters()[0].ParameterType.TypeHandle;
                //    }
                //    else
                //    {
                //        Type baseType = this._target.GetType();
                //        Type genericTypeDefinition = declaringType.GetRuntimeType().GetGenericTypeDefinition();
                //        while (true)
                //        {
                //            if (baseType.IsGenericType && (baseType.GetGenericTypeDefinition() == genericTypeDefinition))
                //            {
                //                break;
                //            }
                //            baseType = baseType.BaseType;
                //        }
                //        declaringType = baseType.TypeHandle;
                //    }
                //}
                //this._methodBase = (MethodInfo)RuntimeType.GetMethodBase(declaringType, methodHandle);
            }
            return (MethodInfo)this._methodBase;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern string GetMethodNameImpl();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int GetParameterCountImpl();
      

 
	}
}

#endif