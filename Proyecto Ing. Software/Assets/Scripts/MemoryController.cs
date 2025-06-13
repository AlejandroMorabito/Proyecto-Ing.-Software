using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MemoryController : MonoBehaviour
{
    // [Asignar estos en el Inspector]
    public Image[] uiImages = new Image[36];
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI pointsText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject introCanvas;
    public GameObject juegoCanvas;
    public GameObject HUDCanvas;
    public Button startButton;
    public Button exitButton;

    private readonly Color color0 = new Color(0f, 42f/255f, 152f/255f);
    private readonly Color color1 = new Color(47f/255f, 104f/255f, 255f/255f);
    private readonly Color wrongColor = new Color(1f, 0.2157f, 0.1843f);

    private List<int> imageValues = new List<int>();
    private List<int> correctIndices = new List<int>();
    private int lives = 3;
    private bool gameActive = false;
    private int score = 0;
    private int totalPoints = 0;
    private Coroutine gameRoutine;
    public PlayerController playerController;

    // Valores de recompensa/castigo
    [Header("Recompensas")]
    public int conocimientoPorGanar = 2;
    public int estresPorGanar = 1;
    public int conocimientoPorPerder = -1;
    public int estresPorPerder = 2;

    void Start()
    {
        startButton.onClick.AddListener(StartGameFromIntro);
        exitButton.onClick.AddListener(ExitGame);
        
        introCanvas.SetActive(false);
        juegoCanvas.SetActive(false);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        
        SetGameElementsActive(false);
    }

    void Update()
    {
        // Verificación del nivel de estrés
        if (PlayerStatsManager.Instance != null && PlayerStatsManager.Instance.Estres >= 100)
        {
            ExitGame();
        }
    }


    public void ExitGame()
    {
        juegoCanvas.SetActive(false);
        if (playerController != null)
        {
            playerController.enabled = true;
            GetComponent<Canvas>().enabled = true;
        }
    }

    public void StartGameFromIntro()
    {
        introCanvas.SetActive(false);
        juegoCanvas.SetActive(true);
        SetGameElementsActive(true);
        
        totalPoints = 0;
        UpdatePointsUI();
        StartNewGame();
    }

    private void SetGameElementsActive(bool active)
    {
        foreach(var img in uiImages)
        {
            if(img != null) img.gameObject.SetActive(active);
        }
        countdownText.gameObject.SetActive(active);
        livesText.gameObject.SetActive(active);
        pointsText.gameObject.SetActive(active);
    }

    public void StartNewGame()
    {
        if(gameRoutine != null) 
        {
            StopCoroutine(gameRoutine);
        }

        InitializeGameState();
        gameRoutine = StartCoroutine(GameLoop());
    }

    private void InitializeGameState()
    {
        gameActive = false;
        lives = 3;
        score = 0;
        imageValues.Clear();
        correctIndices.Clear();
        
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);

        foreach(var img in uiImages)
        {
            if(img != null) img.color = color0;
        }

        for(int i = 0; i < uiImages.Length; i++)
        {
            imageValues.Add(0);
        }

        livesText.text = "Vidas: " + lives;
    }

    private void UpdatePointsUI()
    {
        pointsText.text = "Puntos: " + totalPoints;
    }

    private IEnumerator GameLoop()
    {
        SetRandomHighlightedImages(6);
        countdownText.text = "Memoriza";
        yield return new WaitForSeconds(5f);

        foreach(int index in correctIndices)
        {
            if(uiImages[index] != null)
            {
                uiImages[index].color = color0;
                imageValues[index] = 0;
            }
        }

        countdownText.text = "";
        gameActive = true;
    }

    private void SetRandomHighlightedImages(int count)
    {
        List<int> availableIndices = new List<int>();
        for(int i = 0; i < uiImages.Length; i++)
        {
            if(uiImages[i] != null) availableIndices.Add(i);
        }

        for(int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(i, availableIndices.Count);
            int temp = availableIndices[i];
            availableIndices[i] = availableIndices[randomIndex];
            availableIndices[randomIndex] = temp;

            int index = availableIndices[i];
            uiImages[index].color = color1;
            imageValues[index] = 1;
            correctIndices.Add(index);
        }
    }

    public void OnSquareClicked(int index)
    {
        if(!gameActive || imageValues[index] == 2) return;

        if(correctIndices.Contains(index))
        {
            uiImages[index].color = color1;
            imageValues[index] = 2;
            score++;
            
            if(CheckWinCondition())
            {
                totalPoints++;
                UpdatePointsUI();
                // Añadir recompensas al ganar
                PlayerStatsManager.Instance.AddConocimiento(conocimientoPorGanar);
                PlayerStatsManager.Instance.AddEstres(estresPorGanar);
                StartCoroutine(EndGame(true));
            }
        }
        else
        {
            uiImages[index].color = wrongColor;
            lives--;
            livesText.text = "Vidas: " + lives;
            
            if(lives <= 0)
            {
                totalPoints = Mathf.Max(0, totalPoints - 1);
                UpdatePointsUI();
                // Añadir penalizaciones al perder
                PlayerStatsManager.Instance.AddConocimiento(conocimientoPorPerder);
                PlayerStatsManager.Instance.AddEstres(estresPorPerder);
                StartCoroutine(EndGame(false));
            }
        }
    }

    private bool CheckWinCondition()
    {
        foreach(int index in correctIndices)
        {
            if(imageValues[index] != 2) return false;
        }
        return true;
    }

    private IEnumerator EndGame(bool won)
    {
        gameActive = false;
        
        if(won)
        {
            winPanel.SetActive(true);
            resultText.text = $"¡Ganaste! Puntos: +1\nTotal: {totalPoints}";
        }
        else
        {
            gameOverPanel.SetActive(true);
            resultText.text = $"Perdiste. Puntos: -1\nTotal: {totalPoints}";
        }

        yield return new WaitForSeconds(2f);
        StartNewGame();
    }
}