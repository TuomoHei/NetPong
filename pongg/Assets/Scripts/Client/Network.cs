using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Network : MonoBehaviour, INetEventListener
{

    public int id = -1;

    public Player player;
    private Vector3 lastNetworkedPosition = Vector3.zero;

    private float lastDistance = 0.0f;
    const float MIN_DISTANCE_TO_SEND_POSITION = 0.01f;

    private NetDataWriter dataWriter;
    private NetManager clientNetManager;
    private NetPeer serverPeer;

    public GameObject netPlayerPrefab;
    private Dictionary<long, NetPlayer> netPlayersDictionary;

    private void Awake()
    {
        netPlayersDictionary = new Dictionary<long, NetPlayer>();
        dataWriter = new NetDataWriter();
        clientNetManager = new NetManager(this);
    }

    private void Start()
    {
        clientNetManager.Start();
        clientNetManager.Connect("localhost", 2310, "NetPong");
    }

    private void FixedUpdate()
    {
        clientNetManager.PollEvents();

        if (clientNetManager != null)
        {
            if (clientNetManager.IsRunning)
            {
                clientNetManager.PollEvents();
        
                lastDistance = Vector3.Distance(lastNetworkedPosition, player.transform.position);
                if (lastDistance >= MIN_DISTANCE_TO_SEND_POSITION)
                {
                    dataWriter.Reset();
        
                    dataWriter.Put((int)NetworkTags.PlayerPosition);
                    dataWriter.Put(player.transform.position.x);
                    dataWriter.Put(player.transform.position.y);
                    dataWriter.Put(player.transform.position.z);
        
                    serverPeer.Send(dataWriter, DeliveryMethod.Sequenced);
        
                    lastNetworkedPosition = player.transform.position;
                }
            }
        }

        foreach (var player in netPlayersDictionary)
        {
            if (!player.Value.GameObjectAdded)
            {
                player.Value.GameObjectAdded = true;
                player.Value.GameObject = Instantiate(netPlayerPrefab, player.Value.Position, Quaternion.identity);
                Debug.Log("Spawned new player");
            }
            else
            {
                player.Value.GameObject.transform.position = player.Value.Position;
            }
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        serverPeer = peer;
        Debug.Log("[CLIENT] OnPeerConnected");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        throw new System.NotImplementedException();
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        throw new System.NotImplementedException();
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        if (reader.RawData == null)
        {
            return;
        }

        if (reader.RawData.Length >= 4)
        {
            NetworkTags networkTag = (NetworkTags)reader.GetInt();
            if (networkTag == NetworkTags.PlayerPositionsArray)
            {
                int lengthArr = (reader.RawData.Length - 4) / (sizeof(long) + sizeof(float) * 3);

                Debug.Log("Got positions array data num : " + lengthArr);

                for (int i = 0; i < lengthArr; i++)
                {
                    long playerid = reader.GetLong();

                    if (!netPlayersDictionary.ContainsKey(playerid))
                    {
                        netPlayersDictionary.Add(playerid, new NetPlayer());
                        id = (int)playerid;
                    }

                    netPlayersDictionary[playerid].X = reader.GetFloat();
                    netPlayersDictionary[playerid].Y = reader.GetFloat();
                    netPlayersDictionary[playerid].Z = reader.GetFloat();
                }
            }
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        throw new System.NotImplementedException();
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
    }
}