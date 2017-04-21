using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Room.RoomState
{
    enum ROOM_STATE
    {
        NONE,
        READY,
        WAIT,
        PLAY,
        END
    }

    class BattleRoomState
    {
        public BattleRoom m_BattleRoom { get; private set; }

        public BattleRoomState(BattleRoom battleRoom)
        {
            m_BattleRoom = battleRoom;
        }

        public virtual void Initialize() { }

        public virtual void Update(double deltatime) { }
    }
}
