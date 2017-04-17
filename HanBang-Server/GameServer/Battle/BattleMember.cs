using GameServer.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class BattleMember
    {
        public PosData m_PlayerPos = new PosData();

        private object m_MoveTypeLock = new object();
        private MOVE_TYPE m_PlayerMoveType = MOVE_TYPE.NONE;

        public MOVE_TYPE PlayerMoveType
        {
            get
            {
                lock(m_MoveTypeLock)
                {
                    return m_PlayerMoveType;
                }
            }

            set
            {
                lock(m_MoveTypeLock)
                {
                    m_PlayerMoveType = value;
                }
            }
        }

        public int PlayerIndex { get; private set; }

        public string m_SessionID = String.Empty;

        public BattleMember(string sessionID, int playerIndex)
        {
            m_SessionID = sessionID;
            PlayerIndex = playerIndex;
        }
    }
}
