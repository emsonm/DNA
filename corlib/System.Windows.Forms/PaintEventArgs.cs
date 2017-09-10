using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    public class PaintEventArgs : EventArgs, IDisposable
    {
        // Fields
        private readonly Rectangle clipRect;
        private readonly IntPtr dc;
        private Graphics graphics;
        private IntPtr oldPal;
     //   private GraphicsState savedGraphicsState;

        // Methods
        public PaintEventArgs(Graphics graphics, Rectangle clipRect)
        {
            this.dc = IntPtr.Zero;
            this.oldPal = IntPtr.Zero;
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }
            this.graphics = graphics;
            this.clipRect = clipRect;
        }

        internal PaintEventArgs(IntPtr dc, Rectangle clipRect)
        {
            this.dc = IntPtr.Zero;
            this.oldPal = IntPtr.Zero;
            this.dc = dc;
            this.clipRect = clipRect;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ((disposing && (this.graphics != null)) && (this.dc != IntPtr.Zero))
            {
                this.graphics.Dispose();
            }
            //if ((this.oldPal != IntPtr.Zero) && (this.dc != IntPtr.Zero))
            //{
            //    SafeNativeMethods.SelectPalette(new HandleRef(this, this.dc), new HandleRef(this, this.oldPal), 0);
            //    this.oldPal = IntPtr.Zero;
            //}
        }

        ~PaintEventArgs()
        {
            this.Dispose(false);
        }

        internal void ResetGraphics()
        {
            //if ((this.graphics != null) && (this.savedGraphicsState != null))
            //{
            //    this.graphics.Restore(this.savedGraphicsState);
            //    this.savedGraphicsState = null;
            //}
        }

        // Properties
        public Rectangle ClipRectangle
        {
            get
            {
                return this.clipRect;
            }
        }

        public Graphics Graphics
        {
            get
            {
                if ((this.graphics == null) && (this.dc != IntPtr.Zero))
                {
                 //   this.oldPal = Control.SetUpPalette(this.dc, false, false);
                    this.graphics = Graphics.FromHdcInternal(this.dc);
                    this.graphics.PageUnit = GraphicsUnit.Pixel;
                 //   this.savedGraphicsState = this.graphics.Save();
                }
                return this.graphics;
            }
        }

        internal IntPtr HDC
        {
            get
            {
                if (this.graphics == null)
                {
                    return this.dc;
                }
                return IntPtr.Zero;
            }
        }
    }

 

}
