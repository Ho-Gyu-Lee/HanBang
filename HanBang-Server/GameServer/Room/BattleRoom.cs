using GameServer.Battle;
using GameServer.Common.Packet;
using System.Collections.Concurrent;
using System.Threading;

namespace GameServer.Room
{
    class BattleRoom
    {
        private const int MAX_MEMBER_COUNT = 2;

        private System.Threading.Timer m_BattleRoomTimer = null;

        private ConcurrentDictionary<int, BattleMember> m_BattleMembers = new ConcurrentDictionary<int, BattleMember>();

        private BattleManager m_BattleManager = new BattleManager();

        private volatile int m_Frame = 0;

        private float m_TimePast = 0.0F;

        private int m_GameTimeRemain = 0;

        private bool m_IsGameStart = false;

        public int m_RoomIndex = -1;

        public int MemberCount { get { return m_BattleMembers.Count; } }

        public BattleRoom(int roomIndex)
        {
            m_RoomIndex = roomIndex;
            m_BattleRoomTimer = new System.Threading.Timer(Update, new object(), 0, 1000 / 60);
        }

        public void OnBattleMemberData()
        {
            SCBattleMemberData data = new SCBattleMemberData();
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                data.m_BattleMemberDatas.Add(member.PlayerIndex, member.BattleMemberData);
            }

            foreach (BattleMember member in m_BattleMembers.Values)
            {
                data.m_MyPlayerIndex = member.PlayerIndex;
                member.GameSession.SendManager.SendSCBattleMemberData(data);
            }
        }

        public void Update(object state)
        {
            lock(state)
            {
                m_BattleManager.Update();

                PLAYER_INDEX winPlayer = m_BattleManager.UpdateGameResult();

                if (m_GameTimeRemain < 0)
                {
                    m_TimePast = 0.0F;

                    foreach (BattleMember member in m_BattleMembers.Values)
                    {
                        member.Initialize();
                    }
                }
                else
                {
                    // 누군가 죽어 있다면
                    foreach (BattleMember member in m_BattleMembers.Values)
                    {
                        if(member.MemberActionType == ACTION_TYPE.DIE)
                        {
                            member.Initialize();
                            m_GameTimeRemain = 0;
                            m_TimePast = 0.0F;
                        }
                    }
                }

                m_TimePast += 1000 / 60;
                m_GameTimeRemain = 10 - ((int)m_TimePast / 1000);

                Common.Packet.SCSyncBattleData syncBattleData = new Common.Packet.SCSyncBattleData();
                syncBattleData.m_Frame = m_Frame;
                syncBattleData.m_GameTimeRemain = m_GameTimeRemain;

                foreach (BattleMember member in m_BattleMembers.Values)
                {
                    if(winPlayer != PLAYER_INDEX.NONE)
                    {
                        if (member.PlayerIndex != (int)winPlayer)
                            member.BattleMemberData.m_ActionType = ACTION_TYPE.DIE;
                    }
                    syncBattleData.m_BattleMemberDatas.Add(member.PlayerIndex, member.BattleMemberData);
                }

                foreach (BattleMember member in m_BattleMembers.Values)
                {
                    member.GameSession.SendManager.SendSCSyncBattleData(syncBattleData);
                }

                Interlocked.Increment(ref m_Frame);
            }
        }

        public void JoinBattleRoom(GameSession session, out int roomIndex)
        {
            roomIndex   = m_RoomIndex;

            BattleMember member = new BattleMember(MemberCount, session);
            m_BattleMembers.TryAdd(member.PlayerIndex, member);
            m_BattleManager.SetBattleMember(member.PlayerIndex, member);

            if (MemberCount == MAX_MEMBER_COUNT)
                m_IsGameStart = true;
        }

        public void CloseBattle()
        {
            if (m_BattleRoomTimer != null)
            {
                m_BattleRoomTimer.Change(Timeout.Infinite, Timeout.Infinite);
                m_BattleRoomTimer.Dispose();
                m_BattleRoomTimer = null;
            }
        }

        public void SetBattleMemberActionData(int playerIndex, ACTION_TYPE actionType)
        {
            if (m_BattleMembers.ContainsKey(playerIndex))
            {
                m_BattleMembers[playerIndex].MemberActionType = actionType;
            }
        }
    }
}
