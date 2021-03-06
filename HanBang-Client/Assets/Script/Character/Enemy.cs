﻿using GameServer.Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator m_EnemyAnimator = null;

    private bool m_IsPlayerRotation = false;

    private bool m_IsEnemyDie = false;

    public void Initialize(PLAYER_INDEX playerIndex)
    {
        m_IsEnemyDie = false;

        if (m_EnemyAnimator != null)
            m_EnemyAnimator.SetInteger("ActionControll", 0);

        if (playerIndex == PLAYER_INDEX.PLAYER_2)
        {
            if(m_IsPlayerRotation == false)
            {
                m_IsPlayerRotation = true;
                transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
            }
        }
        else
        {
            if (m_IsPlayerRotation)
            {
                m_IsPlayerRotation = false;
                transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
            }
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
                    if (m_IsEnemyDie) break;

                    Vector3 playerInfo = transform.transform.position;
                    GameObject instance = Instantiate(Resources.Load("Cube Space/Prefabs/Ef_Electro_02", typeof(GameObject)) as GameObject, new Vector3(playerInfo.x, playerInfo.y + 1.0F, 0.0F), Quaternion.identity);
                    instance.transform.localScale = new Vector3(1.5F, 1.5F, 1);

                    m_IsEnemyDie = true;
                    m_EnemyAnimator.SetInteger("ActionControll", 3);
                }
                break;
            default:
                {
                    if (m_IsEnemyDie) break;

                    m_EnemyAnimator.SetInteger("ActionControll", 0);
                }
                break;
        }
    }
}
