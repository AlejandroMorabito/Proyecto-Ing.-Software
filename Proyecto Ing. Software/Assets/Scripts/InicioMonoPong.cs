using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InicioMonoPong : MonoBehaviour
{
    public GameObject introCanvas; // Canvas introductorio
    public Button playButton;
    public Button exitButton;
    public string escenaDestino; // Nombre de la escena a cargar

    // Referencia al ScoreScript
    public ScoreScript scoreScript;

    void Start()
    {
        // Asegurarse que el canvas introductorio está desactivado al inicio
        if (introCanvas != null)
        {
            introCanvas.SetActive(false);
        }

        // Asignar el listener al botón
        if (playButton != null)
        {
            playButton.onClick.AddListener(CambiarEscena);
            Debug.LogError("presionado Play");
        }
        else
        {
            Debug.LogError("No se ha asignado el botón Play en el inspector");
        }

        // Asignar el listener al botón de salir
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(CambiarEscenaSalida);
        }
        else
        {
            Debug.LogError("No se ha asignado el botón Exit en el inspector");
        }
    }

    void CambiarEscena()
    {
        if (!string.IsNullOrEmpty(escenaDestino))
        {
            Debug.Log("Botón Play presionado. Cambiando a escena: " + escenaDestino);
            SceneManager.LoadScene(escenaDestino);
        }
        else
        {
            Debug.LogError("No se ha especificado el nombre de la escena destino");
        }
    }
    public void CambiarEscenaSalida()
    {
        // Obtener el score antes de salir
        if (scoreScript != null)
        {
            int scoreActual = scoreScript.GetScore();
            PlayerStatsManager.Instance.AddEstres(-(scoreActual / 10)); // Ajustar el estrés basado en el score
            PlayerStatsManager.Instance.AddConocimiento(-(scoreActual / 50)); // Ajustar el conocimiento basado en el score
            Debug.Log("Score actual antes de salir: " + scoreActual);
            SceneManager.LoadScene(escenaDestino);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el ScoreScript en el inspector");
        }
    }
}
