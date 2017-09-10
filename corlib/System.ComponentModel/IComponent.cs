using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public interface IComponent : IDisposable
    {
        // Events
        event EventHandler Disposed;

        // Properties
        ISite Site { get; set; }
    }

 

}
