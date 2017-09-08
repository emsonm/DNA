using System;
using System.Runtime.InteropServices;

namespace PInvokeTest
{
    public static class Simple
    {
        [DllImport("libSimple", CallingConvention=CallingConvention.StdCall, EntryPoint ="_Test@4")]
        extern public static int Test(int value);
    }
}
