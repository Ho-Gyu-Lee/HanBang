using GameServer.Common.Packet;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{
    class BattleManager
    {
        private const float PLAYER_SPEED = 7.0F;

        private const float PLAYER_COLLISION_BOX_X = 1.3F;
        private const float PLAYER_COLLISION_BOX_Y = 1.3F;

        private const float OBSTACLE_COLLISION_BOX = 1.3F;

        private Random m_BattleRandom = new Random(Guid.NewGuid().GetHashCode());

        public PLAYER_INDEX UpdateGameResult(PosData member1Pos, ACTION_TYPE member1Action, PosData member2Pos, ACTION_TYPE member2Action)
        {
            if (member1Pos != null && member2Pos != null)
            {
                if (member1Action == ACTION_TYPE.ATTACK)
                {
                    if (ExaminePlayerDamage(member1Action, member1Pos, member2Pos))
                        return PLAYER_INDEX.PLAYER_2;
                }
                else if (member2Action == ACTION_TYPE.ATTACK)
                {
                    if (ExaminePlayerDamage(member2Action, member2Pos, member1Pos))
                        return PLAYER_INDEX.PLAYER_1;
                }
                else if (member1Action == ACTION_TYPE.ATTACK && member2Action == ACTION_TYPE.ATTACK)
                {
                    bool member1Damage = ExaminePlayerDamage(member1Action, member1Pos, member2Pos);
                    bool member2Damage = ExaminePlayerDamage(member2Action, member2Pos, member1Pos);

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

        private bool ExaminePlayerDamage(ACTION_TYPE action, PosData attack, PosData hit)
        {
            PosData attackCollisionBox = null;
            switch(action)
            {
                case ACTION_TYPE.LEFT:
                    attackCollisionBox = new PosData(attack.m_X - (0.5F * 2), attack.m_Y);
                    break;

                case ACTION_TYPE.RIGHT:
                    attackCollisionBox = new PosData(attack.m_X + (0.5F * 2), attack.m_Y);
                    break;
            }

            if (attackCollisionBox.m_X - 0.7F <= hit.m_X &&
                attackCollisionBox.m_X + 0.7F >= hit.m_X &&
                attackCollisionBox.m_Y - 0.6F <= hit.m_Y &&
                attackCollisionBox.m_Y + 0.6F >= hit.m_Y)
            {
                return true;
            }

            return false;
        }

        public void UpdatePlayerTerrainMove(double deltatime, ACTION_TYPE action, PosData pos, BattleTerrainData terrainData)
        {
            if (pos != null)
            {
                // 현재 위치를 얻어 온다.
                PosData copyPos = new PosData(pos);

                // 미리 이동 시켜 본다.
                PlayerNextPosData((float)deltatime, action, ref copyPos);

                // 맵 충돌 체크를 한다. 
                if (copyPos.m_X >= terrainData.m_MinSizeX && copyPos.m_X <= terrainData.m_MaxSizeX &&
                    copyPos.m_Y <= terrainData.m_MaxSizeY - 1.5F && copyPos.m_Y >= terrainData.m_MinSizeY)
                {
                    // 장애물 체크도 한다.
                    foreach (ObstacleData data in terrainData.m_ObstacleDatas)
                    {
                        if (data.m_Pos.m_X - OBSTACLE_COLLISION_BOX <= copyPos.m_X &&
                            data.m_Pos.m_X + OBSTACLE_COLLISION_BOX >= copyPos.m_X &&
                            data.m_Pos.m_Y - 1.5F <= copyPos.m_Y &&
                            data.m_Pos.m_Y + 0.5F >= copyPos.m_Y)
                        {
                            return;
                        }
                    }

                    pos.m_X = copyPos.m_X;
                    pos.m_Y = copyPos.m_Y;
                }
            }
        }

        public void UpdatePlayersCollision(double deltatime, ACTION_TYPE member1Action, PosData member1Pos, ACTION_TYPE member2Action, PosData member2Pos)
        {
            if (member1Pos != null && member2Pos != null)
            {
                if (member1Pos.m_X - PLAYER_COLLISION_BOX_X <= member2Pos.m_X && 
                    member1Pos.m_X + PLAYER_COLLISION_BOX_X >= member2Pos.m_X &&
                    member1Pos.m_Y - PLAYER_COLLISION_BOX_Y <= member2Pos.m_Y &&
                    member1Pos.m_Y + PLAYER_COLLISION_BOX_Y >= member2Pos.m_Y)
                {
                    if (member1Action == ACTION_TYPE.NONE)
                    {
                        PlayerPrevPosData((float)deltatime, member2Action, ref member2Pos);
                    }
                    else if (member2Action == ACTION_TYPE.NONE)
                    {
                        PlayerPrevPosData((float)deltatime, member1Action, ref member1Pos);
                    }
                    else
                    {
                        int result = m_BattleRandom.Next(0, 1);
                        if (result == 0)
                            PlayerPrevPosData((float)deltatime, member2Action, ref member2Pos);
                        else
                            PlayerPrevPosData((float)deltatime, member1Action, ref member1Pos);
                    }
                }
            }
        }

        private void PlayerNextPosData(float deltatime, ACTION_TYPE actionType, ref PosData pos)
        {
            switch (actionType)
            {
                case ACTION_TYPE.LEFT:
                    {
                        pos.m_X -= PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.RIGHT:
                    {
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

        private void PlayerPrevPosData(float deltatime, ACTION_TYPE actionType, ref PosData pos)
        {
            switch (actionType)
            {
                case ACTION_TYPE.LEFT:
                    {
                        pos.m_X += PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.RIGHT:
                    {
                        pos.m_X -= PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.DOWN:
                    {
                        pos.m_Y += PLAYER_SPEED * deltatime;
                    }
                    break;
                case ACTION_TYPE.UP:
                    {
                        pos.m_Y -= PLAYER_SPEED * deltatime;
                    }
                    break;
            }
        }
    }
}
