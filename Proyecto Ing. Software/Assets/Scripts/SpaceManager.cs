using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // <-- Agrega esto

public class SpaceManager : MonoBehaviour
{
    public GameObject player;
    public GameObject invadersGroup;
    public TMP_Text gameoverText; // Asigna el TMP_Text en el inspector
    public GameObject startButton; // Asigna aquí tu botón de inicio
    public TMP_Text winText; // Asigna el TMP_Text en el inspector
    public List<GameObject> Bunckers;

    private void Start()
    {
        gameoverText.gameObject.SetActive(false);
        foreach (GameObject obj in Bunckers) // <-- aquí debe ser Bunckers
        {
            if (obj != null)
                obj.SetActive(false);
        }
        if (winText != null) winText.gameObject.SetActive(false);

        Time.timeScale = 0f;
        if (player != null) player.SetActive(false);
        if (invadersGroup != null) invadersGroup.SetActive(false);
        if (startButton != null) startButton.SetActive(true);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        if (player != null) player.SetActive(true);
        foreach (GameObject obj in Bunckers) // <-- aquí debe ser Bunckers
        {
            if (obj != null)
                obj.SetActive(true);
        }
        if (invadersGroup != null) invadersGroup.SetActive(true);
        if (startButton != null) startButton.SetActive(false);
        if (winText != null) winText.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // NUEVO: Método para mostrar victoria
    public void WinGame()
    {
        Time.timeScale = 0f;
        if (winText != null) winText.gameObject.SetActive(true);
        PlayerStatsManager.Instance.AddEstres(-10);
    }
}