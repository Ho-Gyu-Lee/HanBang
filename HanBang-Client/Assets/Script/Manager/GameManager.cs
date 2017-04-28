using GameServer.Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region SingleTon
    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                if (!m_Instance)
                    Debug.LogError("There needs to be one active MyClass script on a GameObject in your scene.");
            }

            return m_Instance;
        }
    }
    #endregion

    private Dictionary<string, GameObject> m_Characters = new Dictionary<string, GameObject>();

    private GameObject m_CharacterGroup = null;

    private GameObject m_Player = null;
    private Player m_PlayerScript = null;

    private GameObject m_Enemy = null;
    private Enemy m_EnemyScript = null;

    private GameObject m_BattleUI = null;
    private GameObject m_TitleUI = null;

    private Text m_BattleTimeText = null;

    private GameObject m_StartBattleCount = null;
    private Text m_StartBattleCountText = null;

    private Text m_Player1KillCountText = null;
    private Text m_Player2KillCountText = null;

    private CSBattleMemberActionData m_Player1ActionData = new CSBattleMemberActionData();

    private List<string> m_CharacterResourcesName = new List<string>();

    public int RoomIndex { get; private set; }

    public PLAYER_INDEX PlayerIndex { get; private set; }

    public PLAYER_INDEX EnumyPlayerIndex { get; private set; }

    void Awake()
    {
        Screen.SetResolution(1280, 720, false);

        MsgPack.Serialization.MessagePackSerializer.PrepareType<PACKET_TYPE>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<ACTION_TYPE>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<WAITING_TYPE>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<PLAYER_INDEX>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<PosData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<ObstacleData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<BattleTerrainData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<BattleMemberData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<CSBattleMemberActionData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<SCBattleWatingData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<SCSyncBattleData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<SCMatchBattleRoomData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<SCBattleMemberData>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<int>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<float>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<List<ObstacleData>>();
        MsgPack.Serialization.MessagePackSerializer.PrepareType<Dictionary<int, BattleMemberData>>();
    }

    // Use this for initialization
    void Start()
    {
        object[] resources = Resources.LoadAll("Mighty Heroes/Prefabs");
        foreach (object prefab in resources)
        {
            GameObject gameobject = prefab as GameObject;
            m_Characters.Add(gameobject.name, gameobject);
        }

        m_BattleUI = GameObject.Find("BattleUI");
        m_TitleUI  = GameObject.Find("TitleUI");

        m_BattleTimeText = GameObject.Find("GameTimeRemain").GetComponent<Text>();

        m_StartBattleCount = GameObject.Find("StartCount");
        m_StartBattleCountText = m_StartBattleCount.GetComponent<Text>();
        m_StartBattleCount.SetActive(false);

        m_Player1KillCountText = GameObject.Find("PlayerKillCount").GetComponent<Text>();
        m_Player2KillCountText = GameObject.Find("EnemyKillCount").GetComponent<Text>();

        m_BattleUI.SetActive(false);

        m_CharacterGroup = GameObject.Find("CharacterGroup");

        // 임시 캐릭터 리소스
        m_CharacterResourcesName.Add("Rogue_06");
        m_CharacterResourcesName.Add("Rogue_01");

        Application.runInBackground = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeBattleRoom(int roomIndex, BattleTerrainData battleTerrainData)
    {
        RoomIndex = roomIndex;
        TerrainManager.Instance.OnBattleTerrainData(battleTerrainData);

        m_TitleUI.SetActive(false);
        m_BattleUI.SetActive(true);
    }

    public void InitializePlayer()
    {
        m_PlayerScript.Initialize(PlayerIndex);
        //m_EnemyScript.Initialize(EnumyPlayerIndex);
    }

    public void OnPlayerSpawn(PLAYER_INDEX playerIndex, PosData pos)
    {
        if (m_Player != null) return;

        PlayerIndex = playerIndex;

        m_Player = Instantiate(m_Characters[m_CharacterResourcesName[(int)PlayerIndex]], new Vector3(pos.m_X, pos.m_Y, 0.0F), Quaternion.identity);
        m_Player.transform.localScale = new Vector3(0.23F, 0.23F, 1);
        m_Player.transform.parent = m_CharacterGroup.transform;
        m_Player.AddComponent<Player>();

        m_PlayerScript = m_Player.GetComponent<Player>();
        m_PlayerScript.Initialize(PlayerIndex);

        ClientNetworkManager.Instance.SendManager.SendCSReadyBattle();
    }

    public void OnEnemySpawn(PLAYER_INDEX playerIndex, PosData pos)
    {
        if (m_Enemy != null) return;

        EnumyPlayerIndex = playerIndex;

        m_Enemy = Instantiate(m_Characters[m_CharacterResourcesName[(int)EnumyPlayerIndex]], new Vector3(pos.m_X, pos.m_Y, 0.0F), Quaternion.identity);
        m_Enemy.transform.localScale = new Vector3(0.23F, 0.23F, 1);
        m_Enemy.transform.parent = m_CharacterGroup.transform;
        m_Enemy.AddComponent<Enemy>();

        m_EnemyScript = m_Enemy.GetComponent<Enemy>();
        m_EnemyScript.Initialize(EnumyPlayerIndex);

        // 적 정보 까지 모두 받으면 준비 완료
        ClientNetworkManager.Instance.SendManager.SendCSReadyBattle();
    }

    public void OnSCBattleWating(SCBattleWatingData data)
    {
        switch (data.m_WaitingType)
        {
            case WAITING_TYPE.START_BATTLE:
                {
                    if (m_StartBattleCount.activeSelf == false)
                    {
                        InitializePlayer();
                        m_StartBattleCount.SetActive(true);
                    }

                    m_StartBattleCountText.text = data.m_Count.ToString();

                    if (data.m_Count <= 0)
                        m_StartBattleCount.SetActive(false);
                }
                break;

            case WAITING_TYPE.RE_START_BATTLE:
                {
                    if (data.m_Count <= 0)
                        InitializePlayer();
                }
                break;
        }
    }

    public void SendBattleMemberActionData(ACTION_TYPE action)
    {
        m_Player1ActionData.m_ActionQueue.Enqueue(action);
    }

    public void OnSCBattleMemberActionData()
    {
        m_Player1ActionData.m_ActionQueue.Clear();
    }

    public void OnSyncBattle(SCSyncBattleData data)
    {
        m_BattleTimeText.text = "남은 시간 : " + data.m_GameTimeRemain.ToString();

        foreach (BattleMemberData member in data.m_BattleMemberDatas.Values)
        {
            GameObject gameObject = null;
            if (PlayerIndex != member.m_PlayerIndex)
            {
                gameObject = m_Enemy;
                m_EnemyScript.OnChageAnimation(member.m_ActionType);
            }
            else
            {
                gameObject = m_Player;
                m_PlayerScript.OnChageAnimation(member.m_ActionType);
            }

            if(member.m_PlayerIndex == PLAYER_INDEX.PLAYER_1)
                m_Player1KillCountText.text = member.m_KillCount.ToString();
            else
                m_Player2KillCountText.text = member.m_KillCount.ToString();

            if (gameObject != null)
                gameObject.transform.position = new Vector3(member.m_Pos.m_X, member.m_Pos.m_Y, 0);
        }

        m_Player1ActionData.m_Frame = data.m_Frame;
        ClientNetworkManager.Instance.SendManager.SendCSBattleMemberActionData(m_Player1ActionData);
    }
}
