using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{

    [Header("Configuración de Marcador")]
    [SerializeField] private int winningScore = 11;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Efectos")]
    [SerializeField] private AudioClip scoreSound;
    [SerializeField] private AudioClip winSound;

    private int player1Score = 0;
    private int player2Score = 0;
    private AudioSource audioSource;
    private bool gameOver = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ResetScores();
    }

    public void AddScore(int playerNumber)
    {
        if (gameOver) return;

        // Reproducir sonido de puntaje
        if (scoreSound != null)
        {
            audioSource.PlayOneShot(scoreSound);
        }

        // Aumentar puntaje
        if (playerNumber == 1)
        {
            player1Score++;
            player1ScoreText.text = player1Score.ToString();
        }
        else if (playerNumber == 2)
        {
            player2Score++;
            player2ScoreText.text = player2Score.ToString();
        }

        // Verificar si hay ganador
        CheckForWinner();
    }

    private void CheckForWinner()
    {
        if (player1Score >= winningScore || player2Score >= winningScore)
        {
            gameOver = true;
            string winnerName = player1Score > player2Score ? "Jugador 1" : "Jugador 2";
            winnerText.text = $"{winnerName} Gana!";

            if (winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }

            // Mostrar panel de fin de juego
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }

    public void ResetScores()
    {
        player1Score = 0;
        player2Score = 0;
        player1ScoreText.text = "0";
        player2ScoreText.text = "0";
        gameOver = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}