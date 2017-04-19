using GameServer.Common.Packet;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{
    class BattleManager
    {
        private const float MIN_PLAYER_POSX = -19.0F;
        private const float MIN_PLAYER_POSY = -18.0F;

        private const float MAX_PLAYER_POSX = 17.8F;
        private const float MAX_PLAYER_POSY = 17.9F;

        private const float PLAYER_SPEED = 0.25F;

        private const float PLAYER_COLLISION_BOX_X = 1.5F;
        private const float PLAYER_COLLISION_BOX_Y = 2.0F;

        private BattleMember m_Member1 = null;
        private BattleMember m_Member2 = null;

        private Random m_BattleRandom = new Random(Guid.NewGuid().GetHashCode());

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

        public void Update()
        {
            // 매 프레임 마다 위치를 이동 시킨다.
            UpdatePlayerMapMove(m_Member1);
            UpdatePlayerMapMove(m_Member2);

            // 캐릭터 충돌 체크
            UpdatePlayersCollision(m_Member1, m_Member2);
        }

        public bool UpdateGameResult()
        {
            if (m_Member1 != null && m_Member2 != null)
            {
                if(m_Member1.MemberActionType == ACTION_TYPE.ATTACK)
                {
                    return ExaminePlayerDamage(m_Member1.MemberLook, m_Member1.MemberPos, m_Member2.MemberPos);
                }
                else if(m_Member2.MemberActionType == ACTION_TYPE.ATTACK)
                {
                    return ExaminePlayerDamage(m_Member2.MemberLook, m_Member2.MemberPos, m_Member1.MemberPos);
                }
                else if(m_Member1.MemberActionType == ACTION_TYPE.ATTACK && m_Member2.MemberActionType == ACTION_TYPE.ATTACK)
                {
                    bool member1Damage = ExaminePlayerDamage(m_Member1.MemberLook, m_Member1.MemberPos, m_Member2.MemberPos);
                    bool member2Damage = ExaminePlayerDamage(m_Member2.MemberLook, m_Member2.MemberPos, m_Member1.MemberPos);

                    if(member1Damage && member2Damage)
                    {
                        // 둘다 데미지를 받았다면
                    }
                    else if(member1Damage)
                    {
                        // 1 번 유저가 데미지 입음
                    }
                    else if(member2Damage)
                    {
                        // 2번 유저가 데미지 입음
                    }
                }
            }

            return false;
        }

        private bool ExaminePlayerDamage(bool look, PosData attack, PosData hit)
        {
            float collisionBoxX = 0.0F;
            if (look)
            {
                // 왼쪽에 유저가 존재 하는가?
                collisionBoxX = attack.m_X - (PLAYER_COLLISION_BOX_X * 2);
                if (collisionBoxX - PLAYER_COLLISION_BOX_X < hit.m_X &&
                    collisionBoxX + PLAYER_COLLISION_BOX_X > hit.m_X)
                {
                    return true;
                }
            }
            else
            {
                // 오른쪽에 유저가 존재 하는가?
                collisionBoxX = attack.m_X + (PLAYER_COLLISION_BOX_X * 2);
                if (collisionBoxX - PLAYER_COLLISION_BOX_X < hit.m_X &&
                    collisionBoxX + PLAYER_COLLISION_BOX_X > hit.m_X)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdatePlayerMapMove(BattleMember member)
        {
            if(member != null)
            {
                // 현재 위치를 얻어 온다.
                PosData pos = new PosData(member.MemberPos);

                // 미리 이동 시켜 본다.
                PlayerNextPosData(member.MemberActionType, ref member.MemberLook, ref pos);

                // 맵 충돌 체크를 한다.
                if (pos.m_X > MIN_PLAYER_POSX && pos.m_X < MAX_PLAYER_POSX &&
                    pos.m_Y < MAX_PLAYER_POSY && pos.m_Y > MIN_PLAYER_POSY)
                {
                    member.MemberPos.m_X = pos.m_X;
                    member.MemberPos.m_Y = pos.m_Y;
                }
            }
        }

        private void UpdatePlayersCollision(BattleMember member1, BattleMember member2)
        {
            if(member1 != null && member2 != null)
            {
                if(member1.MemberPos.m_X - PLAYER_COLLISION_BOX_X < member2.MemberPos.m_X && 
                   member1.MemberPos.m_X + PLAYER_COLLISION_BOX_X > member2.MemberPos.m_X &&
                   member1.MemberPos.m_Y - PLAYER_COLLISION_BOX_Y < member2.MemberPos.m_Y &&
                   member1.MemberPos.m_Y + PLAYER_COLLISION_BOX_Y > member2.MemberPos.m_Y)
                {
                    if(member1.MemberActionType == ACTION_TYPE.NONE)
                    {
                        PlayerPrevPosData(member2.MemberActionType, ref member2.MemberPos.m_X, ref member2.MemberPos.m_Y);
                    }
                    else if(member2.MemberActionType == ACTION_TYPE.NONE)
                    {
                        PlayerPrevPosData(member1.MemberActionType, ref member1.MemberPos.m_X, ref member1.MemberPos.m_Y);
                    }
                    else
                    {
                        int result = m_BattleRandom.Next(0, 1);
                        if (result == 0)
                            PlayerPrevPosData(member2.MemberActionType, ref member2.MemberPos.m_X, ref member2.MemberPos.m_Y);
                        else
                            PlayerPrevPosData(member1.MemberActionType, ref member1.MemberPos.m_X, ref member1.MemberPos.m_Y);
                    }
                }
            }
        }

        private void PlayerNextPosData(ACTION_TYPE actionType, ref bool look, ref PosData pos)
        {
            switch (actionType)
            {
                case ACTION_TYPE.LEFT:
                    {
                        look = true;
                        pos.m_X -= PLAYER_SPEED;
                    }
                    break;
                case ACTION_TYPE.RIGHT:
                    {
                        look = false;
                        pos.m_X += PLAYER_SPEED;
                    }
                    break;
                case ACTION_TYPE.DOWN:
                    {
                        pos.m_Y -= PLAYER_SPEED;
                    }
                    break;
                case ACTION_TYPE.UP:
                    {
                        pos.m_Y += PLAYER_SPEED;
                    }
                    break;
            }
        }

        private void PlayerPrevPosData(ACTION_TYPE actionType, ref float x, ref float y)
        {
            switch (actionType)
            {
                case ACTION_TYPE.LEFT:
                    {
                        x += PLAYER_SPEED;
                    }
                    break;
                case ACTION_TYPE.RIGHT:
                    {
                        x -= PLAYER_SPEED;
                    }
                    break;
                case ACTION_TYPE.DOWN:
                    {
                        y += PLAYER_SPEED;
                    }
                    break;
                case ACTION_TYPE.UP:
                    {
                        y -= PLAYER_SPEED;
                    }
                    break;
            }
        }
    }
}
