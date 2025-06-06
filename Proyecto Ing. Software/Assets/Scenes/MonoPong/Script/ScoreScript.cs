/*
this script manages the score
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreScript : MonoBehaviour {

    /*<summary>the current score ...ie number of bounces<summary>*/
    public int Score = 0;

    //method for updating the score
    public void UpdateScore(int Delta)
    {
        Score += Delta;

        gameObject.GetComponent<Text>().text = Score.ToString();
    }

    //method used to reset the score
    public void ResetScore()
    {
        Score = 0;
        gameObject.GetComponent<Text>().text = Score.ToString();
    }
}
