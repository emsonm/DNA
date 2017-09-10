using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Sockets
{
    public class TcpListener
    {
        // Fields
        private bool active;
        private EndPoint savedEP;
        private Socket server;

        // Methods
      //  [Obsolete("Use TcpListener (IPAddress address, int port) instead")]
        public TcpListener(int port)
        {
            if ((port < 0) || (port > 65535))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            this.Init(AddressFamily.InterNetwork, new IPEndPoint(IPAddress.Any, port));
        }

        public TcpListener(IPEndPoint local_end_point)
        {
            if (local_end_point == null)
            {
                throw new ArgumentNullException("local_end_point");
            }
            this.Init(local_end_point.AddressFamily, local_end_point);
        }

        public TcpListener(IPAddress listen_ip, int port)
        {
            if (listen_ip == null)
            {
                throw new ArgumentNullException("listen_ip");
            }
            if ((port < 0) || (port > 65535))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            this.Init(listen_ip.AddressFamily, new IPEndPoint(listen_ip, port));
        }

        public Socket AcceptSocket()
        {
            if (!this.active)
            {
                throw new InvalidOperationException("Socket is not listening");
            }
            return this.server.Accept();
        }

        public TcpClient AcceptTcpClient()
        {
            if (!this.active)
            {
                throw new InvalidOperationException("Socket is not listening");
            }
            Socket s = this.server.Accept();
            TcpClient client = new TcpClient();
            client.SetTcpClient(s);
            return client;
        }

        //public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
        //{
        //    if (this.server == null)
        //    {
        //        throw new ObjectDisposedException(base.GetType().ToString());
        //    }
        //    return this.server.BeginAccept(callback, state);
        //}

        //public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state)
        //{
        //    if (this.server == null)
        //    {
        //        throw new ObjectDisposedException(base.GetType().ToString());
        //    }
        //    return this.server.BeginAccept(callback, state);
        //}

        //public Socket EndAcceptSocket(IAsyncResult asyncResult)
        //{
        //    return this.server.EndAccept(asyncResult);
        //}

        //public TcpClient EndAcceptTcpClient(IAsyncResult asyncResult)
        //{
        //    Socket s = this.server.EndAccept(asyncResult);
        //    TcpClient client = new TcpClient();
        //    client.SetTcpClient(s);
        //    return client;
        //}

        ~TcpListener()
        {
            if (this.active)
            {
                this.Stop();
            }
        }

        private void Init(AddressFamily family, EndPoint ep)
        {
            this.active = false;
            this.server = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
            this.savedEP = ep;
        }

        //public bool Pending()
        //{
        //    if (!this.active)
        //    {
        //        throw new InvalidOperationException("Socket is not listening");
        //    }
        //    return this.server.Poll(0, SelectMode.SelectRead);
        //}

        public void Start()
        {
            this.Start(5);
        }

        public void Start(int backlog)
        {
            if (!this.active)
            {
                if (this.server == null)
                {
                    throw new InvalidOperationException("Invalid server socket");
                }
                this.server.Bind(this.savedEP);
                this.server.Listen(backlog);
                this.active = true;
            }
        }

        public void Stop()
        {
            if (this.active)
            {
                this.server.Close();
                this.server = null;
            }
            this.Init(AddressFamily.InterNetwork, this.savedEP);
        }

        // Properties
        protected bool Active
        {
            get
            {
                return this.active;
            }
        }

        //public bool ExclusiveAddressUse
        //{
        //    get
        //    {
        //        if (this.server == null)
        //        {
        //            throw new ObjectDisposedException(base.GetType().ToString());
        //        }
        //        if (this.active)
        //        {
        //            throw new InvalidOperationException("The TcpListener has been started");
        //        }
        //        return this.server.ExclusiveAddressUse;
        //    }
        //    set
        //    {
        //        if (this.server == null)
        //        {
        //            throw new ObjectDisposedException(base.GetType().ToString());
        //        }
        //        if (this.active)
        //        {
        //            throw new InvalidOperationException("The TcpListener has been started");
        //        }
        //        this.server.ExclusiveAddressUse = value;
        //    }
        //}

        public EndPoint LocalEndpoint
        {
            get
            {
                //if (this.active)
                //{
                //    return this.server.LocalEndPoint;
                //}
                return this.savedEP;
            }
        }

        public Socket Server
        {
            get
            {
                return this.server;
            }
        }
    }


}
