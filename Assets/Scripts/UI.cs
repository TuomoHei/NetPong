using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLib;

public class UI : MonoBehaviour
{
    [SerializeField]
    private GameObject uiObject;
    
    //[SerializeField]
    //private Server server;

    [SerializeField]
    private Client client;

    [SerializeField]
    private InputField ip;

    [SerializeField]
    private InputField port;

    private void Awake()
    {
        ip.text = NetUtils.GetLocalIp(LocalAddrType.IPv4);
    }

    public void OnHostClick()
    {
        //this client is P1
        //server.StartServer();
        client.Start();
        client.Connect("localhost", int.Parse(port.text)); //2310-2320
        uiObject.SetActive(false);
    }

    public void OnJoinClick()
    {
        //this client is P2
        client.Start();
        client.Connect(ip.text, int.Parse(port.text)); //2310-2320
        uiObject.SetActive(false);
    }
}
