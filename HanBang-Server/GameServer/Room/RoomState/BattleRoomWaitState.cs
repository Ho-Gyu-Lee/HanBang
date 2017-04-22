using GameServer.Common.Packet;
using System;

namespace GameServer.Room.RoomState
{
    class BattleRoomWaitState : BattleRoomState
    {
        private double m_WaitTime = 3.0F;

        private int m_Count = 0;

        public BattleRoomWaitState(BattleRoom battleRoom) : base(battleRoom)
        {
        }

        public override void Update(double deltatime)
        {
            // 3초 후에 게임 다시 시작
            m_WaitTime -= deltatime;
            if(m_WaitTime <= 0.0F)
            {
                m_BattleRoom.SendWatingData(WAITING_TYPE.RE_START_BATTLE, m_Count);

                // 유저 초기화
                m_BattleRoom.InitBattleMember();

                // 게임 상태 플레이로 변경
                m_BattleRoom.ChangeBattleRoomState(ROOM_STATE.PLAY);
            }
        }
    }
}
