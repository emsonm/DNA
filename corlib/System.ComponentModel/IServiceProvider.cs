using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public interface IServiceProvider
    {
        // Methods
        object GetService(Type serviceType);
    }

 

 

}
