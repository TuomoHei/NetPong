using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Scripts.Client;
using Shared.Enums;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Client : MonoBehaviour, INetEventListener
{
    public Player player;
    private Vector3 lastNetworkPosition = Vector3.zero;

    private float lastDistance = 0.0f;
    const float MIN_DISTANCE_TO_SEND_POSITION = 0.01f;

    private NetDataWriter dataWriter;
    private NetManager netClient;
    private NetPeer server; //server peer

    public GameObject netPlayer;
    private Dictionary<long, NetPlayer> netDictionary;

    private string thisIP;

    public void Start()
    {
        netClient.Start();

        if (netClient.Start())
        {
            dataWriter = new NetDataWriter();
            netDictionary = new Dictionary<long, NetPlayer>();
            netClient = new NetManager(this);

            Debug.Log("Client || NetManager started.");
        }

        else
        {
            Debug.LogError("Client || Couldn't start NetManager.");
        }
    }

    public void Connect(string ip, int port)
    {
        netClient.Connect(ip, port, "NetPong"); //2310-2320
    }

    private void FixedUpdate()
    {
        if (netClient != null)
        {
            if (netClient.IsRunning)
            {
                netClient.PollEvents();

                lastDistance = Vector3.Distance(lastNetworkPosition, player.transform.position);

                if (lastDistance >= MIN_DISTANCE_TO_SEND_POSITION)
                {
                    dataWriter.Reset();
                    dataWriter.Put((int)NetworkTags.PlayerPosition);
                    dataWriter.Put(player.transform.position.z);

                    server.Send(dataWriter, DeliveryMethod.Sequenced);

                    lastNetworkPosition = player.transform.position;
                }
            }
        }

        foreach (var player in netDictionary)
        {
            if (!player.Value.isGameObjectAdded)
            {
                player.Value.isGameObjectAdded = true;
                player.Value.GameObject = Instantiate(netPlayer, player.Value.Position, Quaternion.identity);
            }

            else
            {
                player.Value.GameObject.transform.position = player.Value.Position;
            }
        }
    }

    private void OnApplicationQuit()
    {
       if (netClient != null)
        {
            if (netClient.IsRunning)
            {
                netClient.Stop();
            }
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        if (reader.RawData == null)
        {
            return;
        }

        Debug.Log("Client: " + reader.RawData.Length);

        if (reader.RawData.Length >= 4)
        {
            NetworkTags netTag = (NetworkTags)reader.GetInt();

            if (netTag == NetworkTags.PlayerPositionsArray)
            {
                int lengthArray = (reader.RawData.Length - 4) / (sizeof(long) + sizeof(float) * 3);
                Debug.Log("Positions array data num: " + lengthArray);

                for (int i = 0; i < lengthArray; i++)
                {
                    long playerId = reader.GetLong();

                    if (!netDictionary.ContainsKey(playerId))
                    {
                        netDictionary.Add(playerId, new NetPlayer());
                    }

                    netDictionary[playerId].X = reader.GetFloat();
                    netDictionary[playerId].Y = reader.GetFloat();
                    netDictionary[playerId].Z = reader.GetFloat();
                }
            }
        }
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
        if (reader.RawData == null)
        {
            return;
        }

        Debug.Log("Client || OnNetworkReceive: " + reader.RawData.Length);

        if (reader.RawData.Length >= 4)
        {
            NetworkTags netTag = (NetworkTags)reader.GetInt();

            if (netTag == NetworkTags.PlayerPositionsArray)
            {
                int lengthArray = (reader.RawData.Length - 4) / (sizeof(long) + sizeof(float) * 3);

                Debug.Log("Client || Positions array data num: " + lengthArray);

                for (int i = 0; i < lengthArray; i++)
                {
                    long playerId = reader.GetLong();

                    if (!netDictionary.ContainsKey(playerId))
                    {
                        netDictionary.Add(playerId, new NetPlayer());
                    }

                    netDictionary[playerId].Z = reader.GetFloat();
                }
            }
        }
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

    }

    void INetEventListener.OnConnectionRequest(ConnectionRequest request)
    {
        request.Reject();
    }
    #endregion
}