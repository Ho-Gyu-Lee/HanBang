using SuperSocket.SocketBase.Protocol;
using System;

namespace GameServer.Protocol
{
    class PacketRequestInfo : BinaryRequestInfo
    {
        public int Type { get; private set; }

        public PacketRequestInfo(int type, byte[] body) 
            : base(String.Empty, body)
        {
            Type = type;
        }
    }
}
