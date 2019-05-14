using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

class Packet
{
    public float PlayerPos { get; set; }
    public float BallPosX { get; set; }
    public float BallPosZ { get; set; }
}
