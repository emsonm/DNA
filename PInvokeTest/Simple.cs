using System;
using System.Runtime.InteropServices;

namespace PInvokeTest
{
    public static class Simple
    {
        [DllImport("libSimple", CallingConvention=CallingConvention.Cdecl)]
        extern public static int Test(int value);
    }
}
