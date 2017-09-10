using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.ComponentModel
{
    public class ComponentCollection : ReadOnlyCollectionBase
    {
        // Methods
        public ComponentCollection(IComponent[] components)
        {
            base.InnerList.AddRange(components);
        }

        public void CopyTo(IComponent[] array, int index)
        {
            base.InnerList.CopyTo(array, index);
        }

        // Properties
        public virtual IComponent this[int index]
        {
            get
            {
                return (IComponent)base.InnerList[index];
            }
        }

        public virtual IComponent this[string name]
        {
            get
            {
                if (name != null)
                {
                    foreach (IComponent component in base.InnerList)
                    {
                        if (((component != null) && (component.Site != null)) && ((component.Site.Name != null) && string.Equals(component.Site.Name, name/*, StringComparison.OrdinalIgnoreCase*/)))
                        {
                            return component;
                        }
                    }
                }
                return null;
            }
        }
    }

}
