using UnityEngine;

public class ActivarCanvas : MonoBehaviour
{
    public GameObject canvasUI;
    public GameObject HUDCanvas;
    private bool jugadorDentro = false;
    public PlayerController playerController;
    
    // Referencia al HUDController para mostrar mensajes
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
                hudController.MostrarMensaje($"Presiona E para estudiar");
            }
        }
        // Verificación segura de PlayerStatsManager
        if (PlayerStatsManager.Instance == null) return;

        if (PlayerStatsManager.Instance.Estres >= 100)
        {
            // Mostrar mensaje en el HUD en lugar de Debug.Log
            if (hudController != null)
            {
                hudController.MostrarMensaje("¡Nivel de estrés máximo alcanzado!");
            }
            
            // Cerrar el canvas si está abierto
            if (canvasUI != null && canvasUI.activeSelf)
            {
                canvasUI.SetActive(false);
                if (HUDCanvas != null)GetComponent<Canvas>().enabled = true;
                if (playerController != null) playerController.enabled = true;
            }
        }
        else if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            if (canvasUI == null) return;

            bool canvasActivo = !canvasUI.activeSelf;
            canvasUI.SetActive(canvasActivo);
            
            if (HUDCanvas != null)
            {
                GetComponent<Canvas>().enabled = false;  
            }

            if (playerController != null)
            {
                playerController.enabled = !canvasActivo;
            }
        }
    }
}