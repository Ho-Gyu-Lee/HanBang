using EchoServer.Protocol;
using SuperSocket.SocketBase;
using System;
using System.Text;

namespace EchoServer
{
    class EchoSession : AppSession<EchoSession, MyRequestInfo>
    {
        public void OnReceiveMessage(int type, byte[] body)
        {
            Console.WriteLine(Encoding.Default.GetString(body));

            byte[] msgTypeContent = BitConverter.GetBytes(type);
            byte[] msgSizeContent = BitConverter.GetBytes(body.Length);

            int totalSize = sizeof(int) * 2 + body.Length;

            byte[] msgByte = new byte[totalSize];

            Array.Copy(msgTypeContent, 0, msgByte, 0, msgTypeContent.Length);
            Array.Copy(msgSizeContent, 0, msgByte, msgTypeContent.Length, msgSizeContent.Length);
            Array.Copy(body, 0, msgByte, msgSizeContent.Length + msgTypeContent.Length, body.Length);

            Send(msgByte, 0, msgByte.Length);
        }
    }
}
