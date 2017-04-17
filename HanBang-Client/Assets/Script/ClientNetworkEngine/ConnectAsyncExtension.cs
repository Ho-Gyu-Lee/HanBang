﻿using System;
using System.Net;
using System.Net.Sockets;

namespace ClientNetworkEngine
{
    public static class ConnectAsyncExtension
    {
        public delegate void ConnectedCallback(Socket socket, object state, SocketAsyncEventArgs e);

        class ConnectToken
        {
            public object State { get; set; }

            public ConnectedCallback Callback { get; set; }
        }

        class DnsConnectState
        {
            public IPAddress[] Addresses { get; set; }

            public int NextAddressIndex { get; set; }

            public int Port { get; set; }

            public Socket Socket4 { get; set; }

            public Socket Socket6 { get; set; }

            public object State { get; set; }

            public ConnectedCallback Callback { get; set; }
        }

        public static void ConnectAsync(this EndPoint remoteEndPoint, ConnectedCallback callback, object state)
        {
            ConnectAsyncInternal(remoteEndPoint, callback, state);
        }

        private static void CreateAttempSocket(DnsConnectState connectState)
        {
            if (Socket.OSSupportsIPv6)
                connectState.Socket6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            connectState.Socket4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private static void ConnectAsyncInternal(this EndPoint remoteEndPoint, ConnectedCallback callback, object state)
        {
            if (remoteEndPoint is DnsEndPoint)
            {
                var dnsEndPoint = (DnsEndPoint)remoteEndPoint;

                var asyncResult = Dns.BeginGetHostAddresses(dnsEndPoint.Host, OnGetHostAddresses,
                    new DnsConnectState
                    {
                        Port = dnsEndPoint.Port,
                        Callback = callback,
                        State = state
                    });

                if (asyncResult.CompletedSynchronously)
                    OnGetHostAddresses(asyncResult);
            }
            else
            {
                var e = CreateSocketAsyncEventArgs(remoteEndPoint, callback, state);
                var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.ConnectAsync(e);
            }
        }

        private static SocketAsyncEventArgs CreateSocketAsyncEventArgs(EndPoint remoteEndPoint, ConnectedCallback callback, object state)
        {
            var e = new SocketAsyncEventArgs();

            e.UserToken = new ConnectToken
            {
                State = state,
                Callback = callback
            };

            e.RemoteEndPoint = remoteEndPoint;
            e.Completed += new EventHandler<SocketAsyncEventArgs>(SocketAsyncEventCompleted);

            return e;
        }

        private static void OnGetHostAddresses(IAsyncResult result)
        {
            var connectState = result.AsyncState as DnsConnectState;

            IPAddress[] addresses;

            try
            {
                addresses = Dns.EndGetHostAddresses(result);
            }
            catch
            {
                connectState.Callback(null, connectState.State, null);
                return;
            }

            if (addresses == null || addresses.Length <= 0)
            {
                connectState.Callback(null, connectState.State, null);
                return;
            }

            connectState.Addresses = addresses;

            CreateAttempSocket(connectState);

            Socket attempSocket;

            var address = GetNextAddress(connectState, out attempSocket);

            if (address == null)
            {
                connectState.Callback(null, connectState.State, null);
                return;
            }

            var socketEventArgs = new SocketAsyncEventArgs();
            socketEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(SocketConnectCompleted);
            var ipEndPoint = new IPEndPoint(address, connectState.Port);
            socketEventArgs.RemoteEndPoint = ipEndPoint;

            socketEventArgs.UserToken = connectState;

            if (!attempSocket.ConnectAsync(socketEventArgs))
                SocketConnectCompleted(attempSocket, socketEventArgs);
        }

        private static IPAddress GetNextAddress(DnsConnectState state, out Socket attempSocket)
        {
            IPAddress address = null;
            attempSocket = null;

            var currentIndex = state.NextAddressIndex;

            while (attempSocket == null)
            {
                if (currentIndex >= state.Addresses.Length)
                    return null;

                address = state.Addresses[currentIndex++];

                if (address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    attempSocket = state.Socket6;
                }
                else if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    attempSocket = state.Socket4;
                }
            }

            state.NextAddressIndex = currentIndex;
            return address;
        }

        private static void SocketAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= SocketAsyncEventCompleted;
            var token = (ConnectToken)e.UserToken;
            e.UserToken = null;
            token.Callback(sender as Socket, token.State, e);
        }

        private static void SocketConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            var connectState = e.UserToken as DnsConnectState;

            if (e.SocketError == SocketError.Success)
            {
                ClearSocketAsyncEventArgs(e);
                connectState.Callback((Socket)sender, connectState.State, e);
                return;
            }

            if (e.SocketError != SocketError.HostUnreachable && e.SocketError != SocketError.ConnectionRefused)
            {
                ClearSocketAsyncEventArgs(e);
                connectState.Callback(null, connectState.State, e);
                return;
            }

            Socket attempSocket;

            var address = GetNextAddress(connectState, out attempSocket);

            if (address == null)
            {
                ClearSocketAsyncEventArgs(e);
                e.SocketError = SocketError.HostUnreachable;
                connectState.Callback(null, connectState.State, e);
                return;
            }

            e.RemoteEndPoint = new IPEndPoint(address, connectState.Port);

            if (!attempSocket.ConnectAsync(e))
                SocketConnectCompleted(attempSocket, e);
        }

        private static void ClearSocketAsyncEventArgs(SocketAsyncEventArgs e)
        {
            e.Completed -= SocketConnectCompleted;
            e.UserToken = null;
        }
    }
}
