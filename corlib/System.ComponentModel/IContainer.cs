using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public interface IContainer : IDisposable
    {
        // Methods
        void Add(IComponent component);
        void Add(IComponent component, string name);
        void Remove(IComponent component);

        // Properties
        ComponentCollection Components { get; }
    }

 

 

}
