using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class Score : MonoBehaviour
{
    public int scoreWin = 5;

    public TextMesh displayScoreP1;
    public TextMesh displayScoreP2;

    private int scoreP1 = 0;
    private int scoreP2 = 0;

    public void AddScore(int player)
    {
        if (player == 1)
        {
            scoreP1++;
        }

        else
        {
            scoreP2++;
        }
        
        if (scoreP1 >= scoreWin && scoreP1 > scoreP2)
        {
            Debug.Log("Player 1 wins");
            ResetScore();
        }

        if (scoreP2 >= scoreWin && scoreP2 > scoreP1)
        {
            Debug.Log("Player 2 wins");
            ResetScore();
        }

        displayScoreP1.text = scoreP1.ToString();
        displayScoreP2.text = scoreP2.ToString();
    }

    void ResetScore()
    {
        scoreP1 = 0;
        scoreP2 = 0;
    }
}