using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ClientNetworkEngine
{
    public class UnityTcpSession : AsyncTcpSession
    {
        private volatile object _NextPacketLock = new object();
        private volatile byte[] _NextPacket     = null;

        public delegate void OnReceiveMessageDelegate(int msgType, byte[] msg);
        public OnReceiveMessageDelegate ReceiveMessage;

        public UnityTcpSession()
            : base()
        {

        }

        private byte[] ReadMsgContent(byte[] packet, int startIndex, int packetSize)
        {
            byte[] Msgcontent = new byte[packetSize];
            System.Buffer.BlockCopy(packet, startIndex, Msgcontent, 0, packetSize);

            return Msgcontent;
        }

        public byte[] Write(int MsgType, byte[] MsgContent, int length)
        {
            byte[] MsgTypeContent = BitConverter.GetBytes(MsgType);
            byte[] MsgSizeContent = BitConverter.GetBytes(length);

            int TotalSize = sizeof(int) * 2 + length;

            byte[] MsgByte = new byte[TotalSize];

            Array.Copy(MsgTypeContent, 0, MsgByte, 0, MsgTypeContent.Length);
            Array.Copy(MsgSizeContent, 0, MsgByte, MsgTypeContent.Length, MsgSizeContent.Length);
            Array.Copy(MsgContent, 0, MsgByte, MsgSizeContent.Length + MsgTypeContent.Length, length);

            return MsgByte;
        }

        public virtual void SendMsg(int packetType, byte[] packetContent, int length)
        {
            if (IsConnected)
            {
                byte[] msg = Write(packetType, packetContent, length);
                Send(msg, 0, msg.Length);
            }
        }

        protected virtual void OnReceiveMessage(int msgType, byte[] msg)
        {
            ReceiveMessage(msgType, msg);
        }

        protected override void OnClosed()
        {
            lock (_NextPacketLock)
            {
                _NextPacket = null;
            }

            base.OnClosed();
        }

        protected override void OnDataReceived(byte[] data, int offset, int length)
        {
            int bytesReceived = length;

            lock (_NextPacketLock)
            {
                byte[] packet = null;
                if (_NextPacket != null)
                {
                    packet = new byte[bytesReceived + _NextPacket.Length];
                    System.Buffer.BlockCopy(_NextPacket, 0, packet, 0, _NextPacket.Length);
                    System.Buffer.BlockCopy(data, offset, packet, _NextPacket.Length, bytesReceived);
                    bytesReceived += _NextPacket.Length;
                }
                else
                {
                    packet = new byte[bytesReceived];
                    System.Buffer.BlockCopy(data, offset, packet, 0, bytesReceived);
                }

                int LeftMsgSize = bytesReceived;
                while (true)
                {
                    // 남는 패킷이 없거나.
                    if (LeftMsgSize == 0)
                    {
                        _NextPacket = null;
                        break;
                    }

                    // 패킷이 모자를 경우.
                    if (LeftMsgSize < sizeof(int) * 2)
                    {
                        _NextPacket = packet;
                        break;
                    }

                    // 뭉쳐서 오는 패킷을 나눠서 사용한다.
                    int MsgSize = BitConverter.ToInt32(packet, sizeof(int)) + sizeof(int) * 2;

                    // 완전한 패킷이 아닐 경우.
                    if (LeftMsgSize < MsgSize)
                    {
                        _NextPacket = packet;
                        break;
                    }

                    // 패킷 한개 넘기기.
                    int msgLength  = BitConverter.ToInt32(packet, sizeof(int));
                    byte[] msgBody = new byte[msgLength];
                    System.Buffer.BlockCopy(packet, sizeof(int) * 2, msgBody, 0, msgLength);

                    OnReceiveMessage(BitConverter.ToInt32(packet, 0), msgBody);

                    // 나머지 패킷 자르기.
                    LeftMsgSize -= MsgSize;
                    packet = ReadMsgContent(packet, MsgSize, LeftMsgSize);
                }
            }
        }
    }
}
