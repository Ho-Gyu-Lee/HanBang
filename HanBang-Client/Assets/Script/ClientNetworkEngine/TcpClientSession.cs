﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClientNetworkEngine
{
    public abstract class TcpClientSession : ClientSession
    {
        private bool m_InConnecting = false;

        public TcpClientSession()
            : this(10240)
        {

        }

        public TcpClientSession(int receiveBufferSize)
            : base()
        {
            ReceiveBufferSize = receiveBufferSize;
        }

        public override int ReceiveBufferSize
        {
            get
            {
                return base.ReceiveBufferSize;
            }

            set
            {
                if (Buffer.Array != null)
                    throw new Exception("ReceiveBufferSize cannot be set after the socket has been connected!");

                base.ReceiveBufferSize = value;
            }
        }

        protected virtual bool IsIgnorableException(Exception e)
        {
            if (e is System.ObjectDisposedException)
                return true;

            if (e is NullReferenceException)
                return true;

            return false;
        }

        protected bool IsIgnorableSocketError(int errorCode)
        {
            //SocketError.Shutdown = 10058
            //SocketError.ConnectionAborted = 10053
            //SocketError.ConnectionReset = 10054
            //SocketError.OperationAborted = 995
            if (errorCode == 10058 || errorCode == 10053 || 
                errorCode == 10054 || errorCode == 995 || 
                errorCode == 10004)
                return true;

            return false;
        }

        protected abstract void SocketEventArgsCompleted(object sender, SocketAsyncEventArgs e);

        public override void Connect(string hostName, int port)
        {
            if (m_InConnecting)
                throw new Exception("The socket is connecting, cannot connect again!");

            if (Client != null)
                throw new Exception("The socket is connected, you needn't connect again!");

            RemoteEndPoint = new IPEndPoint(IPAddress.Parse(hostName), port);

            //If there is a proxy set, connect the proxy server by proxy connector
            if (Proxy != null)
            {
                Proxy.Completed += new EventHandler<ProxyEventArgs>(Proxy_Completed);
                Proxy.Connect(RemoteEndPoint);
                m_InConnecting = true;
                return;
            }

            m_InConnecting = true;

            RemoteEndPoint.ConnectAsync(ProcessConnect, null);
        }

        public override void Connect(IPAddress ipAddress, int port)
        {
            if (m_InConnecting)
                throw new Exception("The socket is connecting, cannot connect again!");

            if (Client != null)
                throw new Exception("The socket is connected, you needn't connect again!");

            RemoteEndPoint = new IPEndPoint(ipAddress, port); ;

            //If there is a proxy set, connect the proxy server by proxy connector
            if (Proxy != null)
            {
                Proxy.Completed += new EventHandler<ProxyEventArgs>(Proxy_Completed);
                Proxy.Connect(RemoteEndPoint);
                m_InConnecting = true;
                return;
            }

            m_InConnecting = true;

            RemoteEndPoint.ConnectAsync(ProcessConnect, null);
        }

        void Proxy_Completed(object sender, ProxyEventArgs e)
        {
            Proxy.Completed -= new EventHandler<ProxyEventArgs>(Proxy_Completed);

            if (e.Connected)
            {
                ProcessConnect(e.Socket, null, null);
                return;
            }

            OnError(new Exception("proxy error", e.Exception));
            m_InConnecting = false;
        }

        protected void ProcessConnect(Socket socket, object state, SocketAsyncEventArgs e)
        {
            if (e != null && e.SocketError != SocketError.Success)
            {
                e.Dispose();
                m_InConnecting = false;
                OnError(new SocketException((int)e.SocketError));
                return;
            }

            if (socket == null)
            {
                m_InConnecting = false;
                OnError(new SocketException((int)SocketError.ConnectionAborted));
                return;
            }

            //To walk around a MonoTouch's issue
            //one user reported in some cases the e.SocketError = SocketError.Succes but the socket is not connected in MonoTouch
            if (!socket.Connected)
            {
                m_InConnecting = false;

                var socketError = (SocketError)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);

                OnError(new SocketException((int)socketError));
                return;
            }

            if (e == null)
                e = new SocketAsyncEventArgs();

            e.Completed += SocketEventArgsCompleted;

            Client = socket;

            m_InConnecting = false;

#if !SILVERLIGHT && !NETFX_CORE
            try
            {
                //Set keep alive
                Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
/*
#if !CLIENT
                int size = sizeof(UInt32);
                UInt32 on = 1;
                UInt32 keepAliveInterval = 1000 * 60;       // Send a packet once every 10 seconds.
                UInt32 retryInterval     = 1000 * 60;       // If no response, resend every second.
                byte[] inArray = new byte[size * 3];
                Array.Copy(BitConverter.GetBytes(on), 0, inArray, 0, size);
                Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, inArray, size, size);
                Array.Copy(BitConverter.GetBytes(retryInterval), 0, inArray, size * 2, size);

                Client.IOControl(IOControlCode.KeepAliveValues, inArray, null);
#endif
*/

            }
            catch
            {
            }
#endif

            OnGetSocket(e);
        }

        protected abstract void OnGetSocket(SocketAsyncEventArgs e);

        protected bool EnsureSocketClosed()
        {
            return EnsureSocketClosed(null);
        }

        protected bool EnsureSocketClosed(Socket prevClient)
        {
            var client = Client;

            if (client == null)
                return false;

            var fireOnClosedEvent = true;

            if (prevClient != null && prevClient != client) //originalClient is previous disconnected socket, so we needn't fire event for it
            {
                client = prevClient;
                fireOnClosedEvent = false;
            }
            else
            {
                Client = null;
                m_IsSending = 0;
            }

            try
            {
                client.Shutdown(SocketShutdown.Both);
            }
            catch
            { }
            finally
            {
                try
                {
                    client.Close();
                }
                catch
                { }
            }

            return fireOnClosedEvent;
        }

        private bool DetectConnected()
        {
            if (Client != null)
                return true;
            OnError(new SocketException((int)SocketError.NotConnected));
            return false;
        }

        private IBatchQueue<ArraySegment<byte>> m_SendingQueue;

        private IBatchQueue<ArraySegment<byte>> GetSendingQueue()
        {
            if (m_SendingQueue != null)
                return m_SendingQueue;

            lock (this)
            {
                if (m_SendingQueue != null)
                    return m_SendingQueue;

                //Sending queue size must be greater than 3
                m_SendingQueue = new ConcurrentBatchQueue<ArraySegment<byte>>(Math.Max(SendingQueueSize, 3), (t) => t.Array == null);
                return m_SendingQueue;
            }
        }

        private PosList<ArraySegment<byte>> m_SendingItems;

        private PosList<ArraySegment<byte>> GetSendingItems()
        {
            if (m_SendingItems == null)
                m_SendingItems = new PosList<ArraySegment<byte>>();

            return m_SendingItems;
        }

        private int m_IsSending = 0;

        protected bool IsSending
        {
            get { return m_IsSending == 1; }
        }

        public override bool TrySend(ArraySegment<byte> segment)
        {
            if (!DetectConnected())
            {
                //may be return false? 
                return true;
            }

            if (!GetSendingQueue().Enqueue(segment))
                return false;

            if (Interlocked.CompareExchange(ref m_IsSending, 1, 0) != 0)
                return true;

            DequeueSend();

            return true;
        }

        public override bool TrySend(IList<ArraySegment<byte>> segments)
        {
            if (!DetectConnected())
            {
                //may be return false? 
                return true;
            }

            if (!GetSendingQueue().Enqueue(segments))
                return false;

            if (Interlocked.CompareExchange(ref m_IsSending, 1, 0) != 0)
                return true;

            DequeueSend();

            return true;
        }

        private void DequeueSend()
        {
            var sendingItems = GetSendingItems();

            if (!m_SendingQueue.TryDequeue(sendingItems))
            {
                m_IsSending = 0;
                return;
            }

            SendInternal(sendingItems);
        }

        protected abstract void SendInternal(PosList<ArraySegment<byte>> items);

        protected void OnSendingCompleted()
        {
            var sendingItems = GetSendingItems();
            sendingItems.Clear();
            sendingItems.Position = 0;

            if (!m_SendingQueue.TryDequeue(sendingItems))
            {
                m_IsSending = 0;
                return;
            }

            SendInternal(sendingItems);
        }

        public override void Close()
        {
            if (EnsureSocketClosed())
                OnClosed();
        }
    }
}
