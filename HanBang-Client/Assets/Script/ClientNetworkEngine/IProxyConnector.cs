using System;
using System.Net;
using System.Collections;

namespace ClientNetworkEngine
{
    public interface IProxyConnector
    {
        void Connect(EndPoint remoteEndPoint);
        event EventHandler<ProxyEventArgs> Completed;
    }
}
