using UnityEngine;

public class ActivarCanvasCL : MonoBehaviour
{
    public GameObject canvasUI;
    public GameObject HUDCanvas;
    private bool jugadorDentro = false;
    public PlayerController playerController;
    
    // Referencia al HUDController para mostrar mensajes
    private HUDController hudController;

    [Header("Horario Permitido")]
    [Tooltip("Hora de inicio en formato 24h")]
    [Range(0, 23)] public int horaInicio = 8; // 8:00 AM
    [Range(0, 59)] public int minutoInicio = 0;
    [Tooltip("Hora final en formato 24h")]
    [Range(0, 23)] public int horaFin = 18;   // 6:00 PM
    [Range(0, 59)] public int minutoFin = 0;

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
            // Limpiar mensaje al salir
            if (hudController != null)
            {
                hudController.MostrarMensaje("", 0.1f);
            }
        }
    }

    private void Update()
    {
        if (PlayerStatsManager.Instance == null) return;

        // Mostrar mensaje general cuando el jugador está dentro
        if (jugadorDentro)
        {
            if (hudController != null)
            {
                if (EsHoraPermitida())
                {
                    hudController.MostrarMensaje("Presiona E para estudiar");
                }
                else
                {
                    hudController.MostrarMensaje("No es la hora adecuada para estudiar");
                }
            }
        }

        // Manejo de estrés máximo
        if (PlayerStatsManager.Instance.Estres >= 100)
        {
            if (hudController != null)
            {
                hudController.MostrarMensaje("¡Nivel de estrés máximo alcanzado!");
            }
            
            if (canvasUI != null && canvasUI.activeSelf)
            {
                canvasUI.SetActive(false);
                if (HUDCanvas != null)GetComponent<Canvas>().enabled = true;
                if (playerController != null) playerController.enabled = true;
            }
            return;
        }

        // Interacción con E
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            if (!EsHoraPermitida())
            {
                string horarioPermitido = $"{horaInicio:00}:{minutoInicio:00} - {horaFin:00}:{minutoFin:00}";
                hudController?.MostrarMensaje($"Horario permitido: {horarioPermitido}", 3f);
                return;
            }

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

    private bool EsHoraPermitida()
    {
        if (PlayerStatsManager.Instance == null) return false;
        
        var (horaActual, minutoActual) = PlayerStatsManager.Instance.GetHoraYMinutosActual();
        
        // Convertir a minutos totales para comparación precisa
        int totalActual = horaActual * 60 + minutoActual;
        int totalInicio = horaInicio * 60 + minutoInicio;
        int totalFin = horaFin * 60 + minutoFin;

        return totalActual >= totalInicio && totalActual < totalFin;
    }
}