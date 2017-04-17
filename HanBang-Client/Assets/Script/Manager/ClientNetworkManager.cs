using ClientNetworkEngine;
using GameServer.Common.Packet;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientNetworkManager : MonoBehaviour
{
    #region SingleTon
    private static ClientNetworkManager m_Instance;
    public static ClientNetworkManager Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType(typeof(ClientNetworkManager)) as ClientNetworkManager;
                if (!m_Instance)
                    Debug.LogError("There needs to be one active MyClass script on a GameObject in your scene.");
            }

            return m_Instance;
        }
    }
    #endregion

    struct MessageData
    {
        public int m_MessageType;
        public byte[] m_Message;
    }

    private UnityTcpSession m_Session = new UnityTcpSession();

    private PacketReceiveManager m_PacketReceiveManager = new PacketReceiveManager();

    private PacketSendManager m_PacketSendManager = new PacketSendManager();

    private Queue<MessageData> m_MessageDatas = new Queue<MessageData>();

    public PacketSendManager SendManager { get { return m_PacketSendManager; } }

    // Use this for initialization
    void Start ()
    {
        m_Session.NoDeplay         = true;
        m_Session.SendTimeOut      = 5000;
        m_Session.SendingQueueSize = 100;

        m_Session.Connected += OnCompletConnectedToServer;
        m_Session.Closed    += OnClosedToServer;
        m_Session.Error     += OnErrorToServer;

        m_Session.ReceiveMessage += OnReceiveMessage;

        m_PacketSendManager.SendHandler += m_Session.SendMsg;

        m_PacketReceiveManager.SCMatchBattleRoom += OnSCMatchBattleRoom;
        m_PacketReceiveManager.SCSyncBattleData += OnSCSyncBattleData;
    }
	
	// Update is called once per frame
	void Update ()
    {
        lock(m_MessageDatas)
        {
            if (m_MessageDatas.Count > 0)
            {
                while(m_MessageDatas.Count != 0)
                {
                    MessageData data = m_MessageDatas.Dequeue();
                    m_PacketReceiveManager.OnReceiveMessage(data.m_MessageType, data.m_Message);
                }
            }
        }
	}

    void ApplicationQuit()
    {
        m_Session.Close();
    }

    public void Connection()
    {
        m_Session.Connect("127.0.0.1", 10001);
    }

    private void OnCompletConnectedToServer(object sender, EventArgs e)
    {
        SendManager.SendCSMatchBattleRoom();
    }

    private void OnClosedToServer(object sender, EventArgs e)
    {

    }

    private void OnErrorToServer(object sender, ErrorEventArgs e)
    {

    }

    private void OnReceiveMessage(int msgType, byte[] msg)
    {
        lock (m_MessageDatas)
        {
            m_MessageDatas.Enqueue(new MessageData() { m_MessageType = msgType, m_Message = msg });
        }
    }

    private void OnSCMatchBattleRoom(SCMatchBattleRoom data)
    {
        GameManager.Instance.InitializePlayer(data.m_PlayerIndex);
    }

    private void OnSCSyncBattleData(SCSyncBattleData data)
    {
        foreach(BattleMemberData member in data.m_BattleMemberDatas.Values)
        {
            GameManager.Instance.OnPlayerMove(member.m_PlayerIndex, member.m_MoveType, member.m_Pos);
        }
    }
}
