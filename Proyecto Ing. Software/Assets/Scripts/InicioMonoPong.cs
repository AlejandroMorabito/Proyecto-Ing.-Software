using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InicioMonoPong : MonoBehaviour
{
    public GameObject introCanvas; // Canvas introductorio
    public Button playButton;
    public string escenaDestino; // Nombre de la escena a cargar

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
        }
        else
        {
            Debug.LogError("No se ha asignado el botón Play en el inspector");
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
}
