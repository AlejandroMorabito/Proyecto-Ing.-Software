using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public int Score { get; private set; }
    public TMP_Text scoreText; // Asigna un TMP_Text en el inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoint()
    {
        Score++;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Score;
        }
    }
}