using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Room.RoomState
{
    class BattleRoomWaitState : BattleRoomState
    {
        private double m_WaitTime = 3.0F;

        public BattleRoomWaitState(BattleRoom battleRoom) : base(battleRoom)
        {
        }

        public override void Update(double deltatime)
        {
            // 3초 후에 게임 다시 시작
            m_WaitTime -= deltatime;
            if(m_WaitTime <= 0.0F)
            {
                // 유저 위치 초기화

                // 유저 액션 초기화

                // 게임 상태 플레이로 변경
            }
        }
    }
}
