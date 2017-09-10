using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Sockets
{
    public class LingerOption
    {
        // Fields
        private bool enabled;
        private int seconds;

        // Methods
        public LingerOption(bool enable, int secs)
        {
            this.enabled = enable;
            this.seconds = secs;
        }

        // Properties
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }

        public int LingerTime
        {
            get
            {
                return this.seconds;
            }
            set
            {
                this.seconds = value;
            }
        }
    }

 

}
