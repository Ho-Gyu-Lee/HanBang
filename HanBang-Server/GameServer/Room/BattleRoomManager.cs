using GameServer.Common.Util;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace GameServer.Room
{
    class BattleRoomManager : ThreadSafeSingleton<BattleRoomManager>
    {
        private ConcurrentDictionary<int, BattleRoom> m_BattleRooms = new ConcurrentDictionary<int, BattleRoom>();

        private ConcurrentQueue<int> m_BattleRoomIndexs = new ConcurrentQueue<int>();

        public BattleRoomManager()
        {
            // 2000개 방 생성 가능
            for(int i = 0; i < 2000; ++i)
            {
                m_BattleRoomIndexs.Enqueue(i);
            }
        }

        public BattleRoom GetBattleRoom(int roomIndex)
        {
            BattleRoom battleRoom = null;
            if(m_BattleRooms.ContainsKey(roomIndex))
            {
                battleRoom = m_BattleRooms[roomIndex];
            }
            return battleRoom;
        }

        public void MatchBattleRoom(GameSession gameUserSession, ref int roomIndex, ref int playerIndex)
        {
            BattleRoom battleRoom = m_BattleRooms.Where(rooms => rooms.Value.MemberCount == 1).FirstOrDefault().Value;

            if(battleRoom != null)
            {
                battleRoom.JoinBattleRoom(gameUserSession, out roomIndex, out playerIndex);
                return;
            }

            if(!m_BattleRoomIndexs.TryDequeue(out roomIndex))
            {
                Console.WriteLine("Failed Battle Room Index");
                return;
            }

            if(roomIndex < 0)
            {
                Console.WriteLine("Failed Battle Room Index");
                return;
            }

            battleRoom = new BattleRoom(roomIndex);
            if (!m_BattleRooms.TryAdd(roomIndex, battleRoom))
            {
                Console.WriteLine("Failed Create Battle Room");
                return;
            }

            battleRoom.JoinBattleRoom(gameUserSession, out roomIndex, out playerIndex);
        }

        public void CloseBattle(int roomIndex)
        {
            BattleRoom battleRoom = null;
            if(!m_BattleRooms.TryRemove(roomIndex, out battleRoom))
            {
                Console.WriteLine("Not Remove Battle Room");
                return;
            }

            if(battleRoom == null)
            {
                Console.WriteLine("Null of Battle Room");
                return;
            }

            battleRoom.CloseBattle();
        }
    }
}
