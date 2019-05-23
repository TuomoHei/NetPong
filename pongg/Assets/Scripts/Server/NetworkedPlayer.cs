using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class NetworkedPlayer
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public NetPeer NetPeer { get; set; }
    public bool Moved { get; set; }


    public NetworkedPlayer(NetPeer peer)
    {
        NetPeer = peer;

        X = 0.0f;
        Y = 0.0f;
        Z = 0.0f;

        Moved = false;
    }
}
