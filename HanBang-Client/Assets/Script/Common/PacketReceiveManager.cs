using MsgPack.Serialization;
using System.IO;

namespace GameServer.Packet
{
    public class PacketReceiveManager
    {
        private MemoryStream m_MemStream = new MemoryStream();

        public delegate void OnCSMoveDataDelegate(CSMoveData data);
        public OnCSMoveDataDelegate CSMoveData;

        public delegate void OnSCMoveDataDelegate(SCMoveData data);
        public OnSCMoveDataDelegate SCMoveData;

        public delegate void OnCSAttacDatakDelegate();
        public OnCSAttacDatakDelegate CSAttackData;

        public delegate void OnSCAttackDataDelegate(SCAttackData data);
        public OnSCAttackDataDelegate SCAttackData;

        public delegate void OnCSMatchBattleRoomDelegate();
        public OnCSMatchBattleRoomDelegate CSMatchBattleRoom;

        public delegate void OnSCMatchBattleRoomDelegate(SCMatchBattleRoom data);
        public OnSCMatchBattleRoomDelegate SCMatchBattleRoom;

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

                case PACKET_TYPE.SC_MOVE:
                    {
                        MessagePackSerializer<SCMoveData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCMoveData>();
                        SCMoveData(derializer.Unpack(m_MemStream));
                    }
                    break;

                case PACKET_TYPE.CS_ATTACK:
                    {
                        CSAttackData();
                    }
                    break;

                case PACKET_TYPE.SC_ATTACK:
                    {
                        MessagePackSerializer<SCAttackData> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCAttackData>();
                        SCAttackData(derializer.Unpack(m_MemStream));
                    }
                    break;

                case PACKET_TYPE.CS_MATCH_BATTLE_ROOM:
                    {
                        CSMatchBattleRoom();
                    }
                    break;

                case PACKET_TYPE.SC_MATCH_BATTLE_ROOM:
                    {
                        MessagePackSerializer<SCMatchBattleRoom> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCMatchBattleRoom>();
                        SCMatchBattleRoom(derializer.Unpack(m_MemStream));
                    }
                    break;
            }
        }
    }
}
