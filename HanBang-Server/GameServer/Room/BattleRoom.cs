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

        private int m_Frame = 0;

        public int m_RoomIndex = -1;

        public int MemberCount { get { return m_BattleMembers.Count; } }

        public BattleRoom(int roomIndex)
        {
            m_RoomIndex = roomIndex;
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

                Common.Packet.SCSyncBattleData syncBattleData = new Common.Packet.SCSyncBattleData();
                syncBattleData.m_Frame = m_Frame;
                foreach (BattleMember member in m_BattleMembers.Values)
                {
                    syncBattleData.m_BattleMemberDatas.Add(member.PlayerIndex, member.BattleMemberData);
                }

                foreach (BattleMember member in m_BattleMembers.Values)
                {
                    member.GameSession.SendManager.SendSCSyncBattleData(syncBattleData);
                }

                m_Frame++;
            }
        }

        public void JoinBattleRoom(GameSession session, out int roomIndex)
        {
            roomIndex   = m_RoomIndex;

            BattleMember member = new BattleMember(MemberCount, session);
            m_BattleMembers.TryAdd(member.PlayerIndex, member);
            m_BattleManager.SetBattleMember(member.PlayerIndex, member);

            //if(MemberCount == MAX_MEMBER_COUNT)
            //{
                if (m_BattleRoomTimer == null)
                    m_BattleRoomTimer = new System.Threading.Timer(Update, new object(), 0, 1000 / 60);
            //}
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

        public void ChangeMemberMoveType(int playerIndex, MOVE_TYPE moveType)
        {
            if (m_BattleMembers.ContainsKey(playerIndex))
            {
                m_BattleMembers[playerIndex].MemberMoveType = moveType;
            }
        }

        public void Attack(int playerIndex)
        {
            m_BattleManager.Attack(playerIndex);
        }
    }
}
