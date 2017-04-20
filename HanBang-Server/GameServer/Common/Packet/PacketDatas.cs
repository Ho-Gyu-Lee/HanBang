﻿using MsgPack.Serialization;
using System.Collections.Generic;

namespace GameServer.Common.Packet
{
    [MessagePackEnum]
    public enum PACKET_TYPE
    {
        NONE = 1000,
        CS_BATTLE_MEMBER_ACTION_DATA,
        SC_SYNC_BATTLE,
        CS_MATCH_BATTLE_ROOM,
        SC_MATCH_BATTLE_ROOM,
        CS_BATTLE_MEMBER_DATA,
        SC_BATTLE_MEMBER_DATA,
        CS_READY_BATTLE,
        SC_START_BATTLE,
        SC_END_BATTLE,
        CS_LEAVE_BATTLE_ROOM,
    }

    [MessagePackEnum]
    public enum ACTION_TYPE
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN,
        ATTACK,
        DIE,
    }

    [MessagePackEnum]
    public enum PLAYER_INDEX
    {
        NONE     = -1,
        PLAYER_1 = 0,
        PLAYER_2 = 1,
    }

    public class PosData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public float m_X = 0.0F;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public float m_Y = 0.0F;

        public PosData() { }

        public PosData(PosData data)
        {
            m_X = data.m_X;
            m_Y = data.m_Y;
        }

        public PosData(float x, float y)
        {
            m_X = x;
            m_Y = y;
        }
    }

    public class ObstacleData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public PosData m_Pos;
    }

    public class BattleMapData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_MapIndex = 0;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public float m_MinMapSizeX = 0.0F;

        [MessagePackMember(2, NilImplication = NilImplication.MemberDefault)]
        public float m_MaxMapSizeX = 0.0F;

        [MessagePackMember(3, NilImplication = NilImplication.MemberDefault)]
        public float m_MinMapSizeY = 0.0F;

        [MessagePackMember(4, NilImplication = NilImplication.MemberDefault)]
        public float m_MaxMapSizeY = 0.0F;

        [MessagePackMember(5, NilImplication = NilImplication.MemberDefault)]
        public List<ObstacleData> m_ObstacleDatas = new List<ObstacleData>();
    }

    public class BattleMemberData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_PlayerIndex = -1;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public ACTION_TYPE m_ActionType = ACTION_TYPE.NONE;

        [MessagePackMember(2, NilImplication = NilImplication.MemberDefault)]
        public PosData m_Pos;
    }

    public class CSBattleMemberActionData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public ACTION_TYPE m_ActionType = ACTION_TYPE.NONE;
    }

    public class SCSyncBattleData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_GameTimeRemain = 0;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public Dictionary<int, BattleMemberData> m_BattleMemberDatas = new Dictionary<int, BattleMemberData>();
    }

    public class SCMatchBattleRoomData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_RoomIndex = -1;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public BattleMapData m_BattleMapData;
    }

    public class SCBattleMemberData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_MyPlayerIndex = -1;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public Dictionary<int, BattleMemberData> m_BattleMemberDatas = new Dictionary<int, BattleMemberData>();
    }
}
