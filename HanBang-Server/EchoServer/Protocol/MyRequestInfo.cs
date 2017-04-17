using SuperSocket.SocketBase.Protocol;
using System;

namespace EchoServer.Protocol
{
    class MyRequestInfo : BinaryRequestInfo
    {
        public int Type { get; private set; }

        public MyRequestInfo(int type, byte[] body) 
            : base(String.Empty, body)
        {
            Type = type;
        }
    }
}
