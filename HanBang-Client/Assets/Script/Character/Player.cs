using GameServer.Common.Packet;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator m_PlayerAnimator = null;

    private GameObject m_Camera = null;

    private bool m_IsAttackAnimation = false;
    private bool m_IsPlayerRotation  = false;

    private CSBattleMemberActionData m_BattleMemberActionData = new CSBattleMemberActionData();

    public void InitializeMoveData(int roomIndex, int playerIndex)
    {
        m_BattleMemberActionData.m_PlayerIndex = playerIndex;
        m_BattleMemberActionData.m_RoomIndex   = roomIndex;
        m_BattleMemberActionData.m_ActionType  = ACTION_TYPE.NONE;

        if(playerIndex == 1)
        {
            m_IsPlayerRotation = true;
            transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
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
        
        Vector3 playerInfo = transform.transform.position;

        // 카메라 충돌 체크
        Vector3 cameraPostion = m_Camera.transform.position;
        if (playerInfo.x > -10.9F && playerInfo.x < 9.6F)
        {
            cameraPostion.x = playerInfo.x;
        }

        if (playerInfo.y < 13.7F && playerInfo.y > -14.2F)
        {
            cameraPostion.y = playerInfo.y + 1;
        }

        m_Camera.transform.position = new Vector3(cameraPostion.x, cameraPostion.y, cameraPostion.z);

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
        if (m_IsAttackAnimation == true) return;
        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.LEFT) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.LEFT;

        if(m_IsPlayerRotation == false)
        {
            m_IsPlayerRotation = true;
            transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
        }

        m_PlayerAnimator.SetInteger("ActionControll", 1);

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerRightMove()
    {
        if (m_IsAttackAnimation == true) return;
        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.RIGHT) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.RIGHT;

        if(m_IsPlayerRotation)
        {
            m_IsPlayerRotation = false;
            transform.transform.Rotate(new Vector3(0.0F, 180.0F, 0.0F));
        }

        m_PlayerAnimator.SetInteger("ActionControll", 1);

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerUpMove()
    {
        if (m_IsAttackAnimation == true) return;
        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.UP) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.UP;
        m_PlayerAnimator.SetInteger("ActionControll", 1);

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerDownMove()
    {
        if (m_IsAttackAnimation == true) return;
        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.DOWN) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.DOWN;
        m_PlayerAnimator.SetInteger("ActionControll", 1);

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerIdle()
    {
        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.NONE) return;

        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.NONE;
        m_PlayerAnimator.SetInteger("ActionControll", 0);

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }

    public void SendPlayerAttack()
    {
        if (m_IsAttackAnimation == true) return;
        if (m_BattleMemberActionData.m_ActionType == ACTION_TYPE.ATTACK) return;

        m_IsAttackAnimation = true;
        m_BattleMemberActionData.m_ActionType = ACTION_TYPE.ATTACK;
        m_PlayerAnimator.SetInteger("ActionControll", 2);

        GameManager.Instance.SendPlayerActionData(m_BattleMemberActionData);
    }
}
