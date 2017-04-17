using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using System;

namespace GameServer.Protocol
{
    class PacketReceiveFilter : FixedHeaderReceiveFilter<PacketRequestInfo>
    {
        public PacketReceiveFilter() 
            : base(8)
        {
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return BitConverter.ToInt32(header, offset + 4);
        }

        protected override PacketRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return new PacketRequestInfo(BitConverter.ToInt32(header.Array, 0), bodyBuffer.CloneRange(offset, length));
        }
    }
}
