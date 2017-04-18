using GameServer.Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator m_EnemyAnimator = null;

    public void Initialize(int playerIndex)
    {
        if (playerIndex == 1)
        {
            transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
        }
    }

    // Use this for initialization
    void Start ()
    {
        m_EnemyAnimator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnChageAnimation(MOVE_TYPE moveType)
    {
        switch(moveType)
        {
            case MOVE_TYPE.LEFT:
                {
                    transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
                    m_EnemyAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case MOVE_TYPE.RIGHT:
                {
                    transform.transform.Rotate(new Vector3(0.0F, 0.0F, 0.0F));
                    m_EnemyAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case MOVE_TYPE.UP:
                {
                    m_EnemyAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case MOVE_TYPE.DOWN:
                {
                    m_EnemyAnimator.SetInteger("ActionControll", 1);
                }
                break;
            default:
                {
                    m_EnemyAnimator.SetInteger("ActionControll", 0);
                }
                break;
        }
    }
}
