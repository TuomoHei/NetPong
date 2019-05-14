using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Client : MonoBehaviour, INetEventListener
{
    private readonly NetPacketProcessor netPacketProcessor = new NetPacketProcessor();

    private NetManager netClient;

    [SerializeField]
    private GameObject clientPlayer;

    [SerializeField]
    private GameObject clientPlayerInterpolated;

    [SerializeField]
    private GameObject clientBall;

    [SerializeField]
    private GameObject clientBallInterpolated;

    private float playerNewPos;
    private float playerOldPos;

    private Vector3 ballNewPos;
    private Vector3 ballOldPos;

    private float playerLerpTime;
    private float ballLerpTime;

    private NetPeer server;

    private int ping;

    public string thisIP;

    private void Start()
    {
        netClient = new NetManager(this)
        {
            UnconnectedMessagesEnabled = true,
            UpdateTime = 15,
            AutoRecycle = true
        };

        netClient.Start();
    }

    private void Update()
    {
        netClient.PollEvents();

        var peer = netClient.FirstPeer;

        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            var playerNewPos = clientPlayerInterpolated.transform.position.z;
            var ballNewPos = clientBallInterpolated.transform.position;

            playerNewPos = Mathf.Lerp(playerOldPos, playerNewPos, playerLerpTime);
            ballNewPos.x = Mathf.Lerp(ballOldPos.x, ballNewPos.x, ballLerpTime);
            ballNewPos.z = Mathf.Lerp(ballOldPos.z, ballNewPos.z, ballLerpTime);

            clientPlayerInterpolated.transform.position = new Vector3(0, 0, playerNewPos);
            clientBallInterpolated.transform.position = ballNewPos;

            playerLerpTime += Time.deltaTime / Time.fixedDeltaTime;
            ballLerpTime += Time.deltaTime / Time.fixedDeltaTime;
        }

        else
        {
            netClient.SendBroadcast(new byte[] { 1 }, 2310); //byte[] data, int port
        }
    }

    public void Connect(string ip, int port)
    {
        netClient.Connect(ip, port, "NetPong"); //2310-2320
        thisIP = ip;
    }

    private void OnDestroy()
    {
        if (netClient != null)
        {
            netClient.Stop();
        }
    }

    private void OnServerState()
    {
        
    }

    #region INetEventListener
    void INetEventListener.OnPeerConnected(NetPeer peer)
    {
        Debug.Log("Client || Connected to " + peer.EndPoint);
        server = peer;
    }

    void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo dcInfo)
    {
        server = null;
        //LogicTimer.Stop();
        Debug.Log("Client || Disconnected. (Reason: " + dcInfo.Reason + ")");
    }

    void INetEventListener.OnNetworkError(IPEndPoint ep, SocketError socketErrorCode)
    {
        Debug.Log("Client || Error: " + socketErrorCode);
    }

    void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod delMethod)
    {
        Packet p = new Packet();

        playerNewPos = p.PlayerPos;
        ballNewPos.x = reader.GetFloat();
        ballNewPos.z = reader.GetFloat();

        var playerPos = clientPlayer.transform.position;
        var ballPos = clientBall.transform.position;

        playerOldPos = playerPos.z;
        ballOldPos.x = ballPos.x;
        ballOldPos.z = ballPos.z;

        playerPos.z = playerNewPos;
        ballPos.x = ballNewPos.x;
        ballPos.z = ballNewPos.z;

        clientPlayer.transform.position = playerPos;
        clientBall.transform.position = ballPos;

        playerLerpTime = 0f;
        ballLerpTime = 0f;
    }

    void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEP, NetPacketReader reader, UnconnectedMessageType mType)
    {
        if (mType == UnconnectedMessageType.BasicMessage && netClient.PeersCount == 0 && reader.GetInt() == 1)
        {
            Debug.Log("Client || Response received. Connecting to " + remoteEP);
            netClient.Connect(remoteEP, "NetPong");
        }
    }

    void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        ping = latency;
    }

    void INetEventListener.OnConnectionRequest(ConnectionRequest request)
    {
        request.Reject();
    }
    #endregion
}