using MsgPack.Serialization;
using System.IO;

namespace GameServer.Common.Packet
{
    public class PacketReceiveManager
    {
        private MemoryStream m_MemStream = new MemoryStream();

        public delegate void OnCSBattleMemberActionDataDelegate(CSBattleMemberActionData data);
        public OnCSBattleMemberActionDataDelegate CSBattleMemberActionData;

        public delegate void OnSCSyncBattleDataDelegate(SCSyncBattleData data);
        public OnSCSyncBattleDataDelegate SCSyncBattleData;

        public delegate void OnCSMatchBattleRoomDelegate();
        public OnCSMatchBattleRoomDelegate CSMatchBattleRoom;

        public delegate void OnSCMatchBattleRoomDataDelegate(SCMatchBattleRoomData data);
        public OnSCMatchBattleRoomDataDelegate SCMatchBattleRoomData;

        public delegate void OnCSBattleMemberDataDelegate(CSBattleMemberData data);
        public OnCSBattleMemberDataDelegate CSBattleMemberData;

        public delegate void OnSCBattleMemberDataDelegate(SCBattleMemberData data);
        public OnSCBattleMemberDataDelegate SCBattleMemberData;

        public void OnReceiveMessage(int type, byte[] body)
        {
            m_MemStream.SetLength(0);
            m_MemStream.Write(body, 0, body.Length);
            m_MemStream.Position = 0;

            switch ((PACKET_TYPE)type)
            {
                case PACKET_TYPE.CS_BATTLE_MEMBER_ACTION_DATA:
                    {
                        MessagePackSerializer<CSBattleMemberActionData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<CSBattleMemberActionData>();
                        CSBattleMemberActionData(derializer.Unpack(m_MemStream));
                    }
                    break;

                case PACKET_TYPE.SC_SYNC_BATTLE:
                    {
                        MessagePackSerializer<SCSyncBattleData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCSyncBattleData>();
                        SCSyncBattleData(derializer.Unpack(m_MemStream));
                    }
                    break;

                case PACKET_TYPE.CS_MATCH_BATTLE_ROOM:
                    {
                        CSMatchBattleRoom();
                    }
                    break;

                case PACKET_TYPE.SC_MATCH_BATTLE_ROOM:
                    {
                        MessagePackSerializer<SCMatchBattleRoomData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCMatchBattleRoomData>();
                        SCMatchBattleRoomData(derializer.Unpack(m_MemStream));
                    }
                    break;

                case PACKET_TYPE.CS_BATTLE_MEMBER_DATA:
                    {
                        MessagePackSerializer<CSBattleMemberData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<CSBattleMemberData>();
                        CSBattleMemberData(derializer.Unpack(m_MemStream));
                    }
                    break;

                case PACKET_TYPE.SC_BATTLE_MEMBER_DATA:
                    {
                        MessagePackSerializer<SCBattleMemberData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCBattleMemberData>();
                        SCBattleMemberData(derializer.Unpack(m_MemStream));
                    }
                    break;
            }
        }
    }
}
