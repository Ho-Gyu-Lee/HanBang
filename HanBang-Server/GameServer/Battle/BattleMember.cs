using GameServer.Common.Packet;
using System;

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

        public GameSession GameSession { get; private set; }

        public BattleMember(int playerIndex, GameSession session)
        {
            PlayerIndex = playerIndex;
            GameSession = session;
        }
    }
}
