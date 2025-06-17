using UnityEngine;
using System.Collections.Generic;

public class ActivarCanvasCL : MonoBehaviour
{
    public GameObject canvasGame;
    public GameObject canvasNotas;
    public List<int> SemanasExamenes = new List<int>();
    public List<int> DiasExamenes = new List<int>();
    public List<(int semana, int dia)> fechas = new List<(int, int)>();

    [Header("Canvas HUD")]
    public GameObject HUDCanvas;
    private bool jugadorDentro = false;
    public PlayerController playerController;
    public CardsController cardsController;
    
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
    [Tooltip("Días de la semana permitidos")]
    public List<string> diasPermitidos = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };

    void Awake()
    {
        int count = Mathf.Min(SemanasExamenes.Count, DiasExamenes.Count);
        for (int i = 0; i < count; i++)
        {
            fechas.Add((SemanasExamenes[i], DiasExamenes[i]));
        }
    }

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

        int semanaActual = PlayerStatsManager.Instance.Semana;
        int diaActual = PlayerStatsManager.Instance.nDia;

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
                    
                // Llama al método de CardsController para cerrar el minijuego correctamente
                if (cardsController != null)
                {
                    cardsController.OnExitButtonPressed();
                }
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

            if (fechas.Contains((semanaActual, diaActual)))
            {
                // Si es semana de examen, activar el canvas de notas
                if (hudController != null)
                {
                    hudController.MostrarMensaje("¡Semana de exámenes! Accediendo a las notas...", 2f);
                }
                // Si la semana actual está en la lista de semanas de exámenes, activar el canvas de notas
                canvasNotas.SetActive(true);
                if (playerController != null) playerController.enabled = false;
            }
            else
            {
                // Si no es semana de examen, activar el canvas de estudio
                if (hudController != null)
                {
                    hudController.MostrarMensaje("Accediendo al estudio...", 2f);
                }
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