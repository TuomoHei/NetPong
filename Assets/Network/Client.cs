using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ENet;

public class Client : MonoBehaviour
{
    void Start()
    {
        using (Host client = new Host())
        {
            Address address = new Address();

            address.SetHost(address.GetIP());
            address.Port = 1234;
            client.Create();

            Peer peer = client.Connect(address);

            ENet.Event netEvent;

            if (true)
            {
                bool polled = false;

                while (!polled)
                {
                    if (client.CheckEvents(out netEvent) <= 0)
                    {
                        if (client.Service(0, out netEvent) <= 0)
                        {
                            break;
                        }

                        polled = true;
                    }

                    switch (netEvent.Type)
                    {
                        case ENet.EventType.None:
                            break;

                        case ENet.EventType.Connect:
                            Debug.Log("Client connected to server");
                            break;

                        case ENet.EventType.Disconnect:
                            Debug.Log("Client disconnected from server");
                            break;

                        case ENet.EventType.Timeout:
                            Debug.Log("Client connection timeout");
                            break;

                        case ENet.EventType.Receive:
                            Debug.Log("Packet received from server - Channel ID: " + netEvent.ChannelID + 
                                                                 ", Data length: " + netEvent.Packet.Length);
                            netEvent.Packet.Dispose();
                            break;
                    }
                }
            }

            client.Flush();
        }
    }
}
