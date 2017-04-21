using GameServer.Battle;
using GameServer.Common.Packet;

namespace GameServer.Room.RoomState
{
    class BattleRoomPlayState : BattleRoomState
    {
        public BattleRoomPlayState(BattleRoom battleRoom) : base(battleRoom)
        {
        }

        public override void Update()
        {
            m_BattleRoom.UpdateBattleManager();
        }
    }
}
