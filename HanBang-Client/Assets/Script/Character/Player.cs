using GameServer.Common.Packet;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator m_PlayerAnimator = null;

    private GameObject m_Camera = null;

    private bool m_IsAttackAnimation = false;
    private bool m_IsPlayerRotation  = false;

    private bool m_IsPlayerDie = false;

    private CSBattleMemberActionData m_BattleMemberActionData = new CSBattleMemberActionData();

    public void Initialize(int playerIndex)
    {
        m_IsPlayerDie = false;
        m_IsAttackAnimation = false;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.NONE;

        if(m_PlayerAnimator != null)
            m_PlayerAnimator.SetInteger("ActionControll", 0);

        if (playerIndex == 1)
        {
            if (m_IsPlayerRotation == false)
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
        m_Camera = GameObject.Find("Main Camera");
        m_Camera.transform.position = new Vector3(transform.transform.position.x, transform.transform.position.y + 1, m_Camera.transform.position.z);

        m_PlayerAnimator = GetComponent<Animator>();

        // UI Setting
        CustomButton leftButton = GameObject.Find("Left").GetComponent<CustomButton>();
        leftButton.ButtonDown += SendPlayerLeftMove;
        leftButton.ButtonUp   += SendPlayerIdle;

        CustomButton rightButton = GameObject.Find("Right").GetComponent<CustomButton>();
        rightButton.ButtonDown += SendPlayerRightMove;
        rightButton.ButtonUp   += SendPlayerIdle;

        CustomButton downButton = GameObject.Find("Down").GetComponent<CustomButton>();
        downButton.ButtonDown += SendPlayerDownMove;
        downButton.ButtonUp   += SendPlayerIdle;

        CustomButton upButton = GameObject.Find("Up").GetComponent<CustomButton>();
        upButton.ButtonDown += SendPlayerUpMove;
        upButton.ButtonUp   += SendPlayerIdle;

        Button attackButton = GameObject.Find("Attack").GetComponent<Button>();
        attackButton.onClick.AddListener(delegate () { SendPlayerAttack(); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendPlayerAttack();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SendPlayerUpMove();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SendPlayerDownMove();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SendPlayerLeftMove();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SendPlayerRightMove();
        }

        if (!Input.anyKey)
        {
            SendPlayerIdle();
        }

        // 카메라 충돌 체크
        if(GameManager.Instance.BattleMapData != null)
        {
            float minMapSizeX = GameManager.Instance.BattleMapData.m_MinMapSizeX;
            float maxMapSizeX = GameManager.Instance.BattleMapData.m_MaxMapSizeX;
            float minMapSizeY = GameManager.Instance.BattleMapData.m_MinMapSizeY;
            float maxMapSizeY = GameManager.Instance.BattleMapData.m_MaxMapSizeY;

            Vector3 playerInfo = transform.transform.position;
            Vector3 cameraPostion = m_Camera.transform.position;
            if (playerInfo.x >= (minMapSizeX + 8.3F) && playerInfo.x <= (maxMapSizeX - 8.12F))
            {
                cameraPostion.x = playerInfo.x;
            }

            if (playerInfo.y >= (minMapSizeY + 3.32F) && playerInfo.y <= (maxMapSizeY - 5.5F))
            {
                cameraPostion.y = playerInfo.y + 1;
            }

            m_Camera.transform.position = new Vector3(cameraPostion.x, cameraPostion.y, cameraPostion.z);
        }

        // 공격 모션 완료
        if (m_IsAttackAnimation)
        {
            if (!m_PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Rogue_attack_02"))
            {
                m_IsAttackAnimation = false;
            }
        }
    }

    public void SendPlayerLeftMove()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.LEFT) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.LEFT;

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerRightMove()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.RIGHT) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.RIGHT;

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerUpMove()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.UP) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.UP;

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerDownMove()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.DOWN) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.DOWN;

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerIdle()
    {
        if (m_IsPlayerDie) return;

        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.NONE) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.NONE;

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerAttack()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.ATTACK) return;

        m_IsAttackAnimation = true;
        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.ATTACK;

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void OnChageAnimation(ACTION_TYPE actionType)
    {
        switch (actionType)
        {
            case ACTION_TYPE.LEFT:
                {
                    if (m_IsPlayerRotation == false)
                    {
                        m_IsPlayerRotation = true;
                        transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
                    }

                    m_PlayerAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case ACTION_TYPE.RIGHT:
                {
                    if (m_IsPlayerRotation)
                    {
                        m_IsPlayerRotation = false;
                        transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
                    }

                    m_PlayerAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case ACTION_TYPE.UP:
                {
                    m_PlayerAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case ACTION_TYPE.DOWN:
                {
                    m_PlayerAnimator.SetInteger("ActionControll", 1);
                }
                break;
            case ACTION_TYPE.ATTACK:
                {
                    m_PlayerAnimator.SetInteger("ActionControll", 2);
                }
                break;
            case ACTION_TYPE.DIE:
                {
                    Vector3 playerInfo = transform.transform.position;
                    GameObject instance = Instantiate(Resources.Load("Cube Space/Prefabs/Ef_Electro_02", typeof(GameObject)) as GameObject, new Vector3(playerInfo.x, playerInfo.y + 1.0F, 0.0F), Quaternion.identity);
                    instance.transform.localScale = new Vector3(1.5F, 1.5F, 1);

                    m_IsPlayerDie = true;
                    m_PlayerAnimator.SetInteger("ActionControll", 3);
                }
                break;
            default:
                {
                    m_PlayerAnimator.SetInteger("ActionControll", 0);
                }
                break;
        }
    }
}
