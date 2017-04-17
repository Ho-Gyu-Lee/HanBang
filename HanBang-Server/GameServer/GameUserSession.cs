using GameServer.Packet;
using GameServer.Protocol;
using SuperSocket.SocketBase;
using System;
using System.IO;

namespace GameServer
{
    class GameUserSession : AppSession<GameUserSession, PacketRequestInfo>
    {
        private PacketReceiveManager m_PacketReceiveManager = new PacketReceiveManager();
        private PacketSendManager m_PacketSendManager = new PacketSendManager();

        public GameUserSession()
            : base()
        {
            m_PacketSendManager.SendHandler += SendMsg;

            m_PacketReceiveManager.CSMoveData        += OnCSMoveData;
            m_PacketReceiveManager.CSAttackData      += OnCSAttackData;
            m_PacketReceiveManager.CSMatchBattleRoom += OnMatchBattleRoom;
        }

        protected override void OnSessionClosed(CloseReason reason)
        {

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

        public void SendMsg(int packetType, byte[] packetContent, int length)
        {
            byte[] msg = Write(packetType, packetContent, length);
            Send(msg, 0, msg.Length);
        }

        public void OnReceiveMessage(int type, byte[] body)
        {
            m_PacketReceiveManager.OnReceiveMessage(type, body);
        }

        private void OnCSMoveData(CSMoveData data)
        {
            
        }

        private void OnCSAttackData()
        {

        }

        private void OnMatchBattleRoom()
        {

        }
    }
}
