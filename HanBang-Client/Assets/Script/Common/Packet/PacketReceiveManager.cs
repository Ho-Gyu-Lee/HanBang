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

        public delegate void OnCSCreateBattleRoomDelegate();
        public OnCSCreateBattleRoomDelegate CSCreateBattleRoom;

        public delegate void OnSCCreateBattleRoomDelegate(SCCreateBattleRoom data);
        public OnSCCreateBattleRoomDelegate SCCreateBattleRoom;

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

                case PACKET_TYPE.CS_CREATE_BATTLE_ROOM:
                    {
                        CSCreateBattleRoom();
                    }
                    break;

                case PACKET_TYPE.SC_CREATE_BATTLE_ROOM:
                    {
                        MessagePackSerializer<SCCreateBattleRoom> derializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCCreateBattleRoom>();
                        SCCreateBattleRoom(derializer.Unpack(m_MemStream));
                    }
                    break;
            }
        }
    }
}
