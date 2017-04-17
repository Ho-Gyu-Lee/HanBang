using GameServer.Battle;
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
        private System.Threading.Timer m_BattleRoomTimer = null;

        private ConcurrentDictionary<string, BattleMember> m_BattleMembers = new ConcurrentDictionary<string, BattleMember>();

        private BattleManager m_BattleManager = new BattleManager();

        public int m_RoomIndex = -1;

        public int MemberCount { get { return m_BattleMembers.Count; } }

        public BattleRoom(int roomIndex)
        {
            m_RoomIndex = roomIndex;
            m_BattleRoomTimer = new System.Threading.Timer(Update, new object(), 0, 1000 / 60);
        }

        public void Update(object state)
        {
            lock(state)
            {
                foreach(BattleMember member in m_BattleMembers.Values)
                {
                    m_BattleManager.Update(member);
                }
            }
        }

        public void JoinBattleRoom(GameUserSession gameUserSession)
        {
            //m_BattleMembers.TryAdd(sessionID, new BattleMember(sessionID));
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
    }
}
