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

    // Use this for initialization
    void Start ()
    {
        object[] resources = Resources.LoadAll("Prefabs/Tile");
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

        GameObject terrain = Instantiate(Resources.Load("Prefabs/Terrain/Terrain01") as GameObject, new Vector3(-0.6F, 0.2F, 10), Quaternion.identity);
        terrain.transform.parent = transform;

        foreach (ObstacleData data in BattleTerrainData.m_ObstacleDatas)
        {
            GameObject obstacle = Instantiate(m_TilePrefabs["tree"], new Vector3(data.m_Pos.m_X, data.m_Pos.m_Y, 5), Quaternion.identity);
            obstacle.transform.localScale = new Vector3(1.5F, 1.5F, 0.0F);
            obstacle.transform.parent = terrain.transform;
        }
    }
}
