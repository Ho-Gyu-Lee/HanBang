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

    private GameObject m_Player1 = null;
    private GameObject m_Player2 = null;

    private Text m_BattleTimeText = null;

    private List<string> m_CharacterResourcesName = new List<string>();

    private int m_Frame = 0;

    public int RoomIndex { get; private set; }

    public PLAYER_INDEX PlayerIndex { get; private set; }

    public PLAYER_INDEX EnumyPlayerIndex { get; private set; }

    public BattleMapData BattleMapData { get; private set; }

    public GameObject Player(PLAYER_INDEX playerIndex)
    {
        if (PlayerIndex == playerIndex)
            return m_Player1;
        else
            return m_Player2;
    }

    void Awake()
    {
        Screen.SetResolution(1280, 720, false);
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

        m_BattleTimeText = GameObject.Find("GameTimeRemain").GetComponent<Text>();
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

    public void InitializeBattleRoom(int roomIndex, BattleMapData battleMapData)
    {
        RoomIndex = roomIndex;
        BattleMapData = battleMapData;
    }

    public void InitializePlayer()
    {
        Player playerScript = m_Player1.GetComponent<Player>();
        playerScript.Initialize(PlayerIndex);

        Enemy enemyscript = m_Player2.GetComponent<Enemy>();
        enemyscript.Initialize(EnumyPlayerIndex);
    }

    public void OnPlayerSpawn(PLAYER_INDEX playerIndex, PosData pos)
    {
        if (m_Player1 != null) return;

        PlayerIndex = playerIndex;

        m_Player1 = Instantiate(m_Characters[m_CharacterResourcesName[(int)PlayerIndex]], new Vector3(pos.m_X, pos.m_Y, 0.0F), Quaternion.identity);
        m_Player1.transform.localScale = new Vector3(0.23F, 0.23F, 1);
        m_Player1.transform.parent = m_CharacterGroup.transform;
        m_Player1.AddComponent<Player>();

        Player script = m_Player1.GetComponent<Player>();
        script.Initialize(PlayerIndex);
    }

    public void OnEnemySpawn(PLAYER_INDEX playerIndex, PosData pos)
    {
        if (m_Player2 != null) return;

        EnumyPlayerIndex = playerIndex;

        m_Player2 = Instantiate(m_Characters[m_CharacterResourcesName[(int)EnumyPlayerIndex]], new Vector3(pos.m_X, pos.m_Y, 0.0F), Quaternion.identity);
        m_Player2.transform.localScale = new Vector3(0.23F, 0.23F, 1);
        m_Player2.transform.parent = m_CharacterGroup.transform;
        m_Player2.AddComponent<Enemy>();

        Enemy script = m_Player2.GetComponent<Enemy>();
        script.Initialize(EnumyPlayerIndex);

        // 적 정보 까지 모두 받으면 준비 완료
        ClientNetworkManager.Instance.SendManager.SendCSReadyBattle();
    }

    public void OnSyncBattle(SCSyncBattleData data)
    {
        m_Frame = data.m_Frame;
        m_BattleTimeText.text = "남은 시간 : " + data.m_GameTimeRemain.ToString();

        foreach (BattleMemberData member in data.m_BattleMemberDatas.Values)
        {
            GameObject player = Player(member.m_PlayerIndex);
            if (player != null)
            {
                if (PlayerIndex != member.m_PlayerIndex)
                {
                    Enemy script = player.GetComponent<Enemy>();
                    script.OnChageAnimation(member.m_ActionType);
                }
                else
                {
                    Player script = player.GetComponent<Player>();
                    script.OnChageAnimation(member.m_ActionType);
                }
                player.transform.position = new Vector3(member.m_Pos.m_X, member.m_Pos.m_Y, 0);
            }
        }
    }

    public void SendPlayerActionData(CSBattleMemberActionData data)
    {
        data.m_Frame = m_Frame + 1;
        ClientNetworkManager.Instance.SendManager.SendCSBattleMemberActionData(data);
    }
}
