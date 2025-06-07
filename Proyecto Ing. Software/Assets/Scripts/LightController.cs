using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LightController : MonoBehaviour
{
    [Header("UI Images Settings")]
    [Tooltip("Asigna las 16 imágenes UI (4x4 grid)")]
    public Image[] uiImages = new Image[16];
    
    private readonly Color color0 = new Color(0.0627f, 0.4745f, 0f); // #107900
    private readonly Color color1 = new Color(0.1255f, 1f, 0f);      // #20FF00
    private List<int> imageValues = new List<int>();
    private const int gridSize = 4;
    private bool allSameColor = false;

    void Start()
    {
        InitializeImages();
    }

    void InitializeImages()
    {
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
                ToggleImageAndCross(index);
                CheckSolution();
            });

            imageValues.Add(0); // Inicializar a 0 temporalmente
            UpdateImageColor(i);
        }

        GenerateSolvablePuzzle(); // Generar puzzle solucionable al inicio
    }

    void ToggleImageAndCross(int index)
    {
        if (allSameColor) return;

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
            Debug.Log("¡Felicidades! Todas las imágenes son verdes brillantes (#20FF00)");
            ShowWinEffect();
        }
    }

    void ShowWinEffect()
    {
        StartCoroutine(WinAnimation());
    }

    System.Collections.IEnumerator WinAnimation()
    {
        for (int i = 0; i < 5; i++)
        {
            foreach (var img in uiImages)
            {
                img.color = Color.white;
            }
            yield return new WaitForSeconds(0.2f);
            
            foreach (var img in uiImages)
            {
                img.color = color1;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void GenerateSolvablePuzzle()
    {
        // Primero pon todas a 1 (estado ganador)
        for (int i = 0; i < imageValues.Count; i++)
        {
            imageValues[i] = 1;
            UpdateImageColor(i);
        }

        // Generar una solución aleatoria (entre 3 y 8 movimientos)
        int moves = Random.Range(7, 12);
        List<int> solutionMoves = new List<int>();

        for (int i = 0; i < moves; i++)
        {
            int randomIndex = Random.Range(0, 16);
            solutionMoves.Add(randomIndex);
        }

        // Aplicar la solución inversa para crear el puzzle
        foreach (int move in solutionMoves)
        {
            ToggleImageAndCross(move);
        }

        // Guardar la solución para referencia (opcional)
        PlayerPrefs.SetString("CurrentSolution", string.Join(",", solutionMoves));
    }

    public void ShowSolution()
    {
        string solution = PlayerPrefs.GetString("CurrentSolution", "");
        if (!string.IsNullOrEmpty(solution))
        {
            Debug.Log("Solución: " + solution);
        }
        else
        {
            Debug.Log("No hay solución guardada");
        }
    }

    public void ForceWinState()
    {
        for (int i = 0; i < imageValues.Count; i++)
        {
            imageValues[i] = 1;
            UpdateImageColor(i);
        }
        allSameColor = true;
        ShowWinEffect();
    }
}