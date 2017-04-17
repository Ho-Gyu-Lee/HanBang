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

    public GameObject Player(int playerIndex)
    {
        switch(playerIndex)
        {
            case 1:
                return m_Player1;
            case 2:
                return m_Player2;              
        }

        return null;
    }

	// Use this for initialization
	void Start ()
    {
        object[] resources = Resources.LoadAll("Mighty Heroes/Prefabs");
        foreach(object prefab in resources)
        {
            GameObject gameobject = prefab as GameObject;
            m_Characters.Add(gameobject.name, gameobject);
        }

        // 우선 임시로 캐릭터 한개를 로딩
        m_Player1 = Instantiate(m_Characters["Rogue_01"], new Vector3(0, 0, 0), Quaternion.identity);
        m_Player1.transform.localScale = new Vector3(0.23F, 0.23F, 1);
        m_Player1.AddComponent<Player>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
