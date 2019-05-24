using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Text playerId;

    void Update ()
    {
        if (player.id == 1)
        {
            playerId.text = "Host";
        }

        if (player.id == 0)
        {
            playerId.text = "Client";
        }
    }
}