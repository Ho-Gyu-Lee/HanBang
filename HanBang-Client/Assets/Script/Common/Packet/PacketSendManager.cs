﻿using MsgPack.Serialization;
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
                if(m_MemStream == null)
                {
                    m_MemStream = new MemoryStream();
                }

                return m_MemStream;
            }
        }

        public delegate void SendDelegate(int packetType, byte[] packetContent, int length);
        public SendDelegate SendHandler;

        public void SendCSMoveData(CSMoveData data)
        {
            MemStream.SetLength(0);
            MemStream.Position = 0;

            MessagePackSerializer<CSMoveData> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<CSMoveData>();
            serializer.Pack(MemStream, data);

            SendHandler((int)PACKET_TYPE.CS_MOVE, MemStream.GetBuffer(), (int)MemStream.Length);
        }

        public void SendCSAttackData()
        {
            byte[] buffer = new byte[0];
            SendHandler((int)PACKET_TYPE.CS_ATTACK, buffer, buffer.Length);
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

        public void SendSCMatchBattleRoom(SCMatchBattleRoom data)
        {
            MemStream.SetLength(0);
            MemStream.Position = 0;

            MessagePackSerializer<SCMatchBattleRoom> serializer = MsgPack.Serialization.SerializationContext.Default.GetSerializer<SCMatchBattleRoom>();
            serializer.Pack(MemStream, data);

            SendHandler((int)PACKET_TYPE.SC_MATCH_BATTLE_ROOM, MemStream.GetBuffer(), (int)MemStream.Length);
        }
    }
}