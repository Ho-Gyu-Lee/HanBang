using GameServer.Common.Packet;
using System;
using System.Collections.Concurrent;

namespace GameServer.Battle
{
    class BattleMember
    {
        private BattleMemberData m_BattleMemberData = new BattleMemberData();

        public BattleMemberData BattleMemberData { get { return m_BattleMemberData; } }

        public PLAYER_INDEX PlayerIndex { get { return m_BattleMemberData.m_PlayerIndex; } }

        public GameSession GameSession { get; private set; }

        public PosData MemberPos { get { return m_BattleMemberData.m_Pos; } }

        public bool MemberLook = false;

        public ConcurrentQueue<ACTION_TYPE> ActionDataQueue { get; set; }

        public BattleMember(PLAYER_INDEX playerIndex, GameSession session)
        {
            GameSession = session;

            m_BattleMemberData.m_PlayerIndex = playerIndex;

            Initialize();
        }

        public void Initialize()
        {
            switch (m_BattleMemberData.m_PlayerIndex)
            {
                case PLAYER_INDEX.PLAYER_1:
                    {
                        MemberLook = false;
                        m_BattleMemberData.m_Pos = new PosData(-3.0F, 0.0F);

                    }
                    break;
                case PLAYER_INDEX.PLAYER_2:
                    {
                        MemberLook = true;
                        m_BattleMemberData.m_Pos = new PosData(3.0F, 0.0F);
                    }
                    break;
            }
        }
    }
}
