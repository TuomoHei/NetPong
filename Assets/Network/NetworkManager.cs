#define ENET_LZ4
#define ENET_DLL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ENet;

public class NetworkManager : MonoBehaviour
{
    Server server;
    Client playerOne, playerTwo;

    void Start()
    {
        ENet.Library.Initialize();
    }
}
