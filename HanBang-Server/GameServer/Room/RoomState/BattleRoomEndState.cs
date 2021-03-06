﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Room.RoomState
{
    class BattleRoomEndState : BattleRoomState
    {
        private double m_WaitTime = 3.0F;

        public BattleRoomEndState(BattleRoom battleRoom) : base(battleRoom)
        {
        }

        public override void Update(double deltatime)
        {
            // 테스트 버전의 경우는 모든 룸 상태 및 유저 정보 초기화 후
            // 다시 Ready 상태로 변경 함
            m_WaitTime -= deltatime;
            if (m_WaitTime <= 0.0F)
            {
                m_BattleRoom.InitBattleRoom();
                m_BattleRoom.ChangeBattleRoomState(ROOM_STATE.READY);
            }
        }
    }
}
