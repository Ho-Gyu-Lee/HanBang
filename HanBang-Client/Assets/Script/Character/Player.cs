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

    public void Initialize(PLAYER_INDEX playerIndex)
    {
        m_IsPlayerDie = false;
        m_IsAttackAnimation = false;

        if(m_PlayerAnimator != null)
            m_PlayerAnimator.SetInteger("ActionControll", 0);

        if (playerIndex == PLAYER_INDEX.PLAYER_2)
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
        leftButton.ButtonEnter += SendPlayerLeftMove;
        leftButton.ButtonUp   += SendPlayerIdle;

        CustomButton rightButton = GameObject.Find("Right").GetComponent<CustomButton>();
        rightButton.ButtonEnter += SendPlayerRightMove;
        rightButton.ButtonUp   += SendPlayerIdle;

        CustomButton downButton = GameObject.Find("Down").GetComponent<CustomButton>();
        downButton.ButtonEnter += SendPlayerDownMove;
        downButton.ButtonUp   += SendPlayerIdle;

        CustomButton upButton = GameObject.Find("Up").GetComponent<CustomButton>();
        upButton.ButtonEnter += SendPlayerUpMove;
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
        if(TerrainManager.Instance.BattleTerrainData != null)
        {
            float minMapSizeX = TerrainManager.Instance.BattleTerrainData.m_MinSizeX;
            float maxMapSizeX = TerrainManager.Instance.BattleTerrainData.m_MaxSizeX;
            float minMapSizeY = TerrainManager.Instance.BattleTerrainData.m_MinSizeY;
            float maxMapSizeY = TerrainManager.Instance.BattleTerrainData.m_MaxSizeY;

            Vector3 playerInfo = transform.transform.position;
            Vector3 cameraPostion = m_Camera.transform.position;
            if (playerInfo.x >= (minMapSizeX + 8.3F) && playerInfo.x <= (maxMapSizeX - 8.2F))
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

        GameManager.Instance.SendBattleMemberActionData(ACTION_TYPE.LEFT);
    }

    public void SendPlayerRightMove()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        GameManager.Instance.SendBattleMemberActionData(ACTION_TYPE.RIGHT);
    }

    public void SendPlayerUpMove()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        GameManager.Instance.SendBattleMemberActionData(ACTION_TYPE.UP);
    }

    public void SendPlayerDownMove()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        GameManager.Instance.SendBattleMemberActionData(ACTION_TYPE.DOWN);
    }

    public void SendPlayerIdle()
    {
        if (m_IsPlayerDie) return;

        GameManager.Instance.SendBattleMemberActionData(ACTION_TYPE.NONE);
    }

    public void SendPlayerAttack()
    {
        if (m_IsPlayerDie) return;

        if (m_IsAttackAnimation) return;

        m_IsAttackAnimation = true;
        GameManager.Instance.SendBattleMemberActionData(ACTION_TYPE.ATTACK);
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
                    if (m_IsPlayerDie) break;

                    Vector3 playerInfo = transform.transform.position;
                    GameObject instance = Instantiate(Resources.Load("Cube Space/Prefabs/Ef_Electro_02", typeof(GameObject)) as GameObject, new Vector3(playerInfo.x, playerInfo.y + 1.0F, 0.0F), Quaternion.identity);
                    instance.transform.localScale = new Vector3(1.5F, 1.5F, 1);

                    m_IsPlayerDie = true;
                    m_PlayerAnimator.SetInteger("ActionControll", 3);
                }
                break;
            default:
                {
                    if (m_IsPlayerDie) break;

                    m_PlayerAnimator.SetInteger("ActionControll", 0);
                }
                break;
        }
    }
}
