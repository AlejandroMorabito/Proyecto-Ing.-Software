using UnityEngine;
using System.Collections.Generic;

public class ActivarCanvasCL : MonoBehaviour
{
    public GameObject canvasGame;
    public GameObject canvasNotas;
    public List<int> SemanasExamenes = new List<int>();

    [Header("Canvas HUD")]
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

    [Header("Días Permitidos")]
    [Tooltip("Días de la semana permitidos (ejemplo: Lunes, Martes, etc.)")]
    public List<string> diasPermitidos = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };

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
                if (EsDiaYHoraPermitida())
                {
                    hudController.MostrarMensaje("Presiona E para estudiar");
                }
                else
                {
                    hudController.MostrarMensaje("No es el día u hora adecuada para estudiar");
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
            
            if (canvasGame != null && canvasGame.activeSelf)
            {
                canvasGame.SetActive(false);
                if (HUDCanvas != null) GetComponent<Canvas>().enabled = true;
                if (playerController != null) playerController.enabled = true;
            }
            return;
        }

        // Interacción con E
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            if (!EsDiaYHoraPermitida())
            {
                string horarioPermitido = $"{horaInicio:00}:{minutoInicio:00} - {horaFin:00}:{minutoFin:00}";
                string dias = string.Join(", ", diasPermitidos);
                hudController?.MostrarMensaje($"Días permitidos: {dias}\nHorario permitido: {horarioPermitido}", 3f);
                return;
            }

            if (canvasGame == null && canvasNotas == null) return;

            if (SemanasExamenes.Contains(PlayerStatsManager.Instance.Semana))
            {
                // Si la semana actual está en la lista de semanas de exámenes, activar el canvas de notas
                canvasNotas.SetActive(true);
                if (playerController != null) playerController.enabled = false;
            }
            else
            {
                // Si no es semana de examen, activar el canvas de estudio
                bool canvasActivo = !canvasGame.activeSelf;
                canvasGame.SetActive(canvasActivo);
                if (playerController != null) playerController.enabled = !canvasActivo;
            }

            if (HUDCanvas != null)
            {
                GetComponent<Canvas>().enabled = false;  
            }
        }
    }
    
    private bool EsDiaYHoraPermitida()
    {
        if (PlayerStatsManager.Instance == null) return false;

        // Verifica el día
        string diaActual = PlayerStatsManager.Instance.ObtenerDiaSemana();
        if (!diasPermitidos.Contains(diaActual)) return false;

        // Verifica la hora
        var (horaActual, minutoActual) = PlayerStatsManager.Instance.GetHoraYMinutosActual();
        int totalActual = horaActual * 60 + minutoActual;
        int totalInicio = horaInicio * 60 + minutoInicio;
        int totalFin = horaFin * 60 + minutoFin;

        return totalActual >= totalInicio && totalActual < totalFin;
    }
}