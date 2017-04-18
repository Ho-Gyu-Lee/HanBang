using GameServer.Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private GameObject m_Player1 = null;
    private GameObject m_Player2 = null;

    private List<string> m_CharacterResourcesName = new List<string>();

    public int RoomIndex { get; private set; }

    public int PlayerIndex { get; private set; }

    public GameObject Player(int playerIndex)
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

        // 임시 캐릭터 리소스
        m_CharacterResourcesName.Add("Rogue_06");
        m_CharacterResourcesName.Add("Rogue_01");

        Application.runInBackground = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeBattleRoom(int roomIndex)
    {
        RoomIndex = roomIndex;
    }

    public void OnPlayerSpawn(int playerIndex, PosData pos)
    {
        if (m_Player1 != null) return;

        PlayerIndex = playerIndex;

        m_Player1 = Instantiate(m_Characters[m_CharacterResourcesName[playerIndex]], new Vector3(pos.m_X, pos.m_Y, 0.0F), Quaternion.identity);
        m_Player1.transform.localScale = new Vector3(0.23F, 0.23F, 1);
        m_Player1.AddComponent<Player>();

        Player script = m_Player1.GetComponent<Player>();
        script.InitializeMoveData(RoomIndex, playerIndex);
    }

    public void OnEnemySpawn(int playerIndex, PosData pos)
    {
        if (m_Player2 != null) return;

        m_Player2 = Instantiate(m_Characters[m_CharacterResourcesName[playerIndex]], new Vector3(pos.m_X, pos.m_Y, 0.0F), Quaternion.identity);
        m_Player2.transform.localScale = new Vector3(0.23F, 0.23F, 1);
        m_Player2.AddComponent<Enemy>();

        Enemy script = m_Player2.GetComponent<Enemy>();
        script.Initialize(playerIndex);
    }

    public void OnPlayerMove(int playerIndex, MOVE_TYPE moveType, PosData data)
    {
        GameObject player = Player(playerIndex);
        if(player != null)
        {
            if (PlayerIndex != playerIndex)
            {
                Enemy script = player.GetComponent<Enemy>();
                script.OnChageAnimation(moveType);
            }
            player.transform.position = new Vector3(data.m_X, data.m_Y, 0);
        }
    }
}
