using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;

public class Client : MonoBehaviour, INetEventListener
{
    private NetManager netClient;

    [SerializeField]
    private GameObject clientBall;

    [SerializeField]
    private GameObject clientBallInterpolated;

    private Vector3 newPos;
    private Vector3 oldPos;

    private float lerpTime;

    private int ping;

    void Start()
    {
        netClient = new NetManager(this)
        {
            UnconnectedMessagesEnabled = true,
            UpdateTime = 15
        };

        netClient.Start();
    }

    void Update()
    {
        netClient.PollEvents();

        var peer = netClient.FirstPeer;

        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            var newPos = clientBallInterpolated.transform.position;

            newPos.x = Mathf.Lerp(oldPos.x, newPos.x, lerpTime);
            newPos.z = Mathf.Lerp(oldPos.z, newPos.z, lerpTime);

            clientBallInterpolated.transform.position = newPos;

            lerpTime += Time.deltaTime / Time.fixedDeltaTime;
        }

        else
        {
            netClient.SendBroadcast(new byte[] { 1 }, 5000); //byte[] data, int port
        }
    }

    void OnDestroy()
    {
        if (netClient != null)
        {
            netClient.Stop();
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("Client: Connected to " + peer.EndPoint);
    }

    public void OnNetworkError(IPEndPoint ep, SocketError socketErrorCode)
    {
        Debug.Log("Client: Received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod delMethod)
    {
        newPos.x = reader.GetFloat();
        newPos.z = reader.GetFloat();

        var pos = clientBall.transform.position;

        oldPos.x = pos.x;
        oldPos.z = pos.z;

        pos.x = newPos.x;
        pos.z = newPos.z;

        clientBall.transform.position = pos;

        lerpTime = 0f;
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEP, NetPacketReader reader, UnconnectedMessageType mType)
    {
        if (mType == UnconnectedMessageType.BasicMessage && netClient.PeersCount == 0 && reader.GetInt() == 1)
        {
            Debug.Log("Client: Response received. Connecting to " + remoteEP);
            netClient.Connect(remoteEP, "NetPong");
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        ping = latency;
    }

    public void OnConnectionRequest(ConnectionRequest conReq)
    {

    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo dcInfo)
    {
        Debug.Log("Client: Disconnected. Reason: " + dcInfo.Reason);
    }
}