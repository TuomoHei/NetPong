using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int player;

    public Score score;

    public void GetPoint()
    {
        score.AddScore(player);
    }
}
