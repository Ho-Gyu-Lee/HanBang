using MsgPack.Serialization;
using System.IO;

namespace GameServer.Common.Packet
{
    public class PacketSendManager
    {
        [System.ThreadStatic]
        private static MemoryStream m_MemStream = null;

        private MemoryStream MemStream
        {
            get
            {
                if (m_MemStream == null)
                {
                    m_MemStream = new MemoryStream();
                }

                return m_MemStream;
            }
        }

        public delegate void SendDelegate(int packetType, byte[] packetContent, int length);
        public SendDelegate SendHandler;

        public void SendCSBattleMemberActionData(CSBattleMemberActionData data)
        {
            MemStream.SetLength(0);
            MemStream.Position = 0;

            MessagePackSerializer<CSBattleMemberActionData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<CSBattleMemberActionData>();
            serializer.Pack(MemStream, data);

            SendHandler((int)PACKET_TYPE.CS_BATTLE_MEMBER_ACTION_DATA, MemStream.GetBuffer(), (int)MemStream.Length);
        }

        public void SendSCBattleMemberActionData()
        {
            byte[] buffer = new byte[0];
            SendHandler((int)PACKET_TYPE.SC_BATTLE_MEMBER_ACTION_DATA, buffer, buffer.Length);
        }

        public void SendSCSyncBattleData(SCSyncBattleData data)
        {
            MemStream.SetLength(0);
            MemStream.Position = 0;

            MessagePackSerializer<SCSyncBattleData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCSyncBattleData>();
            serializer.Pack(MemStream, data);

            SendHandler((int)PACKET_TYPE.SC_SYNC_BATTLE, MemStream.GetBuffer(), (int)MemStream.Length);
        }

        public void SendCSMatchBattleRoom()
        {
            byte[] buffer = new byte[0];
            SendHandler((int)PACKET_TYPE.CS_MATCH_BATTLE_ROOM, buffer, buffer.Length);
        }

        public void SendSCMatchBattleRoomData(SCMatchBattleRoomData data)
        {
            MemStream.SetLength(0);
            MemStream.Position = 0;

            MessagePackSerializer<SCMatchBattleRoomData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCMatchBattleRoomData>();
            serializer.Pack(MemStream, data);

            SendHandler((int)PACKET_TYPE.SC_MATCH_BATTLE_ROOM, MemStream.GetBuffer(), (int)MemStream.Length);
        }

        public void SendCSBattleMemberData()
        {
            byte[] buffer = new byte[0];
            SendHandler((int)PACKET_TYPE.CS_BATTLE_MEMBER_DATA, buffer, buffer.Length);
        }

        public void SendSCBattleMemberData(SCBattleMemberData data)
        {
            MemStream.SetLength(0);
            MemStream.Position = 0;

            MessagePackSerializer<SCBattleMemberData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCBattleMemberData>();
            serializer.Pack(MemStream, data);

            SendHandler((int)PACKET_TYPE.SC_BATTLE_MEMBER_DATA, MemStream.GetBuffer(), (int)MemStream.Length);
        }

        public void SendCSReadyBattle()
        {
            byte[] buffer = new byte[0];
            SendHandler((int)PACKET_TYPE.CS_READY_BATTLE, buffer, buffer.Length);
        }

        public void SendCSLeaveBattleRoom()
        {
            byte[] buffer = new byte[0];
            SendHandler((int)PACKET_TYPE.CS_LEAVE_BATTLE_ROOM, buffer, buffer.Length);
        }

        public void SendSCBattleWatingData(SCBattleWatingData data)
        {
            MemStream.SetLength(0);
            MemStream.Position = 0;

            MessagePackSerializer<SCBattleWatingData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCBattleWatingData>();
            serializer.Pack(MemStream, data);

            SendHandler((int)PACKET_TYPE.SC_BATTLE_WATITING_DATA, MemStream.GetBuffer(), (int)MemStream.Length);
        }
    }
}
