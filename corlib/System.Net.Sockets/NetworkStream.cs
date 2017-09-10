using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;

namespace System.Net.Sockets
{
    public class NetworkStream : Stream, IDisposable
    {
        // Fields
        private FileAccess access;
        private bool disposed;
        private bool owns_socket;
        private bool readable;
        private Socket socket;
        private bool writeable;

        // Methods
        public NetworkStream(Socket socket)
            : this(socket, FileAccess.ReadWrite, false)
        {
        }

        public NetworkStream(Socket socket, bool owns_socket)
            : this(socket, FileAccess.ReadWrite, owns_socket)
        {
        }

        public NetworkStream(Socket socket, FileAccess access)
            : this(socket, access, false)
        {
        }

        public NetworkStream(Socket socket, FileAccess access, bool owns_socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException("socket is null");
            }
            if (socket.SocketType != SocketType.Stream)
            {
                throw new ArgumentException("Socket is not of type Stream", "socket");
            }
            //if (!socket.Connected)
            //{
            //    throw new IOException("Not connected");
            //}
            //if (!socket.Blocking)
            //{
            //    throw new IOException("Operation not allowed on a non-blocking socket.");
            //}
            this.socket = socket;
            this.owns_socket = owns_socket;
            this.access = access;
            this.readable = this.CanRead;
            this.writeable = this.CanWrite;
        }

        //public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        //{
        //    IAsyncResult result;
        //    this.CheckDisposed();
        //    if (buffer == null)
        //    {
        //        throw new ArgumentNullException("buffer is null");
        //    }
        //    int length = buffer.Length;
        //    if ((offset < 0) || (offset > length))
        //    {
        //        throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
        //    }
        //    if ((size < 0) || ((offset + size) > length))
        //    {
        //        throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
        //    }
        //    try
        //    {
        //        result = this.socket.BeginReceive(buffer, offset, size, SocketFlags.None, callback, state);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new IOException("BeginReceive failure", exception);
        //    }
        //    return result;
        //}

        //public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        //{
        //    IAsyncResult result;
        //    this.CheckDisposed();
        //    if (buffer == null)
        //    {
        //        throw new ArgumentNullException("buffer is null");
        //    }
        //    int length = buffer.Length;
        //    if ((offset < 0) || (offset > length))
        //    {
        //        throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
        //    }
        //    if ((size < 0) || ((offset + size) > length))
        //    {
        //        throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
        //    }
        //    try
        //    {
        //        result = this.socket.BeginSend(buffer, offset, size, SocketFlags.None, callback, state);
        //    }
        //    catch
        //    {
        //        throw new IOException("BeginWrite failure");
        //    }
        //    return result;
        //}

        private void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }

        public void Close(int timeout)
        {
            if (timeout < -1)
            {
                throw new ArgumentOutOfRangeException("timeout", "timeout is less than -1");
            }
            //Timer timer = new Timer();
            //timer.Elapsed += new ElapsedEventHandler(this.OnTimeoutClose);
            //timer.Interval = timeout;
            //timer.AutoReset = false;
            //timer.Enabled = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
                if (this.owns_socket)
                {
                    Socket socket = this.socket;
                    if (socket != null)
                    {
                        socket.Close();
                    }
                }
                this.socket = null;
                this.access = 0;
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        //public override int EndRead(IAsyncResult ar)
        //{
        //    int num;
        //    this.CheckDisposed();
        //    if (ar == null)
        //    {
        //        throw new ArgumentNullException("async result is null");
        //    }
        //    try
        //    {
        //        num = this.socket.EndReceive(ar);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new IOException("EndRead failure", exception);
        //    }
        //    return num;
        //}

        //public override void EndWrite(IAsyncResult ar)
        //{
        //    this.CheckDisposed();
        //    if (ar == null)
        //    {
        //        throw new ArgumentNullException("async result is null");
        //    }
        //    try
        //    {
        //        this.socket.EndSend(ar);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new IOException("EndWrite failure", exception);
        //    }
        //}

        ~NetworkStream()
        {
            this.Dispose(false);
        }

        public override void Flush()
        {
        }

        //private void OnTimeoutClose(object source, ElapsedEventArgs e)
        //{
        //    this.Close();
        //}

        public override int Read([In, Out] byte[] buffer, int offset, int size)
        {
            int num;
            this.CheckDisposed();
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer is null");
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
            }
            if ((size < 0) || ((offset + size) > buffer.Length))
            {
                throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
            }
            try
            {
                num = this.socket.Receive(buffer, offset, size, SocketFlags.None);
            }
            catch (Exception exception)
            {
                throw new IOException("Read failure", exception);
            }
            return num;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int size)
        {
            this.CheckDisposed();
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if ((offset < 0) || (offset > buffer.Length))
            {
                throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
            }
            if ((size < 0) || (size > (buffer.Length - offset)))
            {
                throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
            }
            try
            {
                for (int i = 0; (size - i) > 0; i += this.socket.Send(buffer, offset + i, size - i, SocketFlags.None))
                {
                }
            }
            catch (Exception exception)
            {
                throw new IOException("Write failure", exception);
            }
        }

        // Properties
        public override bool CanRead
        {
            get
            {
                return ((this.access == FileAccess.ReadWrite) || (this.access == FileAccess.Read));
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return ((this.access == FileAccess.ReadWrite) || (this.access == FileAccess.Write));
            }
        }

        public virtual bool DataAvailable
        {
            get
            {
                this.CheckDisposed();
                return true;// (this.socket.Available > 0);
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        protected bool Readable
        {
            get
            {
                return this.readable;
            }
            set
            {
                this.readable = value;
            }
        }

        //public override int ReadTimeout
        //{
        //    get
        //    {
        //        return this.socket.ReceiveTimeout;
        //    }
        //    set
        //    {
        //        if ((value <= 0) && (value != -1))
        //        {
        //            throw new ArgumentOutOfRangeException("value", "The value specified is less than or equal to zero and is not Infinite.");
        //        }
        //        this.socket.ReceiveTimeout = value;
        //    }
        //}

        protected Socket Socket
        {
            get
            {
                return this.socket;
            }
        }

        protected bool Writeable
        {
            get
            {
                return this.writeable;
            }
            set
            {
                this.writeable = value;
            }
        }

        //public override int WriteTimeout
        //{
        //    get
        //    {
        //        return this.socket.SendTimeout;
        //    }
        //    set
        //    {
        //        if ((value <= 0) && (value != -1))
        //        {
        //            throw new ArgumentOutOfRangeException("value", "The value specified is less than or equal to zero and is not Infinite");
        //        }
        //        this.socket.SendTimeout = value;
        //    }
        //}
    }



}
