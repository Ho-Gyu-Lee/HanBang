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

    public int RoomIndex { get; private set; }

    public GameObject Player(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                return m_Player1;
            case 1:
                return m_Player2;
        }

        return null;
    }

    public Vector3 GetStartPos(int playerIndex)
    {
        switch (playerIndex)
        {
            case 1:
                return new Vector3(-6.0F, 0.0F, 0.0F);
            case 2:
                return new Vector3(6.0F, 0.0F, 0.0F);
        }

        return new Vector3(0.0F, 0.0F, 0.0F);
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeBattleRoom(int roomIndex)
    {
        RoomIndex = roomIndex;
    }

    public void InitializePlayer(int playerIndex)
    {
        switch(playerIndex)
        {
            case 0:
                {
                    m_Player1 = Instantiate(m_Characters["Rogue_06"], GetStartPos(playerIndex), Quaternion.identity);
                    m_Player1.transform.localScale = new Vector3(0.23F, 0.23F, 1);
                    m_Player1.AddComponent<Player>();
                }
                break;
            case 1:
                {
                    m_Player2 = Instantiate(m_Characters["Rogue_01"], GetStartPos(playerIndex), Quaternion.identity);
                    m_Player2.transform.localScale = new Vector3(0.23F, 0.23F, 1);
                    m_Player2.AddComponent<Player>();
                }
                break;
        };
    }

    public void OnPlayerMove(int playerIndex, MOVE_TYPE moveType, PosData data)
    {
        GameObject player = Player(playerIndex);
        if(player != null)
            player.transform.position = new Vector3(data.m_X, data.m_Y, 0);
    }
}
