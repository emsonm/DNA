using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public interface ISite : IServiceProvider
    {
        // Properties
        IComponent Component { get; }
        IContainer Container { get; }
        bool DesignMode { get; }
        string Name { get; set; }
    }

 

 

}
