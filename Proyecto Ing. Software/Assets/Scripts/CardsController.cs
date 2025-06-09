using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using TMPro;

public class CardsController : MonoBehaviour
{
    [Header("Game Components")]
    [SerializeField] Card cardPrefab;
    [SerializeField] Transform gridTransform;
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject introCanvas;
    [SerializeField] GameObject HUDCanvas;

    [Header("HUD Controller")]
    [SerializeField] private HUDController hudController;

    [Header("UI Elements")]
    [SerializeField] Button startButton;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;
    public Button exitButton;

    [Header("Time Settings")]
    [Range(0, 23)] public int endHour = 18; // 6:00 PM
    [Range(0, 59)] public int endMinute = 0;

    private List<Sprite> spritePairs;
    private Card firstSelected;
    private Card secondSelected;
    private int matchCounts;
    private bool gameStarted = false;
    private int totalScore = 0;
    private float timeCheckInterval = 1f;
    private float nextTimeCheck;
    public PlayerController playerController;

    private void Awake()
    {
        hudController = FindObjectOfType<HUDController>();

        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(OnExitButtonPressed);
        
        gameCanvas.SetActive(false);
        introCanvas.SetActive(false);
        UpdateScoreUI();
    }

    private void Update()
    {
        if (Time.time > nextTimeCheck && gameStarted)
        {
            nextTimeCheck = Time.time + timeCheckInterval;
            CheckGameTime();
        }
    }

    void CheckGameTime()
    {
        if (PlayerStatsManager.Instance == null) return;
        
        var (currentHour, currentMinute) = PlayerStatsManager.Instance.GetHoraYMinutosActual();
        timeText.text = $"{currentHour:00}:{currentMinute:00}";
        
        if (currentHour >= endHour && currentMinute >= endMinute)
        {
            EndGameByTime();
        }
    }

    void EndGameByTime()
    {
        if (gameStarted)
        {
            ResetGame();
            // Desactivar canvas del juego
            gameCanvas.SetActive(false);

            // Reactivar controles del jugador
            if (playerController != null)
            {
                playerController.enabled = true;
               GetComponent<Canvas>().enabled = true;  
            }
            if (hudController != null)
            {
                hudController.MostrarMensaje("¡Se acabó el tiempo! Hora de descansar", 3f);
            }
        }
    }

    public void OnExitButtonPressed()
    {
        ResetGame();
        gameCanvas.SetActive(false);
        
        if (playerController != null)
        {
            playerController.enabled = true;
           GetComponent<Canvas>().enabled = true;
        }
    }

    public void StartGame()
    {
        if (!gameStarted && PlayerStatsManager.Instance != null)
        {
            gameStarted = true;
            gameCanvas.SetActive(true);
            introCanvas.SetActive(false);
            
            var (currentHour, currentMinute) = PlayerStatsManager.Instance.GetHoraYMinutosActual();
            if (currentHour >= endHour && currentMinute >= endMinute)
            {
                EndGameByTime();
                return;
            }

            PrepareSprites();
            CreateCards();
            startButton.gameObject.SetActive(false);
        }
    }

    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        foreach (var sprite in sprites)
        {
            spritePairs.Add(sprite);
            spritePairs.Add(sprite);
        }
        ShuffleSprites(spritePairs);
    }

    void CreateCards()
    {
        ClearGrid();
        
        for(int i = 0; i < spritePairs.Count; i++)
        {
            Card card = Instantiate(cardPrefab, gridTransform);
            card.SetIconSprite(spritePairs[i]);
            card.controller = this;
        }
    }

    public void SetSelected(Card card)
    {
        if(!gameStarted || card.isSelected) return;

        card.Show();

        if (firstSelected == null)
        {
            firstSelected = card;
            return;
        }

        secondSelected = card;
        StartCoroutine(CheckMatching(firstSelected, secondSelected));
        firstSelected = null;
        secondSelected = null;
    }

    IEnumerator CheckMatching(Card a, Card b)
    {
        yield return new WaitForSeconds(0.3f);
        
        if(a.iconSprite == b.iconSprite)
        {
            matchCounts++;
            
            if (matchCounts >= spritePairs.Count / 2) 
            {
                WinGame();
            }
        }
        else
        {
            a.Hide();
            b.Hide();
        }
    }

    void WinGame()
    {
        // Sumar puntos
        totalScore++;
        UpdateScoreUI();
        
        Debug.Log("Intentando actualizar estadísticas del jugador...");
        
        if (PlayerStatsManager.Instance != null)
        {
            Debug.Log("PlayerStatsManager.Instance encontrado");
            Debug.Log($"Valores actuales - Conocimiento: {PlayerStatsManager.Instance.Conocimiento}, Estrés: {PlayerStatsManager.Instance.Estres}");
            
            PlayerStatsManager.Instance.AddConocimiento(1);
            PlayerStatsManager.Instance.AddEstres(2);
            
            Debug.Log($"Valores después - Conocimiento: {PlayerStatsManager.Instance.Conocimiento}, Estrés: {PlayerStatsManager.Instance.Estres}");
        }
        else
        {
            Debug.LogError("PlayerStatsManager.Instance es null!");
        }
        
        // Animación de victoria
        PrimeTween.Sequence.Create()
            .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one * 1.2f, 0.2f, ease: PrimeTween.Ease.OutBack))
            .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one, 0.1f))
            .ChainDelay(1f)
            .ChainCallback(() => StartNewGame());
    }

    void StartNewGame()
    {
        matchCounts = 0;
        ClearGrid();
        PrepareSprites();
        CreateCards();
    }

    void ClearGrid()
    {
        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntos: {totalScore}";
        }
    }

    void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (spriteList[i], spriteList[randomIndex]) = (spriteList[randomIndex], spriteList[i]);
        }
    }

    public void ResetGame()
    {
        gameStarted = false;
        matchCounts = 0;
        firstSelected = null;
        secondSelected = null;
        ClearGrid();
        startButton.gameObject.SetActive(true);
    }
}