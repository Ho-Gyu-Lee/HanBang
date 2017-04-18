using MsgPack.Serialization;
using System.IO;

namespace GameServer.Common.Packet
{
    public class PacketReceiveManager
    {
        private MemoryStream m_MemStream = new MemoryStream();

        public delegate void OnCSMoveDataDelegate(CSMoveData data);
        public OnCSMoveDataDelegate CSMoveData;

        public delegate void OnCSAttacDataDelegate(CSAttackData data);
        public OnCSAttacDataDelegate CSAttackData;

        public delegate void OnSCSyncBattleDataDelegate(SCSyncBattleData data);
        public OnSCSyncBattleDataDelegate SCSyncBattleData;

        public delegate void OnCSMatchBattleRoomDelegate();
        public OnCSMatchBattleRoomDelegate CSMatchBattleRoom;

        public delegate void OnSCMatchBattleRoomDataDelegate(SCMatchBattleRoomData data);
        public OnSCMatchBattleRoomDataDelegate SCMatchBattleRoomData;

        public delegate void OnSCBattleMemberSpawnDataDelegate(SCBattleMemberSpawnData data);
        public OnSCBattleMemberSpawnDataDelegate SCBattleMemberSpawnData;

        public void OnReceiveMessage(int type, byte[] body)
        {
            m_MemStream.SetLength(0);
            m_MemStream.Write(body, 0, body.Length);
            m_MemStream.Position = 0;

            switch ((PACKET_TYPE)type)
            {
                case PACKET_TYPE.CS_MOVE:
                    {
                        MessagePackSerializer<CSMoveData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<CSMoveData>();
                        CSMoveData(derializer.Unpack(m_MemStream));
                    }
                    break;

                case PACKET_TYPE.CS_ATTACK:
                    {
                        MessagePackSerializer<CSAttackData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<CSAttackData>();
                        CSAttackData(derializer.Unpack(m_MemStream));
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

                case PACKET_TYPE.SC_BATTLE_MEMBER_SPAWN:
                    {
                        MessagePackSerializer<SCBattleMemberSpawnData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCBattleMemberSpawnData>();
                        SCBattleMemberSpawnData(derializer.Unpack(m_MemStream));
                    }
                    break;
            }
        }
    }
}
