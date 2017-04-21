using GameServer.Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    #region SingleTon
    private static TerrainManager m_Instance;
    public static TerrainManager Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType(typeof(TerrainManager)) as TerrainManager;
                if (!m_Instance)
                    Debug.LogError("There needs to be one active MyClass script on a GameObject in your scene.");
            }

            return m_Instance;
        }
    }
    #endregion

    private Dictionary<string, GameObject> m_TilePrefabs = new Dictionary<string, GameObject>();

    public BattleTerrainData BattleTerrainData { get; private set; }

    public GameObject TileMap01 = null;

    // Use this for initialization
    void Start ()
    {
        object[] resources = Resources.LoadAll("Prefabs/TilePrefabs");
        foreach (object prefab in resources)
        {
            GameObject gameobject = prefab as GameObject;
            m_TilePrefabs.Add(gameobject.name, gameobject);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnBattleTerrainData(BattleTerrainData battleTerrainData)
    {
        BattleTerrainData = battleTerrainData;

        for (int i = 0; i < 20; ++i)
        {
            for (int j = 0; j < 20; ++j)
            {
                GameObject tile = Instantiate(m_TilePrefabs["grass"], new Vector3(-12.8F + (1.28F * j), 12.8F - (1.28F * i), 10), Quaternion.identity);
                tile.name = i.ToString() + "_" + j.ToString();
                tile.transform.parent = TileMap01.transform;
            }
        }

        foreach(ObstacleData data in BattleTerrainData.m_ObstacleDatas)
        {
            GameObject obstacle = Instantiate(m_TilePrefabs["tree"], new Vector3(data.m_Pos.m_X, data.m_Pos.m_Y, 5), Quaternion.identity);
            obstacle.transform.localScale = new Vector3(1.5F, 1.5F, 0.0F);
            obstacle.transform.parent = TileMap01.transform;
        }
    }
}
