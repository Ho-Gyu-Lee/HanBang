using GameServer.Battle;
using GameServer.Common.Packet;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Room
{
    class BattleRoom
    {
        private const int MAX_MEMBER_COUNT = 2;

        private System.Threading.Timer m_BattleRoomTimer = null;

        private ConcurrentDictionary<PLAYER_INDEX, BattleMember> m_BattleMembers = new ConcurrentDictionary<PLAYER_INDEX, BattleMember>();

        private ConcurrentDictionary<PLAYER_INDEX, ConcurrentQueue<CSBattleMemberActionData>> m_BattleMemberActionDatas = new ConcurrentDictionary<PLAYER_INDEX, ConcurrentQueue<CSBattleMemberActionData>>();

        private BattleManager m_BattleManager = new BattleManager();

        private volatile int m_Frame = 0;

        private float m_TimePast = 0.0F;

        private int m_GameTimeRemain = 0;

        private object m_GameStartFlagLock = new object();

        private bool m_IsStartGame = false;

        private bool IsStartGame
        {
            get { lock (m_GameStartFlagLock) return m_IsStartGame; }
            set { lock (m_GameStartFlagLock) m_IsStartGame = value; }
        }

        public int m_RoomIndex = -1;

        private bool m_IsReadyPlayer1 = false;
        private bool m_IsReadyPlayer2 = false;

        public int MemberCount { get { return m_BattleMembers.Count; } }

        public BattleRoom(int roomIndex)
        {
            m_RoomIndex = roomIndex;
            m_BattleRoomTimer = new System.Threading.Timer(Update, new object(), 0, 1000 / 60);
        }

        public void OnOnBattleMemberActionData(PLAYER_INDEX playerIndex, CSBattleMemberActionData data)
        {
            if(m_BattleMemberActionDatas.ContainsKey(playerIndex))
            {
                m_BattleMemberActionDatas[playerIndex].Enqueue(data);
            }
        }

        public void OnBattleMemberData()
        {
            SCBattleMemberData data = new SCBattleMemberData();
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                data.m_BattleMemberDatas.Add((int)member.PlayerIndex, member.BattleMemberData);
            }

            foreach (BattleMember member in m_BattleMembers.Values)
            {
                data.m_MyPlayerIndex = member.PlayerIndex;
                member.GameSession.SendManager.SendSCBattleMemberData(data);
            }
        }

        private void RestartBattle()
        {
            m_TimePast = 0.0F;
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                member.Initialize();
            }
            IsStartGame = true;
        }

        public void Update(object state)
        {
            lock(state)
            {
                foreach(ConcurrentQueue<CSBattleMemberActionData> dataQueue in m_BattleMemberActionDatas.Values)
                {
                    if (dataQueue.Count > 0)
                    {
                        while (dataQueue.Count != 0)
                        {
                            CSBattleMemberActionData data = null;
                            if(dataQueue.TryDequeue(out data))
                            {
                                if(data.m_Frame == m_Frame)
                                {
                                    if(m_BattleMembers.ContainsKey(data.m_PlayerIndex))
                                    {
                                        m_BattleMembers[data.m_PlayerIndex].MemberActionType = data.m_ActionType;
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }

                // 공격 판정
                PLAYER_INDEX loserPlayer = PLAYER_INDEX.NONE;
                if(MemberCount == MAX_MEMBER_COUNT && m_BattleMembers.ContainsKey(PLAYER_INDEX.PLAYER_1) && m_BattleMembers.ContainsKey(PLAYER_INDEX.PLAYER_2))
                {
                    loserPlayer = m_BattleManager.UpdateGameResult(m_BattleMembers[PLAYER_INDEX.PLAYER_1], m_BattleMembers[PLAYER_INDEX.PLAYER_2]);
                }

                if(loserPlayer == PLAYER_INDEX.NONE)
                {
                    // 이동 처리
                    foreach (BattleMember member in m_BattleMembers.Values)
                    {
                        m_BattleManager.UpdatePlayerMapMove(member);
                    }
                }
                else
                {
                    // 승리 처리
                    if(m_BattleMembers.ContainsKey(loserPlayer))
                    {
                        m_BattleMembers[loserPlayer].MemberActionType = ACTION_TYPE.DIE;
                    }
                }

                m_TimePast += 1000 / 60;
                m_GameTimeRemain = 60 - ((int)m_TimePast / 1000);

                // 시간이 종료 되어 무승부 처리
                if (m_GameTimeRemain < 0)
                {
                    return;
                }

                Common.Packet.SCSyncBattleData syncBattleData = new Common.Packet.SCSyncBattleData();
                syncBattleData.m_Frame = m_Frame;
                syncBattleData.m_GameTimeRemain = m_GameTimeRemain;

                foreach (BattleMember member in m_BattleMembers.Values)
                {
                    syncBattleData.m_BattleMemberDatas.Add((int)member.PlayerIndex, member.BattleMemberData);
                }

                foreach (BattleMember member in m_BattleMembers.Values)
                {
                    member.GameSession.SendManager.SendSCSyncBattleData(syncBattleData);
                }

                Interlocked.Increment(ref m_Frame);
            }
        }

        public void JoinBattleRoom(GameSession session, out int roomIndex, out PLAYER_INDEX playerIndex, out BattleMapData battleMapData)
        {
            roomIndex     = m_RoomIndex;
            playerIndex   = (PLAYER_INDEX)MemberCount;
            battleMapData = m_BattleManager.BattleMapData;

            BattleMember member = new BattleMember(playerIndex, session);
            m_BattleMembers.TryAdd(member.PlayerIndex, member);
            m_BattleMemberActionDatas.TryAdd(member.PlayerIndex, new ConcurrentQueue<CSBattleMemberActionData>());
        }

        public void ReadyBattle(PLAYER_INDEX playerIndex)
        {
            switch(playerIndex)
            {
                case PLAYER_INDEX.PLAYER_1:
                    m_IsReadyPlayer1 = true;
                    break;
                case PLAYER_INDEX.PLAYER_2:
                    m_IsReadyPlayer2 = true;
                    break;
            }

            if (m_IsReadyPlayer1 && m_IsReadyPlayer2)
            {
                IsStartGame = true;
            }
        }

        public bool LeaveBattleRoom(PLAYER_INDEX playerIndex)
        {
            BattleMember member = null;
            m_BattleMembers.TryRemove(playerIndex, out member);

            if (MemberCount == 0)
            {
                CloseBattleRoom();
                return true;
            }

            return false;
        }

        public void CloseBattleRoom()
        {
            if (m_BattleRoomTimer != null)
            {
                m_BattleRoomTimer.Change(Timeout.Infinite, Timeout.Infinite);
                m_BattleRoomTimer.Dispose();
                m_BattleRoomTimer = null;
            }
        }
    }
}
