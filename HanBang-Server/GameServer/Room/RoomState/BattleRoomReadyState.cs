using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Room.RoomState
{
    class BattleRoomReadyState : BattleRoomState
    {
        private double m_WaitTime = 3.0F;

        public BattleRoomReadyState(BattleRoom battleRoom) : base(battleRoom)
        {
        }

        public override void Update(double deltatime)
        {
            // 3초 후에 게임 시작
            m_WaitTime -= deltatime;
            if (m_WaitTime <= 0.0F)
            {
                // 이 상태는 무조건 처음 한번만 호출 해야 함
                m_BattleRoom.ChangeBattleRoomState(ROOM_STATE.PLAY);
            }
        }
    }
}
