using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ENet;

public class Server : MonoBehaviour
{

    public void StartServer()
    {
        using (Host server = new Host())
        {
            Address address = new Address();

            address.Port = 1234;
            server.Create(address, 2);

            ENet.Event netEvent;

            if (true)
            {
                bool polled = false;

                while (!polled)
                {
                    if (server.CheckEvents(out netEvent) <= 0)
                    {
                        if (server.Service(0, out netEvent) <= 0)
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
                            Debug.Log("Client connected - ID: " + netEvent.Peer.ID +
                                                       ", IP: " + netEvent.Peer.IP);
                            break;

                        case ENet.EventType.Disconnect:
                            Debug.Log("Client disconnected - ID: " + netEvent.Peer.ID +
                                                          ", IP: " + netEvent.Peer.IP);
                            break;

                        case ENet.EventType.Timeout:
                            Debug.Log("Client timeout - ID: " + netEvent.Peer.ID +
                                                     ", IP: " + netEvent.Peer.IP);
                            break;

                        case ENet.EventType.Receive:
                            Debug.Log("Packet received from - ID:" + netEvent.Peer.ID +
                                                          ", IP: " + netEvent.Peer.IP +
                                                  ", Channel ID: " + netEvent.ChannelID +
                                                 ", Data length: " + netEvent.Packet.Length);
                            netEvent.Packet.Dispose();
                            break;
                    }
                }
            }

            server.Flush();
        }
    }
}
