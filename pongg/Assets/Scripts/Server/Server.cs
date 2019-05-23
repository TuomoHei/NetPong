using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;

public class Server : MonoBehaviour, INetEventListener
{

    public UIManager uiManager;
    [SerializeField] private int port = 2310;

    NetDataWriter dataWriter;
    private NetManager netManager;
    private Dictionary<long, NetworkedPlayer> playersDictionary;

    private void Awake()
    {
        dataWriter = new NetDataWriter();
        netManager = new NetManager(this);
        playersDictionary = new Dictionary<long, NetworkedPlayer>();
    }

    void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        netManager.Start(port);
        Debug.Log("[SERVER] Started succesfully");
        Debug.Log("[SERVER] Started listening to port: " + port);
        uiManager.UpdateServerState("Running");
    }

    private void Update()
    {
        if(netManager.IsRunning)
        {
            netManager.PollEvents();
            SendPlayerPositions();
        }
    }

    private void SendPlayerPositions()
    {
        Dictionary<long, NetworkedPlayer> sendPositionDictionary = new Dictionary<long, NetworkedPlayer>(playersDictionary);

        foreach (var sendToPlayer in sendPositionDictionary)
        {
            if(sendToPlayer.Value == null)
            {
                continue;
            }

            dataWriter.Reset();
            dataWriter.Put((int)NetworkTags.PlayerPositionsArray);

            int amountPlayersMoved = 0;

            foreach (var posPlayers in sendPositionDictionary)
            {
                //if(sendToPlayer.Key == posPlayers.Key)
                //{
                //    continue;
                //}

                if (!posPlayers.Value.Moved)
                {
                    continue;
                }

                dataWriter.Put(posPlayers.Key);

                dataWriter.Put(posPlayers.Value.X);
                dataWriter.Put(posPlayers.Value.Y);
                dataWriter.Put(posPlayers.Value.Z);

                amountPlayersMoved++;
            }

            if(amountPlayersMoved > 0)
            {
                sendToPlayer.Value.NetPeer.Send(dataWriter, DeliveryMethod.Sequenced);
            }
        }

        foreach (var player in playersDictionary)
        {
            player.Value.Moved = false;
        }
    }

    #region Connetion


    // SEND OLD PLAYER POSITION TO NEW PLAYER
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] OnPeerConnected " + peer.EndPoint.Address);
        uiManager.NewConnection(peer.Id.ToString());

        NetDataWriter netDataWriter = new NetDataWriter();
        netDataWriter.Reset();

        netDataWriter.Put((int)NetworkTags.PlayerPositionsArray);
        foreach (var p in playersDictionary)
        {
            netDataWriter.Put(p.Key);
            netDataWriter.Put(p.Value.X);
            netDataWriter.Put(p.Value.Y);
            netDataWriter.Put(p.Value.Z);
        }

        peer.Send(netDataWriter, DeliveryMethod.ReliableOrdered);

        if (!playersDictionary.ContainsKey(peer.Id))
        {
            Debug.Log(peer.Id);
            playersDictionary.Add(peer.Id, new NetworkedPlayer(peer));
        }

        playersDictionary[peer.Id].Moved = true;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (playersDictionary.ContainsKey(peer.Id))
        {
            Debug.Log("[SERVER] OnPeerDisconnected " + peer.EndPoint.Address);
            playersDictionary.Remove(peer.Id);
        }
    }  

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.Log("[SERVER] OnNetworkError in " + endPoint.Address + " " + socketError);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        if (reader.RawData == null)
        {
            return;
        }

        NetworkTags networkTag = (NetworkTags)reader.GetInt();
        if (networkTag == NetworkTags.PlayerPosition)
        {
            float x = reader.GetFloat();
            float y = reader.GetFloat();
            float z = reader.GetFloat();

            Debug.Log("Got position packet : " + x + " " + y + " " + z);

            playersDictionary[peer.Id].X = x;
            playersDictionary[peer.Id].Y = y;
            playersDictionary[peer.Id].Z = z;

            playersDictionary[peer.Id].Moved = true;
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        Debug.Log("[SERVER] OnNetworkReceiveUnconnected");
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        Debug.Log("[SERVER] OnConnectionRequest accepted");
        request.AcceptIfKey("NetPong");
    }


    #endregion

}
