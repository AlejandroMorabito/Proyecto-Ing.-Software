using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum LevelType
{
    Mixed,       // Letras y números (A1, B2, etc.)
    NumbersOnly, // Solo números (00-99)
    LettersOnly, // Solo letras (AA, BB, etc.)
    Braille      // Nivel en Braille (⠁, ⠃, etc.)
}

public class HackingManager : MonoBehaviour
{
    [Header("Textos UI")]
    public TMP_Text[] gridTexts;
    public TMP_Text[] guideTexts;
    public TMP_Text timerText;
    public TMP_Text scoreText; // Nuevo: Texto para mostrar el puntaje

    [Header("playerController")]
    public PlayerController playerController;

    [Header("Fuentes")]
    public TMP_FontAsset normalFont;
    public TMP_FontAsset brailleFont;

    [Header("Objetos")]
    public GameObject startButton;
    public GameObject exitButton;
    public GameObject canvasHacking;

    [Header("Configuración")]
    public int winPoints = 2;    // Puntos al ganar
    public int losePoints = -1;  // Puntos al perder

    private string[] grid = new string[80];
    private List<string> targetSequence = new List<string>();
    private int currentStartIndex = 0;
    private float gameTime = 30f;
    private bool gameActive = true;
    private Coroutine rotationCoroutine;
    private List<int> currentSelection = new List<int>();
    private LevelType currentLevelType;
    private int currentScore = 0; // Variable para almacenar el puntaje

    // Colores
    private Color normalColor = Color.white;
    private Color selectedColor = Color.red;
    private Color correctColor = Color.green;
    private Color incorrectColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);

    void Start()
    {
        startButton.SetActive(true);
        startButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(StartGameFromButton);
    exitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnExitButtonPressed);
        InitializeGrid();
        GenerateTargetSequence();
        HideGrid();
        UpdateScoreText(); // Inicializa el texto del puntaje
    }

    // --- Nuevo método para actualizar el texto del puntaje ---
    void UpdateScoreText()
    {
        scoreText.text = $"Puntos: {currentScore}";
    }
    public void StartGameFromButton()
    {
        startButton.SetActive(false);
        ShowGrid(); // Mostrar todo al iniciar

        // Seleccionar un tipo de nivel aleatorio
        currentLevelType = (LevelType)Random.Range(0, 3);

        StartGame();
    }

    IEnumerator AutoRestartGame()
    {
        yield return new WaitForSeconds(3f); // Espera 3 segundos

        // Reinicia completamente el juego
        InitializeGrid();
        GenerateTargetSequence();
        UpdateSelection(0);
        StartGame();
    }

    void Update()
    {
        if (!gameActive) return;

        HandleInput();
        UpdateTimer();
    }

    void InitializeGrid()
    {
        // Primero llena toda la cuadrícula con celdas aleatorias
        for (int i = 0; i < 80; i++)
        {
            grid[i] = GenerateRandomCell();
        }

        // Luego genera y coloca la secuencia objetivo
        GenerateTargetSequence();

        // Finalmente actualiza los TextMeshPro
        for (int i = 0; i < 80; i++)
        {
            gridTexts[i].text = grid[i];
        }
    }

    string GenerateRandomCell()
    {
        switch (currentLevelType)
        {
            case LevelType.Mixed:
                char letter = (char)('A' + Random.Range(0, 26));
                int number = Random.Range(0, 10);
                return $"{letter}{number}";

            case LevelType.NumbersOnly:
                return Random.Range(0, 100).ToString("00");

            case LevelType.LettersOnly:
                char letter1 = (char)('A' + Random.Range(0, 26));
                char letter2 = (char)('A' + Random.Range(0, 26));
                return $"{letter1}{letter2}";

            case LevelType.Braille:
                // Genera letras/números normales, la fuente Braille los convertirá
                return (Random.Range(0, 2) == 0)
                    ? ((char)('A' + Random.Range(0, 26))).ToString()
                    : Random.Range(0, 10).ToString();

            default:
                return "??";
        }
    }

    void GenerateTargetSequence()
    {
        targetSequence.Clear();

        // Primero genera la secuencia objetivo
        for (int i = 0; i < 4; i++)
        {
            targetSequence.Add(GenerateRandomCell());
            guideTexts[i].text = targetSequence[i];
        }

        // Asegurar que la secuencia objetivo aparezca al menos una vez en la cuadrícula
        int randomPosition = Random.Range(0, 79); // Posición inicial (0-79 para 4 celdas)
        for (int i = 0; i < 4; i++)
        {
            grid[randomPosition + i] = targetSequence[i];
        }
    }

    void UpdateSelection(int indexChange)
    {
        // Calcula la nueva posición con wrap-around (0-79)
        currentStartIndex = (currentStartIndex + indexChange + 80) % 80;

        // Asegura que la selección de 4 elementos no se salga del rango
        if (currentStartIndex > 79) currentStartIndex = 0;  // Si pasa de 79, vuelve al inicio
        if (currentStartIndex < 0) currentStartIndex = 79; // Si es menor a 0, va al final

        // Restablece todos los textos a color normal
        foreach (var text in gridTexts)
        {
            text.color = normalColor;
        }

        // Resalta la nueva selección (4 celdas)
        currentSelection.Clear();
        for (int i = 0; i < 4; i++)
        {
            int index = (currentStartIndex + i) % 80;
            currentSelection.Add(index);
            gridTexts[index].color = selectedColor;
        }
    }

    void StartGame()
    {
        // Selecciona un tipo de nivel aleatorio (0-3)
        currentLevelType = (LevelType)Random.Range(0, 4);

        // Cambia la fuente según el nivel
        TMP_FontAsset fontToUse = (currentLevelType == LevelType.Braille) ? brailleFont : normalFont;
        foreach (var text in gridTexts) text.font = fontToUse;
        foreach (var text in guideTexts) text.font = fontToUse;

        // Inicia el juego
        gameTime = 30f;
        gameActive = true;

        // Posición inicial fija en 44 (mostrará celdas 44, 45, 46, 47)
        currentStartIndex = 43;

        // Actualiza la selección inmediatamente (sin desplazamiento)
        UpdateSelection(0);

        rotationCoroutine = StartCoroutine(RotateGridCoroutine());
    }

    IEnumerator RotateGridCoroutine()
    {
        while (gameActive)
        {
            yield return new WaitForSeconds(2f); // Espera 2 segundos antes de rotar
            RotateGrid();
            UpdateGridDisplay();
        }
    }

    void RotateGrid()
    {
        string first = grid[0];
        for (int i = 0; i < 79; i++) grid[i] = grid[i + 1]; // Desplaza a la izquierda
        grid[79] = first; // El primero va al final

        // Actualiza todos los TextMeshPro
        for (int i = 0; i < 80; i++) gridTexts[i].text = grid[i];
    }

    void UpdateGridDisplay()
    {
        // Elimina las referencias a 'row' y 'col'. Ahora es lineal:
        for (int i = 0; i < 80; i++)
        {
            gridTexts[i].text = grid[i];
        }

        // Actualizar selección (llamada correcta con 1 argumento)
        UpdateSelection(0); // Cambiado de UpdateSelection(0, 0)
    }

    IEnumerator ShowCorrectSequence()
    {
        yield return new WaitForSeconds(1f); // Espera 1 segundo antes de mostrar

        // Buscar secuencias correctas
        for (int i = 0; i <= 79; i++)
        {
            bool match = true;
            for (int j = 0; j < 4; j++)
            {
                if (grid[i + j] != targetSequence[j])
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                for (int j = 0; j < 4; j++)
                {
                    gridTexts[i + j].color = correctColor;
                }
            }
        }

        gameActive = false;
        StopCoroutine(rotationCoroutine);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            UpdateSelection(1); // Derecha
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            UpdateSelection(-1); // Izquierda
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            UpdateSelection(-10); // Arriba (ahora con wrap-around)
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            UpdateSelection(10); // Abajo (ahora con wrap-around)
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            CheckSequence();
        }
    }

    void CheckSequence()
    {
        bool isCorrect = true;
        for (int i = 0; i < 4; i++)
        {
            if (gridTexts[currentSelection[i]].text != targetSequence[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            StartCoroutine(EndGameAndRestart());
        }
        else
        {
            // Mostrar secuencia correcta antes de reiniciar
            StartCoroutine(ShowCorrectSequenceAndRestart());
        }
    }

    void ResetAndStartGame()
    {
        InitializeGrid();
        GenerateTargetSequence();
        UpdateSelection(0);
        StartGame();
    }

    IEnumerator ShowCorrectSequenceAndRestart()
    {
        gameActive = false;
        StopCoroutine(rotationCoroutine);

        // Restar puntos (derrota)
        currentScore += losePoints;
        PlayerStatsManager.Instance.AddConocimiento(-1);
        PlayerStatsManager.Instance.AddEstres(+2);
        if (currentScore < 0) currentScore = 0;
        UpdateScoreText();

        // Mantener la selección actual en rojo
        foreach (int index in currentSelection)
        {
            gridTexts[index].color = selectedColor; // Mantiene el color rojo
        }

        // Mostrar la secuencia correcta en verde
        HighlightCorrectSequence();

        yield return new WaitForSeconds(2f);
        ResetAndStartGame();
    }

    IEnumerator EndGameAndRestart()
    {
        gameActive = false;
        StopCoroutine(rotationCoroutine);

        // Sumar puntos (victoria)
        currentScore += winPoints;
        PlayerStatsManager.Instance.AddConocimiento(+2);
        PlayerStatsManager.Instance.AddEstres(+1);
        UpdateScoreText();

        // Mantener la selección actual en rojo (solo si es por tiempo)
        foreach (int index in currentSelection)
        {
            gridTexts[index].color = selectedColor; // Mantiene el color rojo
        }

        // Mostrar la secuencia correcta en verde
        HighlightCorrectSequence();

        yield return new WaitForSeconds(2f);
        ResetAndStartGame();
    }

void HighlightCorrectSequence()
{
    // 1. Buscar la posición de la secuencia correcta en el grid
    for (int i = 0; i <= 76; i++) // 76 porque son 80 celdas - 4 de la secuencia
    {
        bool esSecuenciaCorrecta = true;
        for (int j = 0; j < 4; j++)
        {
            if (grid[i + j] != targetSequence[j])
            {
                esSecuenciaCorrecta = false;
                break;
            }
        }

        // 2. Si encontramos la secuencia correcta
        if (esSecuenciaCorrecta)
        {
            // Poner en verde solo la secuencia correcta
            for (int j = 0; j < 4; j++)
            {
                gridTexts[i + j].color = correctColor;
            }
            break;
        }
    }
}

    void UpdateTimer()
    {
        gameTime -= Time.deltaTime;

        if (gameTime <= 0)
        {
            gameTime = 0;
            StartCoroutine(EndGameAndRestart()); // Cambiado de StartCoroutine(ShowCorrectSequenceAndRestart())
        }

        timerText.text = $"{Mathf.FloorToInt(gameTime):00}:{Mathf.FloorToInt((gameTime % 1) * 1000):000}";
    }

    void HideGrid()
    {
        // Oculta todos los elementos de la cuadrícula
        foreach (var text in gridTexts)
        {
            text.gameObject.SetActive(false);
        }

        // Oculta los textos guía
        foreach (var guide in guideTexts)
        {
            guide.gameObject.SetActive(false);
        }

        // Oculta el temporizador
        timerText.gameObject.SetActive(false);
    }

    void ShowGrid()
    {
        // Muestra todos los elementos de la cuadrícula
        foreach (var text in gridTexts)
        {
            text.gameObject.SetActive(true);
        }

        // Muestra los textos guía
        foreach (var guide in guideTexts)
        {
            guide.gameObject.SetActive(true);
        }

        // Muestra el temporizador
        timerText.gameObject.SetActive(true);
    }

    public void OnExitButtonPressed()
    {
        // Detener todas las corrutinas
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        // Restablecer el estado del juego
        gameActive = false;

        // Ocultar todos los elementos del juego
        HideGrid();

        // Mostrar el botón de inicio nuevamente
        startButton.SetActive(true);

        // Restablecer el temporizador
        gameTime = 30f;

        // Opcional: Resetear la puntuación si lo deseas
        currentScore = 0;
        UpdateScoreText();

        if (playerController != null) playerController.enabled = true;

        Debug.Log("Juego terminado por el jugador");
        // Desactivar el canvas de hacking
        if (canvasHacking != null)
        {
            canvasHacking.SetActive(false);
            Debug.Log("Canvas de hacking desactivado");
        }
    }
}