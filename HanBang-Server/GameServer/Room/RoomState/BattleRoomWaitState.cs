using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Room.RoomState
{
    class BattleRoomWaitState : BattleRoomState
    {
        public BattleRoomWaitState(BattleRoom battleRoom) : base(battleRoom)
        {
        }

        public override void Update()
        {
            // 현재 시간 멈춤

            // 3초 후에 게임 다시 시작

            // 유저 위치 초기화

            // 유저 액션 초기화

            // 게임 상태 플레이로 변경
        }
    }
}
