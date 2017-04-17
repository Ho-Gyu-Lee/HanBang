using System;
using System.Collections;
using System.Collections.Generic;

namespace ClientNetworkEngine
{
    public interface IClientSession
    {
        IProxyConnector Proxy { get; set; }
        int ReceiveBufferSize { get; set; }
        int SendingQueueSize { get; set; }
        int SendTimeOut { get; set; }
        bool IsConnected { get; }

        void Connect(string hostName, int port);

        void Send(ArraySegment<byte> segment);

        void Send(IList<ArraySegment<byte>> segments);

        void Send(byte[] data, int offset, int length);

        bool TrySend(ArraySegment<byte> segment);

        bool TrySend(IList<ArraySegment<byte>> segments);

        void Close();

        event EventHandler Connected;

        event EventHandler Closed;

        event EventHandler<ErrorEventArgs> Error;

        event EventHandler<DataEventArgs> DataReceived;
    }
}
