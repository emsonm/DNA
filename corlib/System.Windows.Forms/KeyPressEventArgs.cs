using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    public class KeyPressEventArgs : EventArgs
    {
        // Fields
        private bool handled;
        private char keyChar;

        // Methods
        public KeyPressEventArgs(char keyChar)
        {
            this.keyChar = keyChar;
        }

        // Properties
        public bool Handled
        {
            get
            {
                return this.handled;
            }
            set
            {
                this.handled = value;
            }
        }

        public char KeyChar
        {
            get
            {
                return this.keyChar;
            }
            set
            {
                this.keyChar = value;
            }
        }
    }

}
