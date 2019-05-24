using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLib;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Text ipText;
    [SerializeField] private Text serverState;
    [SerializeField] private Text connetedList;

    void Start()
    {
        ipText.text = "IP: " + NetUtils.GetLocalIp(LocalAddrType.IPv4);
    }

    public void UpdateServerState(string state)
    {
        serverState.text = "ServerState: " + state;
    }

    public void NewConnection(string id)
    {
        connetedList.text += id + " ";
    }
}
