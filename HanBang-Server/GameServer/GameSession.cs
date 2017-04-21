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

        private int m_RoomIndex = -1;

        private PLAYER_INDEX m_BattlePlayerIndex = PLAYER_INDEX.PLAYER_1;

        public GameSession()
            : base()
        {
            m_PacketSendManager.SendHandler += SendMsg;

            m_PacketReceiveManager.CSBattleMemberActionData += OnCSActionData;

            m_PacketReceiveManager.CSMatchBattleRoom  += OnMatchBattleRoom;
            m_PacketReceiveManager.CSBattleMemberData += OnBattleMemberData;

            m_PacketReceiveManager.CSReadyBattle += OnReadyBattle;
            m_PacketReceiveManager.CSLeaveBattleRoom += OnLeaveBattleRoom;
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            OnLeaveBattleRoom();
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

        private void OnCSActionData(CSBattleMemberActionData data)
        {
            Room.BattleRoom battleRoom = Room.BattleRoomManager.Instance.GetBattleRoom(m_RoomIndex);
            if (battleRoom == null)
            {
                Console.WriteLine("Not Find Battle Room");
                return;
            }

            data.m_PlayerIndex = m_BattlePlayerIndex;
            battleRoom.OnOnBattleMemberActionData(m_BattlePlayerIndex, data);
        }

        private void OnMatchBattleRoom()
        {
            SCMatchBattleRoomData data = new SCMatchBattleRoomData();
            Room.BattleRoomManager.Instance.MatchBattleRoom(this, ref m_RoomIndex, ref m_BattlePlayerIndex, ref data.m_BattleMapData);

            data.m_RoomIndex = m_RoomIndex;
            SendManager.SendSCMatchBattleRoomData(data);
        }

        private void OnBattleMemberData()
        {
            Room.BattleRoom battleRoom = Room.BattleRoomManager.Instance.GetBattleRoom(m_RoomIndex);
            if (battleRoom == null)
            {
                Console.WriteLine("Not Find Battle Room");
                return;
            }

            battleRoom.OnBattleMemberData();
        }

        private void OnReadyBattle()
        {
            Room.BattleRoom battleRoom = Room.BattleRoomManager.Instance.GetBattleRoom(m_RoomIndex);
            if (battleRoom == null)
            {
                Console.WriteLine("Not Find Battle Room");
                return;
            }

            battleRoom.ReadyBattle(m_BattlePlayerIndex);
        }

        private void OnLeaveBattleRoom()
        {
            if (m_RoomIndex > -1 && m_BattlePlayerIndex > PLAYER_INDEX.NONE)
            {
                Room.BattleRoom battleRoom = Room.BattleRoomManager.Instance.GetBattleRoom(m_RoomIndex);
                if (battleRoom != null)
                {
                    battleRoom.LeaveBattleRoom(m_BattlePlayerIndex);
                }

                m_RoomIndex = -1;
                m_BattlePlayerIndex = PLAYER_INDEX.NONE;
            }
        }
    }
}
