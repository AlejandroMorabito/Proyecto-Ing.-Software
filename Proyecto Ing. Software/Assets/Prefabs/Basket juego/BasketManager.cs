using UnityEngine;
using TMPro;

public class BasketManager : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText; // Asigna este campo desde el Inspector

    private void Start()
    {
        UpdateScoreText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        score++;
        UpdateScoreText();
        Debug.Log("Score: " + score);
    }

    // Este método lo asignas al botón en el Inspector
    public void EnviarEstres()
    {
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.AddEstres(score);
            Debug.Log("Estres agregado: " + score);
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
