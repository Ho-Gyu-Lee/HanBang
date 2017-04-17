using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using System;

namespace EchoServer.Protocol
{
    class MyReceiveFilter : FixedHeaderReceiveFilter<MyRequestInfo>
    {
        public MyReceiveFilter() 
            : base(8)
        {
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return BitConverter.ToInt32(header, offset + 4);
        }

        protected override MyRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return new MyRequestInfo(BitConverter.ToInt32(header.Array, 0), bodyBuffer.CloneRange(offset, length));
        }
    }
}
