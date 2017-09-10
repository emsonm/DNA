using System;
using System.Collections.Generic;
using System.Text;

namespace System.Runtime.InteropServices
{
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited=false)
    //, ComVisible(true)
    ]
public sealed class StructLayoutAttribute : Attribute
{
    // Fields
    internal LayoutKind _val;
    public CharSet CharSet;
    private const int DEFAULT_PACKING_SIZE = 8;
    public int Pack;
    public int Size;

    // Methods
    public StructLayoutAttribute(short layoutKind)
    {
        this._val = (LayoutKind) layoutKind;
    }

    public StructLayoutAttribute(LayoutKind layoutKind)
    {
        this._val = layoutKind;
    }

    internal StructLayoutAttribute(LayoutKind layoutKind, int pack, int size, CharSet charSet)
    {
        this._val = layoutKind;
        this.Pack = pack;
        this.Size = size;
        this.CharSet = charSet;
    }

  //  [SecurityCritical]
    //internal static Attribute GetCustomAttribute(RuntimeType type)
    //{
    //    if (!IsDefined(type))
    //    {
    //        return null;
    //    }
    //    int packSize = 0;
    //    int classSize = 0;
    //    LayoutKind auto = LayoutKind.Auto;
    //    switch ((type.Attributes & TypeAttributes.LayoutMask))
    //    {
    //        case TypeAttributes.AutoLayout:
    //            auto = LayoutKind.Auto;
    //            break;

    //        case TypeAttributes.SequentialLayout:
    //            auto = LayoutKind.Sequential;
    //            break;

    //        case TypeAttributes.ExplicitLayout:
    //            auto = LayoutKind.Explicit;
    //            break;
    //    }
    //    CharSet none = CharSet.None;
    //    TypeAttributes attributes2 = type.Attributes & TypeAttributes.CustomFormatClass;
    //    if (attributes2 == TypeAttributes.AutoLayout)
    //    {
    //        none = CharSet.Ansi;
    //    }
    //    else if (attributes2 == TypeAttributes.UnicodeClass)
    //    {
    //        none = CharSet.Unicode;
    //    }
    //    else if (attributes2 == TypeAttributes.AutoClass)
    //    {
    //        none = CharSet.Auto;
    //    }
    //    type.GetRuntimeModule().MetadataImport.GetClassLayout(type.MetadataToken, out packSize, out classSize);
    //    if (packSize == 0)
    //    {
    //        packSize = 8;
    //    }
    //    return new StructLayoutAttribute(auto, packSize, classSize, none);
    //}

    //internal static bool IsDefined(RuntimeType type)
    //{
    //    return ((!type.IsInterface && !type.HasElementType) && !type.IsGenericParameter);
    //}

    // Properties
    public LayoutKind Value
    {
        get
        {
            return this._val;
        }
    }
}

 

}
