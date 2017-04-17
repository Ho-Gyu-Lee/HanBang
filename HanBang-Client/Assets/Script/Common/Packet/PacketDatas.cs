namespace GameServer.Packet
{
    public enum PACKET_TYPE
    {
        NONE = 1000,
        CS_MOVE,
        SC_MOVE,
        CS_ATTACK,
        SC_ATTACK,
        CS_CREATE_BATTLE_ROOM,
        SC_CREATE_BATTLE_ROOM,
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
        public float m_X;
        public float m_Y;
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

    public class SCCreateBattleRoom
    {
        public int m_RoomIndex = -1;
    }
}
