using GameServer.Battle;
using GameServer.Common.Packet;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Room
{
    class BattleRoom
    {
        private const int MAX_MEMBER_COUNT = 2;

        private System.Threading.Timer m_BattleRoomTimer = null;

        private ConcurrentDictionary<int, BattleMember> m_BattleMembers = new ConcurrentDictionary<int, BattleMember>();

        private BattleManager m_BattleManager = new BattleManager();

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

        private void RestartBattle()
        {
            m_TimePast = 0.0F;
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                member.Initialize();
            }
            IsStartGame = true;

            foreach (BattleMember member in m_BattleMembers.Values)
            {
                member.GameSession.SendManager.SendSCStartBattle();
            }
        }

        private void SendBroadcastEndBattle()
        {
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                member.GameSession.SendManager.SendSCEndBattle();
            }
        }

        public void Update(object state)
        {
            lock(state)
            {
                if(IsStartGame)
                {
                    m_BattleManager.Update();

                    PLAYER_INDEX winPlayer = m_BattleManager.UpdateGameResult();

                    // 시간이 종료 되어 무승부 처리
                    if (m_GameTimeRemain < 0)
                    {
                        IsStartGame = false;

                        // 2초 후에 게임 다시  시작
                        var delay = Task.Delay(2000).ContinueWith(_ =>
                        {
                            RestartBattle();
                        });
                    }

                    m_TimePast += 1000 / 60;
                    m_GameTimeRemain = 60 - ((int)m_TimePast / 1000);

                    Common.Packet.SCSyncBattleData syncBattleData = new Common.Packet.SCSyncBattleData();
                    syncBattleData.m_GameTimeRemain = m_GameTimeRemain;

                    foreach (BattleMember member in m_BattleMembers.Values)
                    {
                        if (winPlayer != PLAYER_INDEX.NONE)
                        {
                            // 누군가 승리
                            if (member.PlayerIndex != (int)winPlayer)
                            {
                                m_IsStartGame = false;

                                member.BattleMemberData.m_ActionType = ACTION_TYPE.DIE;

                                // 2초 후에 게임 다시  시작
                                var delay = Task.Delay(2000).ContinueWith(_ =>
                                {
                                    RestartBattle();
                                });
                            }
                        }
                        syncBattleData.m_BattleMemberDatas.Add(member.PlayerIndex, member.BattleMemberData);
                    }

                    foreach (BattleMember member in m_BattleMembers.Values)
                    {
                        member.GameSession.SendManager.SendSCSyncBattleData(syncBattleData);
                    }
                }
            }
        }

        public void JoinBattleRoom(GameSession session, out int roomIndex, out int playerIndex, out BattleMapData battleMapData)
        {
            roomIndex     = m_RoomIndex;
            playerIndex   = MemberCount;
            battleMapData = m_BattleManager.BattleMapData;

            BattleMember member = new BattleMember(MemberCount, session);
            m_BattleMembers.TryAdd(member.PlayerIndex, member);
            m_BattleManager.SetBattleMember(member.PlayerIndex, member);
        }

        public void ReadyBattle(int playerIndex)
        {
            switch(playerIndex)
            {
                case 0:
                    m_IsReadyPlayer1 = true;
                    break;
                case 1:
                    m_IsReadyPlayer2 = true;
                    break;
            }

            if (m_IsReadyPlayer1 && m_IsReadyPlayer2)
            {
                IsStartGame = true;

                foreach (BattleMember member in m_BattleMembers.Values)
                {
                    member.GameSession.SendManager.SendSCStartBattle();
                }
            }
        }

        public bool LeaveBattleRoom(int playerIndex)
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

        public void SetBattleMemberActionData(int playerIndex, ACTION_TYPE actionType)
        {
            if (m_BattleMembers.ContainsKey(playerIndex))
            {
                m_BattleMembers[playerIndex].MemberActionType = actionType;
            }
        }
    }
}
