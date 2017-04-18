using GameServer.Battle;
using GameServer.Common.Packet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private int m_Frame = 0;

        public int m_RoomIndex = -1;

        public int MemberCount { get { return m_BattleMembers.Count; } }

        public BattleRoom(int roomIndex)
        {
            m_RoomIndex = roomIndex;
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
                    syncBattleData.m_BattleMemberDatas.Add(member.PlayerIndex, new Common.Packet.BattleMemberData()
                    {
                        m_PlayerIndex = member.PlayerIndex,
                        IsDie         = false,
                        m_MoveType    = member.PlayerMoveType,
                        m_Pos         = member.m_PlayerPos
                    });
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

            int playerIndex = MemberCount;

            BattleMember member = new BattleMember(MemberCount, session);

            m_BattleMembers.TryAdd(playerIndex, member);
            m_BattleManager.SetBattleMember(playerIndex, member);

            SCBattleMemberSpawnData data = new SCBattleMemberSpawnData();
            data.m_MyPlayerIndex = member.PlayerIndex;
            foreach (BattleMember battleMember in m_BattleMembers.Values)
            {
                data.m_BattleMemberDatas.Add(battleMember.PlayerIndex, new Common.Packet.BattleMemberData()
                {
                    m_PlayerIndex = battleMember.PlayerIndex,
                    IsDie         = false,
                    m_MoveType    = battleMember.PlayerMoveType,
                    m_Pos         = battleMember.m_PlayerPos
                });
            }

            foreach (BattleMember battleMember in m_BattleMembers.Values)
            {
                member.GameSession.SendManager.SendSCBattleMemberSpawnData(data);
            }

            if(MemberCount == MAX_MEMBER_COUNT)
            {
                if (m_BattleRoomTimer == null)
                    m_BattleRoomTimer = new System.Threading.Timer(Update, new object(), 0, 1000 / 60);
            }
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
                m_BattleMembers[playerIndex].PlayerMoveType = moveType;
            }
        }

        public void Attack(int playerIndex)
        {
            m_BattleManager.Attack(playerIndex);
        }
    }
}
