using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
public class Component : MarshalByRefObject, IComponent, IDisposable
{
    // Fields
    private static readonly object EventDisposed = new object();
    private EventHandlerList events;
    private ISite site;

    // Events
    public event EventHandler Disposed
    {
        add
        {
            this.Events.AddHandler(EventDisposed, value);
        }
        remove
        {
            this.Events.RemoveHandler(EventDisposed, value);
        }
    }

    // Methods
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (this)
            {
                if ((this.site != null) && (this.site.Container != null))
                {
                    this.site.Container.Remove(this);
                }
                if (this.events != null)
                {
                    EventHandler handler = (EventHandler) this.events[EventDisposed];
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            }
        }
    }

    ~Component()
    {
        this.Dispose(false);
    }

    protected virtual object GetService(Type service)
    {
        ISite site = this.site;
        if (site != null)
        {
            return site.GetService(service);
        }
        return null;
    }

    public override string ToString()
    {
        ISite site = this.site;
        if (site != null)
        {
            return (site.Name + " [" + base.GetType().FullName + "]");
        }
        return base.GetType().FullName;
    }

    // Properties
    protected virtual bool CanRaiseEvents
    {
        get
        {
            return true;
        }
    }

    internal bool CanRaiseEventsInternal
    {
        get
        {
            return this.CanRaiseEvents;
        }
    }

    public IContainer Container
    {
        get
        {
            ISite site = this.site;
            if (site != null)
            {
                return site.Container;
            }
            return null;
        }
    }

    protected bool DesignMode
    {
        get
        {
            ISite site = this.site;
            return ((site != null) && site.DesignMode);
        }
    }

    protected EventHandlerList Events
    {
        get
        {
            if (this.events == null)
            {
                this.events = new EventHandlerList();//this);
            }
            return this.events;
        }
    }

    public virtual ISite Site
    {
        get
        {
            return this.site;
        }
        set
        {
            this.site = value;
        }
    }
}

 

}
