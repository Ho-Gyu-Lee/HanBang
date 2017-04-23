using GameServer.Battle;
using GameServer.Common.Packet;
using GameServer.Room.RoomState;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Common.Util;

namespace GameServer.Room
{
    class BattleRoom
    {
        public const int MAX_MEMBER_COUNT = 2;

        private System.Threading.Timer m_BattleRoomTimer = null;

        private ConcurrentDictionary<PLAYER_INDEX, BattleMember> m_BattleMembers = new ConcurrentDictionary<PLAYER_INDEX, BattleMember>();

        private BattleManager m_BattleManager = new BattleManager();

        private object StateLock = new object();

        private ROOM_STATE State { get; set; }

        private BattleRoomState BattleRoomState { get; set; }

        private volatile int m_Frame = 0;

        private float m_GameTimeRemain = 60.0F;

        private Time m_Time = null;

        public BattleTerrainData BattleTerrainData { get; private set; }

        public int m_RoomIndex = -1;

        private bool m_IsReadyPlayer1 = false;
        private bool m_IsReadyPlayer2 = false;

        public int MemberCount { get { return m_BattleMembers.Count; } }

        public BattleRoom(int roomIndex, int mapIndex)
        {
            m_RoomIndex = roomIndex;

            #region 임시 맵 정보 셋팅
            BattleTerrainData = new BattleTerrainData();

            BattleTerrainData.m_MinSizeX = -12.80F;
            BattleTerrainData.m_MaxSizeX = 11.52F;

            BattleTerrainData.m_MinSizeY = -11.52F;
            BattleTerrainData.m_MaxSizeY = 12.8F;

            Random random = new Random();
            ObstacleData obstacleData1 = new ObstacleData();
            obstacleData1.m_Pos = new PosData(-8.36F, 10.9F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData1);

            ObstacleData obstacleData2 = new ObstacleData();
            obstacleData2.m_Pos = new PosData(-1.4F, 8.6F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData2);

            ObstacleData obstacleData3 = new ObstacleData();
            obstacleData3.m_Pos = new PosData(5.0F, 8.8F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData3);

            ObstacleData obstacleData4 = new ObstacleData();
            obstacleData4.m_Pos = new PosData(-6.5F, 3.2F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData4);

            ObstacleData obstacleData5 = new ObstacleData();
            obstacleData5.m_Pos = new PosData(6.89F, 2.56F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData5);

            ObstacleData obstacleData6 = new ObstacleData();
            obstacleData6.m_Pos = new PosData(3.41F, -2.76F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData6);

            ObstacleData obstacleData7 = new ObstacleData();
            obstacleData7.m_Pos = new PosData(-3.7F, -4.27F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData7);

            ObstacleData obstacleData8 = new ObstacleData();
            obstacleData8.m_Pos = new PosData(-9.33F, -8.0F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData8);

            ObstacleData obstacleData9 = new ObstacleData();
            obstacleData9.m_Pos = new PosData(-4.79F, -10.0F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData9);

            ObstacleData obstacleData10 = new ObstacleData();
            obstacleData10.m_Pos = new PosData(-1.8F, -8.95F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData10);

            ObstacleData obstacleData11 = new ObstacleData();
            obstacleData11.m_Pos = new PosData(8.7F, -10.08F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData11);

            ObstacleData obstacleData12 = new ObstacleData();
            obstacleData12.m_Pos = new PosData(11.42F, -10.05F);
            BattleTerrainData.m_ObstacleDatas.Add(obstacleData12);
            #endregion
        }

        public void OnOnBattleMemberActionData(PLAYER_INDEX playerIndex, CSBattleMemberActionData data)
        {
            if (GetRoomState() == ROOM_STATE.PLAY)
            {
                if (m_BattleMembers.ContainsKey(playerIndex))
                    m_BattleMembers[playerIndex].MemberActionType = data.m_ActionType;
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

        public void SendWatingData(WAITING_TYPE waitingType, int count)
        {
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                member.GameSession.SendManager.SendSCBattleWatingData(new SCBattleWatingData() { m_WaitingType = waitingType, m_Count = count });
            }
        }

        public void InitBattleRoom()
        {
            m_GameTimeRemain = 60.0F;
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                member.Initialize();
                member.BattleMemberData.m_KillCount = 0;
            }
        }

        public void InitBattleMember()
        {
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                member.Initialize();
            }
        }

        public void UpdateBattleManager(double deltatime)
        {
            m_GameTimeRemain -= (float)deltatime;

            // 이동 처리
            foreach (BattleMember member in m_BattleMembers.Values)
            {
                m_BattleManager.UpdatePlayerTerrainMove(deltatime, member, BattleTerrainData);
            }

            if (m_BattleMembers.ContainsKey(PLAYER_INDEX.PLAYER_1) && m_BattleMembers.ContainsKey(PLAYER_INDEX.PLAYER_2))
                m_BattleManager.UpdatePlayersCollision(deltatime, m_BattleMembers[PLAYER_INDEX.PLAYER_1], m_BattleMembers[PLAYER_INDEX.PLAYER_2]);

            // 공격 판정
            PLAYER_INDEX loserPlayer = PLAYER_INDEX.NONE;
            if (MemberCount == MAX_MEMBER_COUNT && 
                m_BattleMembers.ContainsKey(PLAYER_INDEX.PLAYER_1) && 
                m_BattleMembers.ContainsKey(PLAYER_INDEX.PLAYER_2))
            {
                loserPlayer = m_BattleManager.UpdateGameResult(m_BattleMembers[PLAYER_INDEX.PLAYER_1], m_BattleMembers[PLAYER_INDEX.PLAYER_2]);
            }

            if (loserPlayer != PLAYER_INDEX.NONE)
            {
                // 승리 처리
                if (m_BattleMembers.ContainsKey(loserPlayer))
                {
                    m_BattleMembers[loserPlayer].MemberActionType = ACTION_TYPE.DIE;

                    switch (loserPlayer)
                    {
                        case PLAYER_INDEX.PLAYER_1:
                            m_BattleMembers[PLAYER_INDEX.PLAYER_2].BattleMemberData.m_KillCount++;
                            break;
                        case PLAYER_INDEX.PLAYER_2:
                            m_BattleMembers[PLAYER_INDEX.PLAYER_1].BattleMemberData.m_KillCount++;
                            break;
                    }
                }
            }
        }

        public ROOM_STATE GetRoomState() { lock (StateLock) return State; }

        public void ChangeBattleRoomState(ROOM_STATE state)
        {
            lock(StateLock)
            {
                switch (state)
                {
                    case ROOM_STATE.READY:
                        BattleRoomState = new BattleRoomReadyState(this);
                        break;
                    case ROOM_STATE.WAIT:
                        BattleRoomState = new BattleRoomWaitState(this);
                        break;
                    case ROOM_STATE.PLAY:
                        BattleRoomState = new BattleRoomPlayState(this);
                        break;
                    case ROOM_STATE.END:
                        BattleRoomState = new BattleRoomEndState(this);
                        break;
                    default:
                        BattleRoomState = null;
                        break;
                }

                State = state;
            }
        }

        public void Update(object state)
        {
            lock(state)
            {
                m_Time.Update();

                if(GetRoomState() == ROOM_STATE.PLAY)
                {
                    // 시간이 종료가 되거나 누군가 3승을 먼저 할 경우 
                    int player1KillCount = 0;
                    int player2KillCount = 0;

                    if (m_BattleMembers.ContainsKey(PLAYER_INDEX.PLAYER_1))
                        player1KillCount = m_BattleMembers[PLAYER_INDEX.PLAYER_1].BattleMemberData.m_KillCount;

                    if (m_BattleMembers.ContainsKey(PLAYER_INDEX.PLAYER_2))
                        player2KillCount = m_BattleMembers[PLAYER_INDEX.PLAYER_2].BattleMemberData.m_KillCount;

                    if (m_GameTimeRemain < 0 || player1KillCount == 3 || player2KillCount == 3)
                    {
                        ChangeBattleRoomState(ROOM_STATE.END);
                    }

                    foreach (BattleMember member in m_BattleMembers.Values)
                    {
                        if (member.MemberActionType == ACTION_TYPE.DIE)
                            ChangeBattleRoomState(ROOM_STATE.WAIT);
                    }
                }

                BattleRoomState.Update(m_Time.DeltaTime);

                Common.Packet.SCSyncBattleData syncBattleData = new Common.Packet.SCSyncBattleData();
                syncBattleData.m_Frame = m_Frame;
                syncBattleData.m_GameTimeRemain = (int)m_GameTimeRemain;

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

        public void JoinBattleRoom(GameSession session, out int roomIndex, out PLAYER_INDEX playerIndex, out BattleTerrainData battleTerrainData)
        {
            roomIndex         = m_RoomIndex;
            playerIndex       = (PLAYER_INDEX)MemberCount;
            battleTerrainData = BattleTerrainData;

            BattleMember member = new BattleMember(playerIndex, session);
            m_BattleMembers.TryAdd(member.PlayerIndex, member);
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
                m_Time = new Time();
                m_BattleRoomTimer = new System.Threading.Timer(Update, new object(), 0, 1000 / 60);
                ChangeBattleRoomState(ROOM_STATE.READY);
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
