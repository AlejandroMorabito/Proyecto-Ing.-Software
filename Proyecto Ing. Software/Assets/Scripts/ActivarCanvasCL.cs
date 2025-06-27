using UnityEngine;
using System.Collections.Generic;

public class ActivarCanvasCL : MonoBehaviour
{
    public List<GameObject> canvasIntro; // Lista de canvases de intro
    public List<GameObject> canvasGames; // Lista de canvases de juego
    public GameObject canvasNotas;
    public List<int> SemanasExamenes = new List<int>();
    public List<int> DiasExamenes = new List<int>();
    public List<(int semana, int dia)> fechas = new List<(int, int)>();

    [Header("Canvas HUD")]
    public GameObject HUDCanvas;
    private bool jugadorDentro = false;
    public PlayerController playerController;
    
    private HUDController hudController;

    [Header("Horario Permitido")]
    [Range(0, 23)] public int horaInicio = 8;
    [Range(0, 59)] public int minutoInicio = 0;
    [Range(0, 23)] public int horaFin = 18;
    [Range(0, 59)] public int minutoFin = 0;

    [Header("Días Permitidos")]
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
                    DesactivarTodosCanvasesJuegos();
                }
            }
        }

        if (PlayerStatsManager.Instance.Estres >= 100)
        {
            if (hudController != null)
            {
                hudController.MostrarMensaje("¡Nivel de estrés máximo alcanzado!");
            }
            
            DesactivarTodosCanvases();
            if (HUDCanvas != null) GetComponent<Canvas>().enabled = true;
            if (playerController != null) playerController.enabled = true;
            return;
        }

        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            if (!EsDiaYHoraPermitida())
            {
                string horarioPermitido = $"{horaInicio:00}:{minutoInicio:00} - {horaFin:00}:{minutoFin:00}";
                string dias = string.Join(", ", diasPermitidos);
                hudController?.MostrarMensaje($"Días permitidos: {dias}\nHorario permitido: {horarioPermitido}", 3f);
                return;
            }

            if (fechas.Contains((semanaActual, diaActual)))
            {
                if (hudController != null)
                {
                    hudController.MostrarMensaje("¡Semana de exámenes! Accediendo a las notas...", 2f);
                }
                canvasNotas.SetActive(true);
                if (playerController != null) playerController.enabled = false;
            }
            else
            {
                if (hudController != null)
                {
                    hudController.MostrarMensaje("Accediendo al estudio...", 2f);
                }
                ActivarCanvasAleatorio();
                if (playerController != null) playerController.enabled = false;
            }

            if (HUDCanvas != null)
            {
                GetComponent<Canvas>().enabled = false;  
            }
        }
    }

    private void ActivarCanvasAleatorio()
    {
        // Desactivar todos los canvases primero
        DesactivarTodosCanvases();
        
        // Activar un canvas aleatorio si hay elementos en la lista
        if (canvasIntro.Count > 0)
        {
            int randomIndex = Random.Range(0, canvasIntro.Count);
            canvasIntro[randomIndex].SetActive(true);
        }
    }

    private void DesactivarTodosCanvases()
    {
        // Desactivar todos los canvases de la lista
        foreach (GameObject canvas in canvasIntro)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }
        
        // Desactivar también el canvas de notas por si acaso
        if (canvasNotas != null)
        {
            canvasNotas.SetActive(false);
        }
    }
    private void DesactivarTodosCanvasesJuegos() 
    {
        foreach (GameObject canvas in canvasGames) 
        {
            if (canvas != null) 
            {
                // Busca todos los scripts adjuntos al canvas
                MonoBehaviour[] scripts = canvas.GetComponents<MonoBehaviour>();
                
                foreach (MonoBehaviour script in scripts) 
                {
                    // Usa reflexión para verificar si el script tiene el método
                    System.Type type = script.GetType();
                    System.Reflection.MethodInfo method = type.GetMethod("OnExitButtonPressed");
                    
                    if (method != null) 
                    {
                        method.Invoke(script, null); // Ejecuta el método
                        break; // Opcional: Si solo un script lo tiene, termina el bucle
                    }
                }
                
                // Si no se encontró el método, desactiva el canvas
                canvas.SetActive(false);
            }
        }

        // Desactivar también el canvas de notas
        if (canvasNotas != null) 
        {
            canvasNotas.SetActive(false);
        }
    }

    private bool EsDiaYHoraPermitida()
    {
        if (PlayerStatsManager.Instance == null) return false;

        string diaActual = PlayerStatsManager.Instance.ObtenerDiaSemana();
        if (!diasPermitidos.Contains(diaActual)) return false;

        var (horaActual, minutoActual) = PlayerStatsManager.Instance.GetHoraYMinutosActual();
        int totalActual = horaActual * 60 + minutoActual;
        int totalInicio = horaInicio * 60 + minutoInicio;
        int totalFin = horaFin * 60 + minutoFin;

        return totalActual >= totalInicio && totalActual < totalFin;
    }
}