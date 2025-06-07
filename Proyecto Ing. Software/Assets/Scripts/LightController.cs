using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LightController : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Asigna las 16 imágenes UI (4x4 grid)")]
    public Image[] uiImages = new Image[16];
    [Tooltip("Texto para mostrar la puntuación")]
    public TextMeshProUGUI scoreText;
    [Tooltip("Texto para mostrar el nivel actual")]
    public TextMeshProUGUI levelText;
    
    [Header("Game Settings")]
    public float delayBetweenGames = 3f;
    public GameObject introCanvas; // Canvas introductorio
    public GameObject juegoCanvas;
    public GameObject HUDCanvas;
    private readonly Color color0 = new Color(0.0627f, 0.4745f, 0f); // #107900
    private readonly Color color1 = new Color(0.1255f, 1f, 0f);      // #20FF00
    private List<int> imageValues = new List<int>();
    private const int gridSize = 4;
    private bool allSameColor = false;
    private bool isAnimating = false;
    private int currentLevel = 1;
    private int totalScore = 0;
    public PlayerController playerController;
    public Button exitButton;

    void Start()
    {
        introCanvas.SetActive(false);
        juegoCanvas.SetActive(false);
        InitializeGame();
        UpdateUI();
        exitButton.onClick.AddListener(OnExitButtonPressed);
    }

    public void OnExitButtonPressed()
    {
        // Reiniciar puntuación y nivel
        ResetGameState();
        
        // Desactivar canvas del juego
        juegoCanvas.SetActive(false);

        // Reactivar controles del jugador
        if (playerController != null)
        {
            playerController.enabled = true;
            HUDCanvas.SetActive(true);
        }
    }

    void ResetGameState()
    {
        totalScore = 0;
        currentLevel = 1;
        UpdateUI();
        InitializeGame();
    }

    void InitializeGame()
    {
        allSameColor = false;
        isAnimating = false;
        imageValues.Clear();
        
        if (uiImages == null || uiImages.Length != 16)
        {
            Debug.LogError("Debes asignar exactamente 16 imágenes (4x4 grid)");
            return;
        }

        for (int i = 0; i < uiImages.Length; i++)
        {
            if (uiImages[i] == null)
            {
                Debug.LogWarning($"Imagen {i} no asignada - Saltando...");
                imageValues.Add(0);
                continue;
            }

            if (!uiImages[i].TryGetComponent<Button>(out var button))
            {
                button = uiImages[i].gameObject.AddComponent<Button>();
            }

            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                if (!allSameColor && !isAnimating) 
                {
                    ToggleImageAndCross(index);
                    CheckSolution();
                }
            });

            imageValues.Add(0);
        }

        GenerateSolvablePuzzle();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Puntos: {totalScore}";
        
        if (levelText != null)
            levelText.text = $"Nivel: {currentLevel}";
    }

    void ToggleImageAndCross(int index)
    {
        ToggleSingleImage(index);
        
        int row = index / gridSize;
        int col = index % gridSize;

        if (row > 0) ToggleSingleImage(index - gridSize);
        if (row < gridSize - 1) ToggleSingleImage(index + gridSize);
        if (col > 0) ToggleSingleImage(index - 1);
        if (col < gridSize - 1) ToggleSingleImage(index + 1);
    }

    void ToggleSingleImage(int index)
    {
        imageValues[index] = 1 - imageValues[index];
        UpdateImageColor(index);
    }

    void UpdateImageColor(int index)
    {
        uiImages[index].color = imageValues[index] == 0 ? color0 : color1;
    }

    void CheckSolution()
    {
        allSameColor = true;

        foreach (int value in imageValues)
        {
            if (value != 1)
            {
                allSameColor = false;
                break;
            }
        }

        if (allSameColor)
        {
            totalScore++; // Aumentar puntuación al ganar
            StartCoroutine(CompleteLevelRoutine());
        }
    }

    IEnumerator CompleteLevelRoutine()
    {
        isAnimating = true;
        yield return StartCoroutine(WinAnimation());
        
        yield return new WaitForSeconds(delayBetweenGames);
        
        currentLevel++;
        InitializeGame();
        UpdateUI();
    }

    IEnumerator WinAnimation()
    {
        for (int i = 0; i < 5; i++)
        {
            foreach (var img in uiImages)
            {
                if (img != null) img.color = Color.white;
            }
            yield return new WaitForSeconds(0.2f);
            
            foreach (var img in uiImages)
            {
                if (img != null) img.color = color1;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void GenerateSolvablePuzzle()
    {
        for (int i = 0; i < imageValues.Count; i++)
        {
            imageValues[i] = 1;
            UpdateImageColor(i);
        }

        int moves = Mathf.Min(5 + currentLevel, 15);
        List<int> solutionMoves = new List<int>();

        for (int i = 0; i < moves; i++)
        {
            int randomIndex = Random.Range(0, 16);
            solutionMoves.Add(randomIndex);
        }

        foreach (int move in solutionMoves)
        {
            ToggleImageAndCross(move);
        }
    }
}