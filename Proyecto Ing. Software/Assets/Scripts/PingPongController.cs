using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PingPongController : MonoBehaviour
{
    public float speed = 400f;
    public RectTransform canvasRect; // Asigna el RectTransform del Canvas en el inspector
    public TextMeshProUGUI scoreText; // Asigna el Text del puntaje en el inspector
    private Vector2 direction;
    private int score1 = 0;
    private RectTransform ballRect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ballRect = GetComponent<RectTransform>();
        ResetBall();
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        // Mueve la pelota
        ballRect.anchoredPosition += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("wall"))
        {
            // Rebote vertical
            direction.y = -direction.y;
        }
        else if (other.CompareTag("wall2"))
        {
            // Suma punto y reinicia pelota
            score1++;
            UpdateScore();
            ResetBall();
        }
    }

    void ResetBall()
    {
        // Centra la pelota
        ballRect.anchoredPosition = Vector2.zero;
        // Dirección aleatoria
        float angle = Random.Range(0, 2 * Mathf.PI);
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + score1;
    }
}
