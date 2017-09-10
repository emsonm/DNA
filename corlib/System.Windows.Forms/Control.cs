using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public class Control : Component,
        //UnsafeNativeMethods.IOleControl, UnsafeNativeMethods.IOleObject, UnsafeNativeMethods.IOleInPlaceObject, UnsafeNativeMethods.IOleInPlaceActiveObject, UnsafeNativeMethods.IOleWindow, UnsafeNativeMethods.IViewObject, UnsafeNativeMethods.IViewObject2, UnsafeNativeMethods.IPersist, UnsafeNativeMethods.IPersistStreamInit, UnsafeNativeMethods.IPersistPropertyBag, UnsafeNativeMethods.IPersistStorage, UnsafeNativeMethods.IQuickActivate,
       // ISupportOleDropSource, IDropTarget, ISynchronizeInvoke, IWin32Window, 
        IArrangedElement,
       // IBindableComponent, 
        IComponent,
        IDisposable
    {
        protected virtual void OnKeyDown(KeyEventArgs e) { }
        protected virtual void OnKeyPress(KeyPressEventArgs e) { }
        protected virtual void OnKeyUp(KeyEventArgs e) { }
        protected virtual void OnPaint(PaintEventArgs e) { }
    }
}
