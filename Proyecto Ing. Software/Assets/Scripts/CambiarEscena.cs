using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    public GameObject canvasInicio; // Canvas que se mostrará al iniciar
    public GameObject[] canvasOcultos; // Canvases que se ocultarán al iniciar

    private bool jugadorDentro = false;
    public string escenaDestino; // Nombre de la escena a cargar
    private HUDController hudController; // Referencia al HUDController

    void Start()
    {
        // Obtener referencia al HUDController al inicio
        hudController = FindObjectOfType<HUDController>();
        MostrarCanvasInicio();
    }

    public void CargarNuevaEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // Función para mostrar el Canvas al iniciar y ocultar otros
    public void MostrarCanvasInicio()
    {
        if (canvasInicio != null)
        {
            canvasInicio.SetActive(true);
        }

        foreach (GameObject canvas in canvasOcultos)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }
    }

    // Función para cambiar de escena al entrar en un trigger y presionar "E"
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            // Ocultar mensaje cuando el jugador sale del trigger
            if (hudController != null)
            {
                hudController.MostrarMensaje("", 0.1f); // Mensaje vacío para limpiar
            }
        }
    }

    private void Update()
    {
        if (jugadorDentro)
        {
            // Mostrar mensaje solo si tenemos referencia al HUDController
            if (hudController != null)
            {
                hudController.MostrarMensaje($"Presiona E para ir a {escenaDestino}");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene(escenaDestino);
            }
        }
    }
}