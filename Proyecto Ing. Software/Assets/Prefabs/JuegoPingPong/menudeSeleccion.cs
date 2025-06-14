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
    public Game_Manager Game_Manager;

    void Start()
    {
        introCanvas.SetActive(true);
        juegoCanvas.SetActive(false);
        j1Button.onClick.AddListener(Onplay1ButtonPressed);
        j2Button.onClick.AddListener(Onplay2ButtonPressed);
    }

    public void Onplay1ButtonPressed()
    {
        Game_Manager.setAI(true);
        introCanvas.SetActive(false);
        juegoCanvas.SetActive(true);
        Debug.Log("AI mode enabled");
    }

    public void Onplay2ButtonPressed()
    {
        Game_Manager.setAI(false);
        introCanvas.SetActive(false);
        juegoCanvas.SetActive(true);
        Debug.Log("AI mode disabled");
    }
}
