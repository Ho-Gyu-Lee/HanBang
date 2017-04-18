using GameServer.Common.Packet;
using GameServer.Protocol;
using SuperSocket.SocketBase;
using System;

namespace GameServer
{
    class GameSession : AppSession<GameSession, PacketRequestInfo>
    {
        private PacketReceiveManager m_PacketReceiveManager = new PacketReceiveManager();
        private PacketSendManager m_PacketSendManager = new PacketSendManager();

        public PacketSendManager SendManager { get { return m_PacketSendManager; } }

        public GameSession()
            : base()
        {
            m_PacketSendManager.SendHandler += SendMsg;

            m_PacketReceiveManager.CSMoveData         += OnCSMoveData;
            m_PacketReceiveManager.CSAttackData       += OnCSAttackData;
            m_PacketReceiveManager.CSMatchBattleRoom  += OnMatchBattleRoom;
            m_PacketReceiveManager.CSBattleMemberData += OnBattleMemberData;
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
            Room.BattleRoom battleRoom = Room.BattleRoomManager.Instance.GetBattleRoom(data.m_RoomIndex);
            if(battleRoom == null)
            {
                Console.WriteLine("Not Find Battle Room");
                return;
            }

            battleRoom.ChangeMemberMoveType(data.m_PlayerIndex, data.m_MoveType);
        }

        private void OnCSAttackData(CSAttackData data)
        {
            Room.BattleRoom battleRoom = Room.BattleRoomManager.Instance.GetBattleRoom(data.m_RoomIndex);
            if (battleRoom == null)
            {
                Console.WriteLine("Not Find Battle Room");
                return;
            }

            battleRoom.Attack(data.m_PlayerIndex);
        }

        private void OnMatchBattleRoom()
        {
            SCMatchBattleRoomData data = new SCMatchBattleRoomData();
            Room.BattleRoomManager.Instance.MatchBattleRoom(this, ref data.m_RoomIndex);

            SendManager.SendSCMatchBattleRoomData(data);
        }

        private void OnBattleMemberData(CSBattleMemberData data)
        {
            Room.BattleRoom battleRoom = Room.BattleRoomManager.Instance.GetBattleRoom(data.m_RoomIndex);
            if (battleRoom == null)
            {
                Console.WriteLine("Not Find Battle Room");
                return;
            }

            battleRoom.OnBattleMemberData();
        }
    }
}
