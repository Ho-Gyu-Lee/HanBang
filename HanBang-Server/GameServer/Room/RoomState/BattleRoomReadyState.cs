using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Room.RoomState
{
    class BattleRoomReadyState : BattleRoomState
    {
        public BattleRoomReadyState(BattleRoom battleRoom) : base(battleRoom)
        {
        }

        public override void Update()
        {
            // 3초 후에 게임 시작

            // 이 상태는 무조건 처음 한번만 호출 해야 함
        }
    }
}
