using GameServer.Common.Packet;
using System;
using System.Collections.Concurrent;

namespace GameServer.Battle
{
    class BattleMember
    {
        private BattleMemberData m_BattleMemberData = new BattleMemberData();

        public BattleMemberData BattleMemberData { get { return m_BattleMemberData; } }

        public int PlayerIndex { get { return m_BattleMemberData.m_PlayerIndex; } }

        public GameSession GameSession { get; private set; }

        public PosData MemberPos { get { return m_BattleMemberData.m_Pos; } }

        public MOVE_TYPE MemberMoveType
        {
            get { return m_BattleMemberData.m_MoveType; }
            set { m_BattleMemberData.m_MoveType = value; }
        }

        public ConcurrentQueue<MOVE_TYPE> m_InputQueue = new ConcurrentQueue<MOVE_TYPE>();

        public BattleMember(int playerIndex, GameSession session)
        {
            GameSession = session;

            m_BattleMemberData.m_PlayerIndex = playerIndex;

            switch (playerIndex)
            {
                case 0:
                    m_BattleMemberData.m_Pos = new PosData(-6.0F, 0.0F);
                    break;
                case 1:
                    m_BattleMemberData.m_Pos = new PosData(6.0F, 0.0F);
                    break;
            }
        }
    }
}
