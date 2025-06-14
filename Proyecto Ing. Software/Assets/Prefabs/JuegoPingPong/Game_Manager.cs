using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    [Header("Configuración de Marcador")]
    [SerializeField] private int winningScore = 11;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("Efectos")]
    [SerializeField] private AudioClip scoreSound;
    [SerializeField] private AudioClip winSound;

    [Header("Pausa")]
    [SerializeField] private GameObject pauseCanvas; // Canvas de pausa
    [SerializeField] private GameObject exitCanvas;  // Canvas de confirmación de salida
    [SerializeField] private GameObject gameCanvas;  // Canvas del juego

    private int player1Score = 0;
    private int player2Score = 0;
    private AudioSource audioSource;
    private bool gameOver = false;
    private bool isPaused = false;
    [SerializeField] public bool IsAI;

    public bool getAI() => IsAI;
    public void setAI(bool isAI) => IsAI = isAI;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ResetScores();
    }

    private void Update()
    {
        // Si se presiona ESC y el juego no ha terminado, alterna la pausa
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void AddScore(int playerNumber)
    {
        if (gameOver) return;

        if (scoreSound != null)
            audioSource.PlayOneShot(scoreSound);

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

        CheckForWinner();
    }

    private void CheckForWinner()
    {
        if (player1Score >= winningScore || player2Score >= winningScore)
        {
            gameOver = true;
            string winnerName = player1Score > player2Score ? "Jugador 1" : "Jugador 2";
            if (winnerText != null)
            {
                winnerText.text = $"{winnerName} GANA!";
                winnerText.gameObject.SetActive(true);
            }

            if (gameOverText != null)
                gameOverText.gameObject.SetActive(true);

            if (winSound != null)
                audioSource.PlayOneShot(winSound);

            // Desactiva la pelota para que no siga jugando
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            if (ball != null)
                ball.SetActive(false);

            // Llama a AddEstres(-11) de PlayerStatsManager al terminar la partida
            if (PlayerStatsManager.Instance != null)
                PlayerStatsManager.Instance.AddEstres(-11);
        }
    }

    public void ResetScores()
    {
        player1Score = 0;
        player2Score = 0;
        if (player1ScoreText != null) player1ScoreText.text = "0";
        if (player2ScoreText != null) player2ScoreText.text = "0";
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (winnerText != null) winnerText.gameObject.SetActive(false);
        if (audioSource != null) audioSource.Stop();
        gameOver = false;
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

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
    }

    // Llama este método desde el botón "Salir" del pauseCanvas
    public void ShowExitCanvas()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        if (exitCanvas != null && gameCanvas != null)
        {
            RestartGame();
            ResumeGame();
            exitCanvas.SetActive(true);
            gameCanvas.SetActive(false);
        }
    }

    // Sistema de puntos para integración con Ball.cs
    public void AddPointToPlayer(int player)
    {
        AddScore(player);
    }

    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f; // Por si el juego estaba pausado
        SceneManager.LoadScene(sceneName);
    }
}