using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)
    //, ComVisible(true)
    ]
    public sealed class SerializableAttribute : Attribute
    {
        // Methods
        //internal static Attribute GetCustomAttribute(RuntimeType type)
        //{
        //    if ((type.Attributes & TypeAttributes.Serializable) != TypeAttributes.Serializable)
        //    {
        //        return null;
        //    }
        //    return new SerializableAttribute();
        //}

        //internal static bool IsDefined(RuntimeType type)
        //{
        //    return type.IsSerializable;
        //}
    }


}
