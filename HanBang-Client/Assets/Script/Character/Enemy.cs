using GameServer.Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator m_EnemyAnimator = null;

    private bool m_IsPlayerRotation = false;

    public void Initialize(int playerIndex)
    {
        if (playerIndex == 1)
        {
            m_IsPlayerRotation = true;
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

    public void OnChageAnimation(ACTION_TYPE actionType)
    {
        switch(actionType)
        {
            case ACTION_TYPE.LEFT:
                {
                    if (m_IsPlayerRotation == false)
                    {
                        m_IsPlayerRotation = true;
                        transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
                    }

                    m_EnemyAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case ACTION_TYPE.RIGHT:
                {
                    if (m_IsPlayerRotation)
                    {
                        m_IsPlayerRotation = false;
                        transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
                    }

                    m_EnemyAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case ACTION_TYPE.UP:
                {
                    m_EnemyAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case ACTION_TYPE.DOWN:
                {
                    m_EnemyAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case ACTION_TYPE.ATTACK:
                {
                    m_EnemyAnimator.SetInteger("ActionControll", 2);
                }
                break;
            case ACTION_TYPE.DIE:
                {
                    m_EnemyAnimator.SetInteger("ActionControll", 3);
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
