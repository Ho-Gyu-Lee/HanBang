using GameServer.Common.Packet;
using System.Collections.Generic;

namespace GameServer.Battle
{
    class BattleManager
    {
        private const float MIN_PLAYER_POSX = -19.0F;
        private const float MIN_PLAYER_POSY = -18.4F;

        private const float MAX_PLAYER_POSX = 17.8F;
        private const float MAX_PLAYER_POSY = 17.9F;

        private const float PLAYER_SPEED = 0.3F;

        private BattleMember m_Member1 = null;
        private BattleMember m_Member2 = null;

        private bool IsMember1Attack = false;
        private bool IsMember2Attack = false;

        public void SetBattleMember(int playerIndex, BattleMember member)
        {
            switch(playerIndex)
            {
                case 0:
                    m_Member1 = member;
                    break;
                case 1:
                    m_Member2 = member;
                    break;
            }
        }

        public void Attack(int playerIndex)
        {
            switch (playerIndex)
            {
                case 0:
                    IsMember1Attack = true;
                    break;
                case 1:
                    IsMember2Attack = true;
                    break;
            }
        }

        public void Update()
        {
            // 매 프레임 마다 위치를 이동 시킨다.
            if(IsMember1Attack == false)
                UpdatePlayerMovePos(m_Member1);

            if (IsMember2Attack == false)
                UpdatePlayerMovePos(m_Member2);
        }

        private void UpdatePlayerMovePos(BattleMember member)
        {
            if(member != null)
            {
                PosData pos = new PosData(member.MemberPos);

                // 미리 이동 시켜 본다.
                PlayerNextPosData(member.MemberMoveType, ref pos);

                // 우선 맵 충돌 체크를 한다.
                bool isMemberMapCollision = CheckMapCollision(pos);

                // 맵 안에 존재 하면 캐릭터 끼리 충돌 체크를 한다.
                if (isMemberMapCollision)
                {
                    member.MemberPos.m_X = pos.m_X;
                    member.MemberPos.m_Y = pos.m_Y;
                }

                // 모두 이상이 없다면 최종적으로 값을 변경 한다.
            }
        }

        private bool CheckMapCollision(PosData pos)
        {
            if (pos.m_X > MIN_PLAYER_POSX &&
                pos.m_X < MAX_PLAYER_POSX &&
                pos.m_Y < MAX_PLAYER_POSY &&
                pos.m_Y > MIN_PLAYER_POSY)
            {
                return true;
            }

            return false;
        }

        private void PlayerNextPosData(MOVE_TYPE moveType, ref PosData pos)
        {
            // 우선 이동 처리를 한다.
            switch (moveType)
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
        }
    }
}
