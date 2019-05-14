using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used for networking
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Code.Shared;


public class Server : MonoBehaviour, INetEventListener
{

    public int port = 2310; // can be changed between 2310-2320

    private NetManager netManager;
    private NetPacketProcessor netPacketProcessor;

    public const int MAX_PLAYERS = 2;
    private LogicTimer logicTimer;
    private readonly NetDataWriter cachedWriter = new NetDataWriter();
    private ushort serverTick;
    //private ServerPlayerManager playerManager;

    private PlayerInputPacket cachedCommand = new PlayerInputPacket();
    private ServerState serverState;
    public ushort Tick => serverTick;

    public void StartServer()
    {
        if (netManager.IsRunning)
            return;
        netManager.Start(port);
        logicTimer.Start();

        Debug.Log("Server started");
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        logicTimer = new LogicTimer(OnLogicUpdate);
        netPacketProcessor = new NetPacketProcessor();
        //playerManager = new ServerPlayerManager(this);

        //register auto serializable vector2
        netPacketProcessor.RegisterNestedType((w, v) => w.Put(v), r => r.GetVector2());

        //register auto serializable PlayerState
        //netPacketProcessor.RegisterNestedType<PlayerState>();
        //
        //netPacketProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);
        netManager = new NetManager(this)
        {
            AutoRecycle = true
        };
    }

    private void OnDestroy()
    {
        netManager.Stop();
        logicTimer.Stop();
    }



    // logicTimer calls this action every frame for synced update
    private void OnLogicUpdate()
    {
        //serverTick = (ushort)((serverTick + 1) % NetworkGeneral.MaxGameSequence);
        //playerManager.LogicUpdate();
        //if (serverTick % 2 == 0)
        //{
        //    serverState.Tick = serverTick;
        //    serverState.PlayerStates = playerManager.PlayerStates;
        //    int pCount = playerManager.Count;
        //
        //    foreach (ServerPlayer p in playerManager)
        //    {
        //        int statesMax = p.AssociatedPeer.GetMaxSinglePacketSize(DeliveryMethod.Unreliable) - ServerState.HeaderSize;
        //        statesMax /= PlayerState.Size;
        //
        //        for (int s = 0; s < (pCount - 1) / statesMax + 1; s++)
        //        {
        //            //TODO: divide
        //            serverState.LastProcessedCommand = p.LastProcessedCommandId;
        //            serverState.PlayerStatesCount = pCount;
        //            serverState.StartState = s * statesMax;
        //            p.AssociatedPeer.Send(WriteSerializable(PacketType.ServerState, serverState), DeliveryMethod.Unreliable);
        //        }
        //    }
        //}
    }

    private void Update()
    {
        netManager.PollEvents();
        logicTimer.Update();
    }

    private void OnInputReceived(NetPacketReader reader, NetPeer peer)
    {
        if(peer.Tag == null)
        {
            return;
        }

        // dezerialize 
        cachedCommand.Deserialize(reader);
        var player = (ServerPlayer)peer.Tag;
        player.ApplyInput(cachedCommand, logicTimer.fd);
    }

    #region Connecting
    public void OnConnectionRequest(ConnectionRequest request)
    {
        // we get connection request
        request.AcceptIfKey("NetPong");

        if(request.Result == ConnectionRequestResult.Accept)
        {
            Debug.Log("Connection accepted");
        }
        else if(request.Result == ConnectionRequestResult.Reject)
        {
            Debug.Log("Connection got rejected");
        }
        else
        {
            Debug.Log("Connection was ignored");
        }

        // if there is less than 3 connections assaing the player a pong and controls
        // assaing the player to the other pong and give controls
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.Log("[S] NetworkError: " + socketError);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        // this is called around once per second
        if (peer.Tag != null)
        {
            var p = (ServerPlayer)peer.Tag;
            p.Ping = latency;
        }
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        byte packetType = reader.GetByte();

        if (packetType >= NetworkGeneral.PacketTypesCount) return;

        PacketType pt = (PacketType)packetType;
        switch (pt)
        {
            case PacketType.Movement:
                OnInputReceived(reader, peer);
                break;
            case PacketType.Serialized:
                // packetProcessor.ReadAllPackets(reader, peer);
                break;
            default:
                Debug.Log("Unhandled packet: " + pt);
                break;
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We got connection: " + peer.EndPoint);
        NetDataWriter writer = new NetDataWriter();
        writer.Put("Welcome to the game!");
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
