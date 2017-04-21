using GameServer.Common.Packet;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{
    class BattleManager
    {
        private const float PLAYER_SPEED = 0.25F;

        private const float PLAYER_COLLISION_BOX_X = 1.3F;
        private const float PLAYER_COLLISION_BOX_Y = 1.3F;

        private const float OBSTACLE_COLLISION_BOX = 1.3F;

        private Random m_BattleRandom = new Random(Guid.NewGuid().GetHashCode());

        public PLAYER_INDEX UpdateGameResult(BattleMember member1, BattleMember member2)
        {
            if (member1 != null && member2 != null)
            {
                if (member1.MemberActionType == ACTION_TYPE.ATTACK)
                {
                    if (ExaminePlayerDamage(member1.MemberLook, member1.MemberPos, member2.MemberPos))
                        return PLAYER_INDEX.PLAYER_2;
                }
                else if (member2.MemberActionType == ACTION_TYPE.ATTACK)
                {
                    if (ExaminePlayerDamage(member2.MemberLook, member2.MemberPos, member1.MemberPos))
                        return PLAYER_INDEX.PLAYER_1;
                }
                else if (member1.MemberActionType == ACTION_TYPE.ATTACK && member2.MemberActionType == ACTION_TYPE.ATTACK)
                {
                    bool member1Damage = ExaminePlayerDamage(member1.MemberLook, member1.MemberPos, member2.MemberPos);
                    bool member2Damage = ExaminePlayerDamage(member2.MemberLook, member2.MemberPos, member1.MemberPos);

                    if (member1Damage && member2Damage)
                    {
                        // 둘다 데미지를 받았다면
                        int result = m_BattleRandom.Next(0, 1);
                        if (result == 0)
                            return PLAYER_INDEX.PLAYER_2;
                        else
                            return PLAYER_INDEX.PLAYER_1;
                    }
                    else if (member1Damage)
                        return PLAYER_INDEX.PLAYER_1;
                    else if (member2Damage)
                        return PLAYER_INDEX.PLAYER_2;
                }
            }

            return PLAYER_INDEX.NONE;
        }

        private bool ExaminePlayerDamage(bool look, PosData attack, PosData hit)
        {
            PosData attackCollisionBox = null;
            if (look)
            {
                attackCollisionBox = new PosData(attack.m_X - (PLAYER_COLLISION_BOX_X * 2), attack.m_Y);
            }
            else
            {
                attackCollisionBox = new PosData(attack.m_X + (PLAYER_COLLISION_BOX_X * 2), attack.m_Y);
            }

            if (attackCollisionBox.m_X - PLAYER_COLLISION_BOX_X <= hit.m_X &&
                attackCollisionBox.m_X + PLAYER_COLLISION_BOX_X >= hit.m_X &&
                attackCollisionBox.m_Y - PLAYER_COLLISION_BOX_Y <= hit.m_Y &&
                attackCollisionBox.m_Y + PLAYER_COLLISION_BOX_Y >= hit.m_Y)
            {
                return true;
            }

            return false;
        }

        public void UpdatePlayerTerrainMove(double deltatime, BattleMember member, BattleTerrainData terrainData)
        {
            if (member != null)
            {
                // 현재 위치를 얻어 온다.
                PosData pos = new PosData(member.MemberPos);

                // 미리 이동 시켜 본다.
                PlayerNextPosData((float)deltatime, member.MemberActionType, ref member.MemberLook, ref pos);

                // 맵 충돌 체크를 한다. 
                if (pos.m_X >= terrainData.m_MinSizeX && pos.m_X <= terrainData.m_MaxSizeX &&
                    pos.m_Y <= terrainData.m_MaxSizeY - 1.5F && pos.m_Y >= terrainData.m_MinSizeY)
                {
                    // 장애물 체크도 한다.
                    foreach (ObstacleData data in terrainData.m_ObstacleDatas)
                    {
                        if (data.m_Pos.m_X - OBSTACLE_COLLISION_BOX <= pos.m_X &&
                            data.m_Pos.m_X + OBSTACLE_COLLISION_BOX >= pos.m_X &&
                            data.m_Pos.m_Y - 1.5F <= pos.m_Y &&
                            data.m_Pos.m_Y + 0.5F >= pos.m_Y)
                        {
                            return;
                        }
                    }

                    member.MemberPos.m_X = pos.m_X;
                    member.MemberPos.m_Y = pos.m_Y;
                }
            }
        }

        public void UpdatePlayersCollision(double deltatime, BattleMember member1, BattleMember member2)
        {
            if (member1 != null && member2 != null)
            {
                if (member1.MemberPos.m_X - PLAYER_COLLISION_BOX_X <= member2.MemberPos.m_X && 
                    member1.MemberPos.m_X + PLAYER_COLLISION_BOX_X >= member2.MemberPos.m_X &&
                    member1.MemberPos.m_Y - PLAYER_COLLISION_BOX_Y <= member2.MemberPos.m_Y &&
                    member1.MemberPos.m_Y + PLAYER_COLLISION_BOX_Y >= member2.MemberPos.m_Y)
                {
                    if (member1.MemberActionType == ACTION_TYPE.NONE)
                    {
                        PlayerPrevPosData((float)deltatime, member2.MemberActionType, ref member2.MemberPos.m_X, ref member2.MemberPos.m_Y);
                    }
                    else if (member2.MemberActionType == ACTION_TYPE.NONE)
                    {
                        PlayerPrevPosData((float)deltatime, member1.MemberActionType, ref member1.MemberPos.m_X, ref member1.MemberPos.m_Y);
                    }
                    else
                    {
                        int result = m_BattleRandom.Next(0, 1);
                        if (result == 0)
                            PlayerPrevPosData((float)deltatime, member2.MemberActionType, ref member2.MemberPos.m_X, ref member2.MemberPos.m_Y);
                        else
                            PlayerPrevPosData((float)deltatime, member1.MemberActionType, ref member1.MemberPos.m_X, ref member1.MemberPos.m_Y);
                    }
                }
            }
        }

        private void PlayerNextPosData(float deltatime, ACTION_TYPE actionType, ref bool look, ref PosData pos)
        {
            switch (actionType)
            {
                case ACTION_TYPE.LEFT:
                    {
                        look = true;
                        pos.m_X -= PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.RIGHT:
                    {
                        look = false;
                        pos.m_X += PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.DOWN:
                    {
                        pos.m_Y -= PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.UP:
                    {
                        pos.m_Y += PLAYER_SPEED * deltatime;
                    }
                    break;
            }
        }

        private void PlayerPrevPosData(float deltatime, ACTION_TYPE actionType, ref float x, ref float y)
        {
            switch (actionType)
            {
                case ACTION_TYPE.LEFT:
                    {
                        x += PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.RIGHT:
                    {
                        x -= PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.DOWN:
                    {
                        y += PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.UP:
                    {
                        y -= PLAYER_SPEED * deltatime;
                    }
                    break;
            }
        }
    }
}
