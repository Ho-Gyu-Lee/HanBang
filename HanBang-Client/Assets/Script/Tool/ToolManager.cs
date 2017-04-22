using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    private Dictionary<string, GameObject> m_TilePrefabs = new Dictionary<string, GameObject>();

    public GameObject Terrain = null;

    // Use this for initialization
    void Start ()
    {
        object[] resources = Resources.LoadAll("Prefabs/Tile");
        foreach (object prefab in resources)
        {
            GameObject gameobject = prefab as GameObject;
            m_TilePrefabs.Add(gameobject.name, gameobject);
        }

        GameObject terrain = Instantiate(Resources.Load("Prefabs/Terrain/Terrain01") as GameObject, new Vector3(0.0F, 0.0F, 10), Quaternion.identity);
        terrain.transform.parent = transform;

        /*
        for (int i = 0; i < 20; ++i)
        {
            for (int j = 0; j < 20; ++j)
            {
                GameObject tile = Instantiate(m_TilePrefabs["grass"], new Vector3(-12.8F + (1.28F * j), 12.8F - (1.28F * i), 10), Quaternion.identity);
                tile.name = i.ToString() + "_" + j.ToString();
                tile.transform.parent = Terrain.transform;
            }
        }
        */
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
