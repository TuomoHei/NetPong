using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLib;

public class UIController : MonoBehaviour
{

    [SerializeField] private GameObject UI;
    [SerializeField] private Server server;
    [SerializeField] private Client client;
    [SerializeField] private InputField ipField;


    private void Awake()
    {
        //ipField.text = LiteNetLib.NetUtils.GetLocalIp(LocalAddrType.IPv4);
    }

    public void HostButton()
    {
        server.StartServer();
        client.Connect("localhost");
        UI.SetActive(false);
    }

    public void ConnectButton()
    {
        client.Connect(ipField.text);
        UI.SetActive(false);
    }

}
