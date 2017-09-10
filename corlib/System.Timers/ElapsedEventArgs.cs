using System;
using System.Collections.Generic;
using System.Text;

namespace System.Timers
{
    public delegate void ElapsedEventHandler(object sender, ElapsedEventArgs e);

    public class ElapsedEventArgs : EventArgs
    {
        // Fields
        private DateTime time;

        // Methods
        internal ElapsedEventArgs(DateTime time)
        {
            this.time = time;
        }

        // Properties
        public DateTime SignalTime
        {
            get
            {
                return this.time;
            }
        }
    }

 

}
