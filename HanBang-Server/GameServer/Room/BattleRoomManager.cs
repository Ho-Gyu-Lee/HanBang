using GameServer.Common.Packet;
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
            if (m_BattleRooms.ContainsKey(roomIndex))
            {
                battleRoom = m_BattleRooms[roomIndex];
            }
            return battleRoom;
        }

        public void MatchBattleRoom(GameSession gameUserSession, ref int roomIndex, ref PLAYER_INDEX playerIndex, ref BattleTerrainData battleTerrainData)
        {
            BattleRoom battleRoom = m_BattleRooms.Where(rooms => rooms.Value.MemberCount == 1).FirstOrDefault().Value;

            if (battleRoom != null)
            {
                battleRoom.JoinBattleRoom(gameUserSession, out roomIndex, out playerIndex, out battleTerrainData);
                return;
            }

            if (!m_BattleRoomIndexs.TryDequeue(out roomIndex))
            {
                Console.WriteLine("Failed Battle Room Index");
                return;
            }

            if (roomIndex < 0)
            {
                Console.WriteLine("Failed Battle Room Index");
                return;
            }

            battleRoom = new BattleRoom(roomIndex, 0);
            if (!m_BattleRooms.TryAdd(roomIndex, battleRoom))
            {
                Console.WriteLine("Failed Create Battle Room");
                return;
            }

            battleRoom.JoinBattleRoom(gameUserSession, out roomIndex, out playerIndex, out battleTerrainData);
        }

        public void LeaveBattleRoom(int roomIndex, PLAYER_INDEX playerIndex)
        {
            if(m_BattleRooms.ContainsKey(roomIndex))
            {
                if(m_BattleRooms[roomIndex].LeaveBattleRoom(playerIndex))
                {
                    BattleRoom battleRoom = null;
                    if (!m_BattleRooms.TryRemove(roomIndex, out battleRoom))
                    {
                        Console.WriteLine("Not Remove Battle Room");
                        return;
                    }
                }
            }
        }
    }
}
