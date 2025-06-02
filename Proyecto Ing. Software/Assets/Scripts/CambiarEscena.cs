using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    public GameObject canvasInicio; // Canvas que se mostrará al iniciar
    public GameObject[] canvasOcultos; // Canvases que se ocultarán al iniciar

    private bool jugadorDentro = false;
    public string escenaDestino; // Nombre de la escena a cargar

    void Start()
    {
        MostrarCanvasInicio();
    }

    public void CargarNuevaEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // ✅ Función para mostrar el Canvas al iniciar y ocultar otros
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

    // ✅ Función para cambiar de escena al entrar en un trigger y presionar "E"
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
        }
    }

    private void Update()
{
    if (jugadorDentro)
    {
        Debug.Log("Jugador dentro del trigger.");

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Tecla 'E' presionada. Cambiando de escena...");
            SceneManager.LoadScene(escenaDestino);
        }
    }
}

}