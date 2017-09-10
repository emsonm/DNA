namespace System
{
    using System.Runtime.CompilerServices;

    public delegate void EventHandler(object sender, EventArgs e);
    public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;

 

}

