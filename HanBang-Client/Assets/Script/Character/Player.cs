using GameServer.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator m_PlayerAnimator = null;

    private GameObject m_Camera = null;

    private bool m_PlayerIdele = false;

    // Use this for initialization
    void Start ()
    {
        m_Camera = GameObject.Find("Main Camera");
        m_PlayerAnimator = GetComponent<Animator>();

        // UI Setting
        CustomButton leftButton = GameObject.Find("Left").GetComponent<CustomButton>();
        leftButton.onClick.AddListener(delegate () { SendPlayerLeftMove(); });
        leftButton.ButtonDown += SendPlayerIdle;

        CustomButton rightButton = GameObject.Find("Right").GetComponent<CustomButton>();
        rightButton.onClick.AddListener(delegate () { SendPlayerRightMove(); });
        leftButton.ButtonDown += SendPlayerIdle;

        CustomButton downButton = GameObject.Find("Down").GetComponent<CustomButton>();
        downButton.onClick.AddListener(delegate () { SendPlayerDownMove(); });
        leftButton.ButtonDown += SendPlayerIdle;

        CustomButton upButton = GameObject.Find("Up").GetComponent<CustomButton>();
        upButton.onClick.AddListener(delegate () { SendPlayerUpMove(); });
        leftButton.ButtonDown += SendPlayerIdle;

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
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SendPlayerDownMove();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SendPlayerLeftMove();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SendPlayerRightMove();
        }

        if (Input.GetKeyUp(KeyCode.UpArrow)   ||
            Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow))
        {
            SendPlayerIdle();
        }

        /*
        Vector3 playerInfo = transform.transform.position + m_MovePostion;

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

        // 캐릭터 충돌 체크
        if (playerInfo.x > -19.1F && playerInfo.x < 17.8F && playerInfo.y < 17.9F && playerInfo.y > -18.4F)
        {
            transform.transform.position = new Vector3(playerInfo.x, playerInfo.y, playerInfo.z);
        }
        */
    }

    public void SendPlayerLeftMove()
    {
        ClientNetworkManager.Instance.SendManager.SendCSMoveData(new CSMoveData() { m_MoveType = MOVE_TYPE.LEFT });
    }

    public void SendPlayerRightMove()
    {
        ClientNetworkManager.Instance.SendManager.SendCSMoveData(new CSMoveData() { m_MoveType = MOVE_TYPE.RIGHT });
    }

    public void SendPlayerUpMove()
    {
        ClientNetworkManager.Instance.SendManager.SendCSMoveData(new CSMoveData() { m_MoveType = MOVE_TYPE.UP });
    }

    public void SendPlayerDownMove()
    {
        ClientNetworkManager.Instance.SendManager.SendCSMoveData(new CSMoveData() { m_MoveType = MOVE_TYPE.DOWN });
    }

    public void SendPlayerIdle()
    {
        ClientNetworkManager.Instance.SendManager.SendCSMoveData(new CSMoveData() { m_MoveType = MOVE_TYPE.NONE });
    }

    public void SendPlayerAttack()
    {
        ClientNetworkManager.Instance.SendManager.SendCSAttackData();
    }
}
