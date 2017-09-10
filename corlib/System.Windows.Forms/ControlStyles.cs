using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    [Flags]
    public enum ControlStyles
    {
        AllPaintingInWmPaint = 8192,
        CacheText = 16384,
        ContainerControl = 1,
      //  [EditorBrowsable(EditorBrowsableState.Never)]
        DoubleBuffer = 65536,
        EnableNotifyMessage = 32768,
        FixedHeight = 64,
        FixedWidth = 32,
        Opaque = 4,
        OptimizedDoubleBuffer = 131072,
        ResizeRedraw = 16,
        Selectable = 512,
        StandardClick = 256,
        StandardDoubleClick = 4096,
        SupportsTransparentBackColor = 2048,
        UserMouse = 1024,
        UserPaint = 2,
        UseTextForAccessibility = 262144
    }

 

 

}
