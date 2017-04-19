using GameServer.Common.Packet;
using System;
using System.Collections.Concurrent;

namespace GameServer.Battle
{
    public class ActionData
    {
        public int m_Frame = -1;

        public ACTION_TYPE m_ActionType = ACTION_TYPE.NONE;
    }

    class BattleMember
    {
        private BattleMemberData m_BattleMemberData = new BattleMemberData();

        public BattleMemberData BattleMemberData { get { return m_BattleMemberData; } }

        public int PlayerIndex { get { return m_BattleMemberData.m_PlayerIndex; } }

        public GameSession GameSession { get; private set; }

        public PosData MemberPos { get { return m_BattleMemberData.m_Pos; } }

        public bool MemberLook = false;

        private object m_MemberActionTypeLock = new object();

        public ACTION_TYPE MemberActionType
        {
            get
            {
                lock(m_MemberActionTypeLock)
                    return m_BattleMemberData.m_ActionType;
            }

            set
            {
                lock(m_MemberActionTypeLock)
                    m_BattleMemberData.m_ActionType = value;
            }
        }

        public BattleMember(int playerIndex, GameSession session)
        {
            GameSession = session;

            m_BattleMemberData.m_PlayerIndex = playerIndex;

            switch (playerIndex)
            {
                case 0:
                    MemberLook = false;
                    m_BattleMemberData.m_Pos = new PosData(-6.0F, 0.0F);
                    break;
                case 1:
                    MemberLook = true;
                    m_BattleMemberData.m_Pos = new PosData(6.0F, 0.0F);
                    break;
            }
        }
    }
}
