using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameModeSelector : MonoBehaviour
{
    public GameObject introCanvas;
    public GameObject juegoCanvas;
    public Button j1Button;
    public Button j2Button;
    public Paddle Paddle;

    void Start()
    {
        introCanvas.SetActive(true);
        juegoCanvas.SetActive(false);
        j1Button.onClick.AddListener(Onplay1ButtonPressed);
        j2Button.onClick.AddListener(Onplay2ButtonPressed);
        Debug.Log("hola");
    }

    public void Onplay1ButtonPressed()
    {
        Paddle.setAI(true);
        Debug.Log("hola");
        Debug.LogWarning("aaaaa");
        introCanvas.SetActive(false);
        juegoCanvas.SetActive(true);
    }

    public void Onplay2ButtonPressed()
    {
        introCanvas.SetActive(false);
        juegoCanvas.SetActive(true);
        Paddle.setAI(true);
        Debug.Log("hola");
    }
}
