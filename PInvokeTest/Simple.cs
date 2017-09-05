using System;
using System.Runtime.InteropServices;

namespace PInvokeTest
{
    public static class Simple
    {
        [DllImport("libSimple")]
        extern public static int Test(int value);
    }
}
