using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Sockets
{
    public class UdpClient : IDisposable
    {
        // Fields
        private bool active;
        private bool disposed;
        private AddressFamily family;
        private byte[] recvbuffer;
        private Socket socket;

        // Methods
        public UdpClient()
            : this(AddressFamily.InterNetwork)
        {
        }

        public UdpClient(int port)
        {
            this.family = AddressFamily.InterNetwork;
            if ((port < 0) || (port > 65535))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            this.family = AddressFamily.InterNetwork;
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
            this.InitSocket(localEP);
        }

        public UdpClient(IPEndPoint localEP)
        {
            this.family = AddressFamily.InterNetwork;
            if (localEP == null)
            {
                throw new ArgumentNullException("localEP");
            }
            this.family = localEP.AddressFamily;
            this.InitSocket(localEP);
        }

        public UdpClient(AddressFamily family)
        {
            this.family = AddressFamily.InterNetwork;
            if ((family != AddressFamily.InterNetwork) && (family != AddressFamily.InterNetworkV6))
            {
                throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
            }
            this.family = family;
            this.InitSocket(null);
        }

        public UdpClient(int port, AddressFamily family)
        {
            IPEndPoint point;
            this.family = AddressFamily.InterNetwork;
            if ((family != AddressFamily.InterNetwork)
             //   && (family != AddressFamily.InterNetworkV6)
                )
            {
                throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
            }
            if ((port < 0) || (port > 65535))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            this.family = family;
          //  if (family == AddressFamily.InterNetwork)
            {
                point = new IPEndPoint(IPAddress.Any, port);
            }
            //else
            //{
            //    point = new IPEndPoint(IPAddress.IPv6Any, port);
            //}
            this.InitSocket(point);
        }

        public UdpClient(string hostname, int port)
        {
            this.family = AddressFamily.InterNetwork;
            if (hostname == null)
            {
                throw new ArgumentNullException("hostname");
            }
            if ((port < 0) || (port > 65535))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            this.InitSocket(null);
            this.Connect(hostname, port);
        }

        //public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        //{
        //    EndPoint point;
        //    this.CheckDisposed();
        //    this.recvbuffer = new byte[8192];
        //    if (this.family == AddressFamily.InterNetwork)
        //    {
        //        point = new IPEndPoint(IPAddress.Any, 0);
        //    }
        //    //else
        //    //{
        //    //    point = new IPEndPoint(IPAddress.IPv6Any, 0);
        //    //}
        //    return this.socket.BeginReceiveFrom(this.recvbuffer, 0, 8192, SocketFlags.None, ref point, callback, state);
        //}

        //public IAsyncResult BeginSend(byte[] datagram, int bytes, AsyncCallback requestCallback, object state)
        //{
        //    return this.BeginSend(datagram, bytes, null, requestCallback, state);
        //}

        //public IAsyncResult BeginSend(byte[] datagram, int bytes, IPEndPoint endPoint, AsyncCallback requestCallback, object state)
        //{
        //    this.CheckDisposed();
        //    if (datagram == null)
        //    {
        //        throw new ArgumentNullException("datagram");
        //    }
        //    return this.DoBeginSend(datagram, bytes, endPoint, requestCallback, state);
        //}

        //public IAsyncResult BeginSend(byte[] datagram, int bytes, string hostname, int port, AsyncCallback requestCallback, object state)
        //{
        //    return this.BeginSend(datagram, bytes, new IPEndPoint(Dns.GetHostAddresses(hostname)[0], port), requestCallback, state);
        //}

        private void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }

        public void Close()
        {
            ((IDisposable)this).Dispose();
        }

        public void Connect(IPEndPoint endPoint)
        {
            this.CheckDisposed();
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }
            this.DoConnect(endPoint);
            this.active = true;
        }

        public void Connect(IPAddress addr, int port)
        {
            if (addr == null)
            {
                throw new ArgumentNullException("addr");
            }
            if ((port < 0) || (port > 65535))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            this.Connect(new IPEndPoint(addr, port));
        }

        public void Connect(string hostname, int port)
        {
            if ((port < 0) || (port > 65535))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
            for (int i = 0; i < hostAddresses.Length; i++)
            {
                try
                {
                    this.family = hostAddresses[i].AddressFamily;
                    this.Connect(new IPEndPoint(hostAddresses[i], port));
                    break;
                }
                catch (Exception exception)
                {
                    if (i == (hostAddresses.Length - 1))
                    {
                        if (this.socket != null)
                        {
                            this.socket.Close();
                            this.socket = null;
                        }
                        throw exception;
                    }
                }
            }
        }

        private byte[] CutArray(byte[] orig, int length)
        {
            byte[] dst = new byte[length];
            Array.Copy(orig, 0, dst, 0, length);
//            Buffer.BlockCopy(orig, 0, dst, 0, length);
            return dst;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
                if (disposing)
                {
                    if (this.socket != null)
                    {
                        this.socket.Close();
                    }
                    this.socket = null;
                }
            }
        }

        //private IAsyncResult DoBeginSend(byte[] datagram, int bytes, IPEndPoint endPoint, AsyncCallback requestCallback, object state)
        //{
        //    try
        //    {
        //        if (endPoint == null)
        //        {
        //            return this.socket.BeginSend(datagram, 0, bytes, SocketFlags.None, requestCallback, state);
        //        }
        //        return this.socket.BeginSendTo(datagram, 0, bytes, SocketFlags.None, endPoint, requestCallback, state);
        //    }
        //    catch (SocketException exception)
        //    {
        //        if (exception.ErrorCode != 10013)
        //        {
        //            throw;
        //        }
        //        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        //        if (endPoint == null)
        //        {
        //            return this.socket.BeginSend(datagram, 0, bytes, SocketFlags.None, requestCallback, state);
        //        }
        //        return this.socket.BeginSendTo(datagram, 0, bytes, SocketFlags.None, endPoint, requestCallback, state);
        //    }
        //}

        private void DoConnect(IPEndPoint endPoint)
        {
            try
            {
                this.socket.Connect(endPoint);
            }
            catch (SocketException exception)
            {
                if (exception.ErrorCode != 10013)
                {
                    throw;
                }
              //  this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                this.socket.Connect(endPoint);
            }
        }

        private int DoSend(byte[] dgram, int bytes, IPEndPoint endPoint)
        {
            try
            {
                if (endPoint == null)
                {
                    return this.socket.Send(dgram, 0, bytes, SocketFlags.None);
                }
                return this.socket.SendTo(dgram, 0, bytes, SocketFlags.None, endPoint);
            }
            catch (SocketException exception)
            {
                if (exception.ErrorCode != 10013)
                {
                    throw;
                }
              //  this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                if (endPoint == null)
                {
                    return this.socket.Send(dgram, 0, bytes, SocketFlags.None);
                }
                return this.socket.SendTo(dgram, 0, bytes, SocketFlags.None, endPoint);
            }
        }

        //public void DropMulticastGroup(IPAddress multicastAddr)
        //{
        //    this.CheckDisposed();
        //    if (multicastAddr == null)
        //    {
        //        throw new ArgumentNullException("multicastAddr");
        //    }
        //    if (this.family == AddressFamily.InterNetwork)
        //    {
        //        this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(multicastAddr));
        //    }
        //    else
        //    {
        //        this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new IPv6MulticastOption(multicastAddr));
        //    }
        //}

        //public void DropMulticastGroup(IPAddress multicastAddr, int ifindex)
        //{
        //    this.CheckDisposed();
        //    if (multicastAddr == null)
        //    {
        //        throw new ArgumentNullException("multicastAddr");
        //    }
        //    if (this.family == AddressFamily.InterNetworkV6)
        //    {
        //        this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new IPv6MulticastOption(multicastAddr, (long)ifindex));
        //    }
        //}

        //public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
        //{
        //    EndPoint point;
        //    this.CheckDisposed();
        //    if (asyncResult == null)
        //    {
        //        throw new ArgumentNullException("asyncResult is a null reference");
        //    }
        //    if (this.family == AddressFamily.InterNetwork)
        //    {
        //        point = new IPEndPoint(IPAddress.Any, 0);
        //    }
        //    else
        //    {
        //        point = new IPEndPoint(IPAddress.IPv6Any, 0);
        //    }
        //    int length = this.socket.EndReceiveFrom(asyncResult, ref point);
        //    remoteEP = (IPEndPoint)point;
        //    byte[] destinationArray = new byte[length];
        //    Array.Copy(this.recvbuffer, destinationArray, length);
        //    return destinationArray;
        //}

        //public int EndSend(IAsyncResult asyncResult)
        //{
        //    this.CheckDisposed();
        //    if (asyncResult == null)
        //    {
        //        throw new ArgumentNullException("asyncResult is a null reference");
        //    }
        //    return this.socket.EndSend(asyncResult);
        //}

        ~UdpClient()
        {
            this.Dispose(false);
        }

        private void InitSocket(EndPoint localEP)
        {
            if (this.socket != null)
            {
                this.socket.Close();
                this.socket = null;
            }
            this.socket = new Socket(this.family, SocketType.Dgram, ProtocolType.Udp);
            if (localEP != null)
            {
                this.socket.Bind(localEP);
            }
        }

        //public void JoinMulticastGroup(IPAddress multicastAddr)
        //{
        //    this.CheckDisposed();
        //    if (multicastAddr == null)
        //    {
        //        throw new ArgumentNullException("multicastAddr");
        //    }
        //    if (this.family == AddressFamily.InterNetwork)
        //    {
        //        this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr));
        //    }
        //    else
        //    {
        //        this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(multicastAddr));
        //    }
        //}

        //public void JoinMulticastGroup(int ifindex, IPAddress multicastAddr)
        //{
        //    this.CheckDisposed();
        //    if (multicastAddr == null)
        //    {
        //        throw new ArgumentNullException("multicastAddr");
        //    }
        //    if (this.family != AddressFamily.InterNetworkV6)
        //    {
        //        throw new SocketException(10045);
        //    }
        //    this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(multicastAddr, (long)ifindex));
        //}

        //public void JoinMulticastGroup(IPAddress multicastAddr, int timeToLive)
        //{
        //    this.CheckDisposed();
        //    if (multicastAddr == null)
        //    {
        //        throw new ArgumentNullException("multicastAddr");
        //    }
        //    if ((timeToLive < 0) || (timeToLive > 255))
        //    {
        //        throw new ArgumentOutOfRangeException("timeToLive");
        //    }
        //    this.JoinMulticastGroup(multicastAddr);
        //    if (this.family == AddressFamily.InterNetwork)
        //    {
        //        this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, timeToLive);
        //    }
        //    else
        //    {
        //        this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, timeToLive);
        //    }
        //}

        //public void JoinMulticastGroup(IPAddress multicastAddr, IPAddress localAddress)
        //{
        //    this.CheckDisposed();
        //    if (this.family != AddressFamily.InterNetwork)
        //    {
        //        throw new SocketException(10045);
        //    }
        //    this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr, localAddress));
        //}

        public byte[] Receive(ref IPEndPoint remoteEP)
        {
            this.CheckDisposed();
            byte[] buffer = new byte[65536];
            var point = new IPEndPoint(IPAddress.Any, 0);
            int length = this.socket.ReceiveFrom(buffer, ref point);
            if (length < buffer.Length)
            {
                buffer = this.CutArray(buffer, length);
            }
            remoteEP = (IPEndPoint)point;
            return buffer;
        }

        public int Send(byte[] dgram, int bytes)
        {
            this.CheckDisposed();
            if (dgram == null)
            {
                throw new ArgumentNullException("dgram");
            }
            if (!this.active)
            {
                throw new InvalidOperationException("Operation not allowed on non-connected sockets.");
            }
            return this.DoSend(dgram, bytes, null);
        }

        public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
        {
            this.CheckDisposed();
            if (dgram == null)
            {
                throw new ArgumentNullException("dgram is null");
            }
            if (!this.active)
            {
                return this.DoSend(dgram, bytes, endPoint);
            }
            if (endPoint != null)
            {
                throw new InvalidOperationException("Cannot send packets to an arbitrary host while connected.");
            }
            return this.DoSend(dgram, bytes, null);
        }

        public int Send(byte[] dgram, int bytes, string hostname, int port)
        {
            return this.Send(dgram, bytes, new IPEndPoint(Dns.GetHostAddresses(hostname)[0], port));
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Properties
        protected bool Active
        {
            get
            {
                return this.active;
            }
            set
            {
                this.active = value;
            }
        }

        //public int Available
        //{
        //    get
        //    {
        //        return this.socket.Available;
        //    }
        //}

        public Socket Client
        {
            get
            {
                return this.socket;
            }
            set
            {
                this.socket = value;
            }
        }

        //public bool DontFragment
        //{
        //    get
        //    {
        //        return this.socket.DontFragment;
        //    }
        //    set
        //    {
        //        this.socket.DontFragment = value;
        //    }
        //}

        //public bool EnableBroadcast
        //{
        //    get
        //    {
        //        return this.socket.EnableBroadcast;
        //    }
        //    set
        //    {
        //        this.socket.EnableBroadcast = value;
        //    }
        //}

        //public bool ExclusiveAddressUse
        //{
        //    get
        //    {
        //        return this.socket.ExclusiveAddressUse;
        //    }
        //    set
        //    {
        //        this.socket.ExclusiveAddressUse = value;
        //    }
        //}

        //public bool MulticastLoopback
        //{
        //    get
        //    {
        //        return this.socket.MulticastLoopback;
        //    }
        //    set
        //    {
        //        this.socket.MulticastLoopback = value;
        //    }
        //}

        //public short Ttl
        //{
        //    get
        //    {
        //        return this.socket.Ttl;
        //    }
        //    set
        //    {
        //        this.socket.Ttl = value;
        //    }
        //}
    }


}
