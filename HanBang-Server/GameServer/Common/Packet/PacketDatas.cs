using MsgPack.Serialization;
using System.Collections.Generic;

namespace GameServer.Common.Packet
{
    [MessagePackEnum]
    public enum PACKET_TYPE
    {
        NONE = 1000,
        CS_MOVE,
        CS_ATTACK,
        SC_SYNC_BATTLE,
        CS_MATCH_BATTLE_ROOM,
        SC_MATCH_BATTLE_ROOM,
    }

    [MessagePackEnum]
    public enum MOVE_TYPE
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN
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
    }

    public class BattleMemberData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_PlayerIndex = -1;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public MOVE_TYPE m_MoveType = MOVE_TYPE.NONE;

        [MessagePackMember(2, NilImplication = NilImplication.MemberDefault)]
        public PosData m_Pos;

        [MessagePackMember(3, NilImplication = NilImplication.MemberDefault)]
        public bool IsDie = false;
    }

    public class CSMoveData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_RoomIndex = -1;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public int m_PlayerIndex = -1;

        [MessagePackMember(2, NilImplication = NilImplication.MemberDefault)]
        public MOVE_TYPE m_MoveType = MOVE_TYPE.NONE;
    }

    public class SCSyncBattleData
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_Frame = 0;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public Dictionary<int, BattleMemberData> m_BattleMemberDatas = new Dictionary<int, BattleMemberData>();
    }

    public class SCMatchBattleRoom
    {
        [MessagePackMember(0, NilImplication = NilImplication.MemberDefault)]
        public int m_RoomIndex = -1;

        [MessagePackMember(1, NilImplication = NilImplication.MemberDefault)]
        public int m_PlayerIndex = -1;
    }
}
