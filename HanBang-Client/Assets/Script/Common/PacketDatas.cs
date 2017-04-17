namespace GameServer.Packet
{
    public enum PACKET_TYPE
    {
        NONE = 1000,
        CS_MOVE,
        SC_MOVE,
        CS_ATTACK,
        SC_ATTACK,
        CS_MATCH_BATTLE_ROOM,
        SC_MATCH_BATTLE_ROOM,
    }

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
        public float m_X = 0.0F;
        public float m_Y = 0.0F;

        public PosData() { }

        public PosData(PosData data)
        {
            m_X = data.m_X;
            m_Y = data.m_Y;
        }
    }

    public class CSMoveData
    {
        public MOVE_TYPE m_MoveType = MOVE_TYPE.NONE;
    }

    public class SCMoveData
    {
        public int m_PlayerIndex = 0;

        public PosData m_Pos = null;
    }

    public class SCAttackData
    {
        public bool m_IsDamage = false;
    }

    public class SCMatchBattleRoom
    {
        public int m_RoomIndex = -1;
    }
}
