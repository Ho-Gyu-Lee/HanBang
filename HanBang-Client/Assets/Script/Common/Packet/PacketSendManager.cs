using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace GameServer.Packet
{
    public class PacketSendManager
    {
        private MemoryStream m_MemStream = new MemoryStream();

        public delegate void SendDelegate(int packetType, byte[] packetContent, int length);
        public SendDelegate SendHandler;

        public void SendCSMoveData(CSMoveData data)
        {
            m_MemStream.SetLength(0);
            m_MemStream.Position = 0;

            MessagePackSerializer<CSMoveData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<CSMoveData>();
            serializer.Pack(m_MemStream, data);

            SendHandler((int)PACKET_TYPE.CS_MOVE, m_MemStream.GetBuffer(), (int)m_MemStream.Length);
        }

        public void SendSCMoveData(SCMoveData data)
        {
            m_MemStream.SetLength(0);
            m_MemStream.Position = 0;

            MessagePackSerializer<SCMoveData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCMoveData>();
            serializer.Pack(m_MemStream, data);

            SendHandler((int)PACKET_TYPE.SC_MOVE, m_MemStream.GetBuffer(), (int)m_MemStream.Length);
        }

        public void SendCSAttackData()
        {
            byte[] buffer = new byte[0];
            SendHandler((int)PACKET_TYPE.CS_ATTACK, buffer, buffer.Length);
        }

        public void SendSCAttackData(SCAttackData data)
        {
            m_MemStream.SetLength(0);
            m_MemStream.Position = 0;

            MessagePackSerializer<SCAttackData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCAttackData>();
            serializer.Pack(m_MemStream, data);

            SendHandler((int)PACKET_TYPE.SC_ATTACK, m_MemStream.GetBuffer(), (int)m_MemStream.Length);
        }

        public void SendCSCreateBattleRoom()
        {
            byte[] buffer = new byte[0];
            SendHandler((int)PACKET_TYPE.CS_CREATE_BATTLE_ROOM, buffer, buffer.Length);
        }

        public void SendSCCreateBattleRoom(SCCreateBattleRoom data)
        {
            m_MemStream.SetLength(0);
            m_MemStream.Position = 0;

            MessagePackSerializer<SCCreateBattleRoom> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCCreateBattleRoom>();
            serializer.Pack(m_MemStream, data);

            SendHandler((int)PACKET_TYPE.SC_CREATE_BATTLE_ROOM, m_MemStream.GetBuffer(), (int)m_MemStream.Length);
        }
    }
}
