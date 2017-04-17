using GameServer.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class BattleManager
    {
        private const float MIN_PLAYER_POSX = -19.1F;
        private const float MIN_PLAYER_POSY = -18.4F;

        private const float MAX_PLAYER_POSX = 17.8F;
        private const float MAX_PLAYER_POSY = 17.9F;

        private const float PLAYER_SPEED = 0.5F;

        private const int PLAYER1_INDEX = 0;
        private const int PLAYER2_INDEX = 1;

        public void Update(BattleMember member)
        {
            // 매 프레임 마다 위치를 이동 시킨다.
            UpdatePlayerPos(member);
        }

        public void UpdatePlayerPos(BattleMember member)
        {
            PosData pos = new PosData(member.m_PlayerPos);

            // 우선 이동 처리를 한다.
            switch (member.PlayerMoveType)
            {
                case MOVE_TYPE.LEFT:
                    {
                        pos.m_X -= PLAYER_SPEED;
                    }
                    break;
                case MOVE_TYPE.RIGHT:
                    {
                        pos.m_X += PLAYER_SPEED;
                    }
                    break;
                case MOVE_TYPE.DOWN:
                    {
                        pos.m_Y -= PLAYER_SPEED;
                    }
                    break;
                case MOVE_TYPE.UP:
                    {
                        pos.m_Y += PLAYER_SPEED;
                    }
                    break;
            }

            // 맵 충돌 체크를 한다.
            if (pos.m_X > MIN_PLAYER_POSX &&
                pos.m_X < MAX_PLAYER_POSX &&
                pos.m_Y < MAX_PLAYER_POSY &&
                pos.m_Y > MIN_PLAYER_POSY)
            {
                member.m_PlayerPos.m_X = pos.m_X;
                member.m_PlayerPos.m_Y = pos.m_Y;
            }
        }
    }
}
