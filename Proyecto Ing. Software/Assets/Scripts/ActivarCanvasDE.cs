using UnityEngine;

public class ActivarCanvasDE : MonoBehaviour
{
    public GameObject canvasUI;
    public GameObject HUDCanvas;
    private bool jugadorDentro = false;
    public PlayerController playerController; // Referencia al script de movimiento del jugador
    private HUDController hudController;

    private void Start()
    {
        // Buscar el HUDController al inicio
        hudController = FindObjectOfType<HUDController>();
    }

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
            // Mostrar mensaje solo si tenemos referencia al HUDController
            if (hudController != null)
            {
                hudController.MostrarMensaje($"Presiona E para juegar");
            }
        }

        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            bool canvasActivo = !canvasUI.activeSelf;
            canvasUI.SetActive(canvasActivo);
            GetComponent<Canvas>().enabled = false;  

            if (playerController != null)
            {
                playerController.enabled = !canvasActivo; // Desactiva el control del jugador
            }
        }
    }
}