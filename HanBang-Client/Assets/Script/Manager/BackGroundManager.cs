using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundManager : MonoBehaviour
{
    private Dictionary<string, GameObject> m_TilePrefabs = new Dictionary<string, GameObject>();

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

        for (int i = 0; i < 20; ++i)
        {
            for(int j = 0; j < 20; ++j)
            {
                GameObject tile = null;
                tile = Instantiate(m_TilePrefabs["grass"], new Vector3(-12.8F + (1.28F * j), 12.8F - (1.28F * i), 10), Quaternion.identity);
                tile.name = i.ToString() + "_" + j.ToString();
                tile.transform.parent = TileMap01.transform;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
