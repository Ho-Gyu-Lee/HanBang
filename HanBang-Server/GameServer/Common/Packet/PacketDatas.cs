using MsgPack.Serialization;
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
        public int m_RoomIndex = -1;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public int m_PlayerIndex = -1;

        [MessagePackMember(2, NilImplication = NilImplication.MemberDefault)]
        public ACTION_TYPE m_ActionType = ACTION_TYPE.NONE;

        public CSBattleMemberActionData() { }

        public CSBattleMemberActionData(CSBattleMemberActionData data)
        {
            m_RoomIndex   = data.m_RoomIndex;
            m_PlayerIndex = data.m_PlayerIndex;
            m_ActionType  = data.m_ActionType;
        }
    }

    public class SCSyncBattleData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_Frame = 0;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public int m_GameTimeRemain = 0;

        [MessagePackMember(2, NilImplication = NilImplication.MemberDefault)]
        public Dictionary<int, BattleMemberData> m_BattleMemberDatas = new Dictionary<int, BattleMemberData>();
    }

    public class SCMatchBattleRoomData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_RoomIndex = -1;

        // 추후 맵 정보도 여기다가 담아서 보낼거야
    }

    public class CSBattleMemberData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_RoomIndex = -1;
    }

    public class SCBattleMemberData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_MyPlayerIndex = -1;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public Dictionary<int, BattleMemberData> m_BattleMemberDatas = new Dictionary<int, BattleMemberData>();
    }
}
