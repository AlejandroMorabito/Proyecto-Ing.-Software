using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MemoryController : MonoBehaviour
{
    public Image[] uiImages = new Image[36];
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    private readonly Color color0 = new Color(0f, 42f/255f, 152f/255f);    // #002A98
    private readonly Color color1 = new Color(47f/255f, 104f/255f, 255f/255f); // #2F68FF
    private readonly Color wrongColor = new Color(1f, 0.2157f, 0.1843f);   // #FF372F (rojo error)

    private List<int> imageValues = new List<int>();
    private List<int> correctIndices = new List<int>();
    private int lives = 3;
    private bool gameActive = false;

    void Start()
    {
        InitializeColors();
        StartCoroutine(StartGameRoutine());
    }

    private void InitializeColors()
    {
        imageValues.Clear();
        correctIndices.Clear();
        
        for (int i = 0; i < uiImages.Length; i++)
        {
            if (uiImages[i] != null)
            {
                uiImages[i].color = color0;
                imageValues.Add(0);
            }
        }
    }

    private IEnumerator StartGameRoutine()
    {
        // Fase de memorización (6 cuadrados en color1)
        SetRandomHighlightedImages(6);
        countdownText.text = "Memoriza...";
        yield return new WaitForSeconds(5f);

        // Vuelve todo a color0
        foreach (int index in correctIndices)
        {
            uiImages[index].color = color0;
            imageValues[index] = 0;
        }

        countdownText.text = "";
        livesText.text = "Vidas: " + lives;
        gameActive = true;
    }

    private void SetRandomHighlightedImages(int count)
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < uiImages.Length; i++)
        {
            if (uiImages[i] != null) availableIndices.Add(i);
        }

        // Mezcla aleatoria
        for (int i = 0; i < count; i++)
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

    // Llamado desde el EventTrigger de cada Image (configura en el Inspector)
    public void OnSquareClicked(int index)
    {
        if (!gameActive || imageValues[index] == 2) return;

        if (correctIndices.Contains(index))
        {
            // Acierto
            uiImages[index].color = color1;
            imageValues[index] = 2; // Marcado como acertado
            CheckWinCondition();
        }
        else
        {
            // Error
            uiImages[index].color = wrongColor;
            lives--;
            livesText.text = "Vidas: " + lives;
            if (lives <= 0) GameOver();
        }
    }

    private void CheckWinCondition()
    {
        foreach (int index in correctIndices)
        {
            if (imageValues[index] != 2) return;
        }
        WinGame();
    }

    private void GameOver()
    {
        gameActive = false;
        gameOverPanel.SetActive(true);
    }

    private void WinGame()
    {
        gameActive = false;
        winPanel.SetActive(true);
    }

    // Reiniciar el juego (llamar desde botón UI)
    public void RestartGame()
    {
        InitializeColors();
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        lives = 3;
        StartCoroutine(StartGameRoutine());
    }
}