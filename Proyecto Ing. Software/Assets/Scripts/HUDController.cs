using UnityEngine;
using TMPro;
using System.Collections;

public class HUDController : MonoBehaviour
{
    [Header("Textos de Estadísticas")]
    public TMP_Text textoConocimiento;
    public TMP_Text textoEstres;

    [Header("Reloj")]
    public TMP_Text textoHora;

    [Header("Mensajes Temporales")]
    public TMP_Text textoMensaje;
    public float duracionMensaje = 3f;
    private Coroutine mensajeCoroutine;

    private void Awake()
    {
        // Actualización inicial forzada
        if (PlayerStatsManager.Instance != null)
        {
            ActualizarHoraUI(PlayerStatsManager.Instance.GetHoraFormateada());
            ActualizarConocimientoUI(PlayerStatsManager.Instance.Conocimiento);
            ActualizarEstresUI(PlayerStatsManager.Instance.Estres);
        }
        else
        {
            Debug.LogWarning("PlayerStatsManager no encontrado en Awake. Asegúrate de que esté inicializado antes de HUDController.");
        }
        // Ajustar auto-sizing de los textos
        AjustarAutoSizeTMP(textoConocimiento);
        AjustarAutoSizeTMP(textoEstres);
        AjustarAutoSizeTMP(textoHora);
        AjustarAutoSizeTMP(textoMensaje);
    }

    private void OnEnable()
    {
        // Suscribirse a eventos
        PlayerStatsManager.OnConocimientoChanged += ActualizarConocimientoUI;
        PlayerStatsManager.OnEstresChanged += ActualizarEstresUI;

        Debug.Log("HUDController suscrito a eventos de estadísticas");

        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.OnHoraCambiada += ActualizarHoraUI;
            // Actualizar inmediatamente al habilitarse
            ActualizarHoraUI(PlayerStatsManager.Instance.GetHoraFormateada());
        }
    }

    private void OnDisable()
    {
        // Desuscribirse
        PlayerStatsManager.OnConocimientoChanged -= ActualizarConocimientoUI;
        PlayerStatsManager.OnEstresChanged -= ActualizarEstresUI;
        Debug.Log("HUDController desuscrito de eventos de estadísticas");

        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.OnHoraCambiada -= ActualizarHoraUI;
        }
    }

    private void ActualizarConocimientoUI(int valor)
    {
        Debug.Log($"Actualizando UI Conocimiento a {valor}");
        if (textoConocimiento != null)
            textoConocimiento.text = $"{valor}";
    }

    private void ActualizarEstresUI(int valor)
    {
        Debug.Log($"Actualizando UI Estrés a {valor}");
        if (textoEstres != null)
            textoEstres.text = $"{valor}";
    }

    public void ActualizarHoraUI(string hora)
    {
        if (textoHora != null)
        {
            textoHora.text = $"{PlayerStatsManager.Instance.NombrePJ}\n\nSemana {PlayerStatsManager.Instance.Semana}\n{PlayerStatsManager.Instance.ObtenerDiaSemana()}\n{hora}\n{MostrarMensajeSiEnHorarioYDia()}";
        }
        else
        {
            Debug.LogWarning("Texto para hora no asignado en el Inspector");
        }
    }

    // Función pública para mostrar mensajes
    public void MostrarMensaje(string mensaje, float duracionPersonalizada = -1f)
    {
        if (textoMensaje == null) return;

        // Detener la corrutina anterior si existe
        if (mensajeCoroutine != null)
        {
            StopCoroutine(mensajeCoroutine);
        }

        // Mostrar el mensaje
        textoMensaje.text = mensaje;
        textoMensaje.gameObject.SetActive(true);

        // Iniciar nueva corrutina con la duración
        float duracion = duracionPersonalizada > 0 ? duracionPersonalizada : duracionMensaje;
        mensajeCoroutine = StartCoroutine(OcultarMensajeDespuesDeTiempo(duracion));
    }

    private IEnumerator OcultarMensajeDespuesDeTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        textoMensaje.gameObject.SetActive(false);
        mensajeCoroutine = null;
    }

    private void AjustarAutoSizeTMP(TMP_Text tmpText, int minSize = 14, int maxSize = 36)
    {
        if (tmpText != null)
        {
            tmpText.enableAutoSizing = true;
            tmpText.fontSizeMin = minSize;
            tmpText.fontSizeMax = maxSize;
        }
    }

    public string MostrarMensajeSiEnHorarioYDia()
    {
        if (PlayerStatsManager.Instance == null) return "";

        string diaActual = PlayerStatsManager.Instance.ObtenerDiaSemana();
        (int horaActual, int minutosActuales) = PlayerStatsManager.Instance.GetHoraYMinutosActual();

        if (diaActual == "Lunes" && 
            ((horaActual == 8 && minutosActuales >= 45) ||
             (horaActual == 9) ||
             (horaActual == 10 && minutosActuales == 15)))
        {
            return "Competencias para Aprender\nA1-204";
        }
        else if (diaActual == "Lunes" && 
            ((horaActual == 12 && minutosActuales >= 15) ||
             (horaActual == 1 && minutosActuales == 45)))
        {
            return "Ingles IV\nA1-201";
        }
        else if (diaActual == "Martes" && 
            ((horaActual == 7 && minutosActuales >= 00) ||
             (horaActual == 8 && minutosActuales == 30)))
        {
            return "Matematicas Basica\nA2-204";
        }
        else if (diaActual == "Martes" && 
            ((horaActual == 12 && minutosActuales >= 15) ||
             (horaActual == 1 && minutosActuales == 45)))
        {
            return "Introducción a la Ingenieria\nA1-202";
        }
        else if (diaActual == "Martes" && 
            ((horaActual == 2 && minutosActuales >= 00) ||
             (horaActual == 3 && minutosActuales == 30)))
        {
            return "Pensamiento Computacional\nA1-203";
        }
        else if (diaActual == "Miercoles" && 
            ((horaActual == 8 && minutosActuales >= 45) ||
             (horaActual == 9) ||
             (horaActual == 10 && minutosActuales == 15)))
        {
            return "Competencias para Aprender\nA1-204";
        }
        else if (diaActual == "Miercoles" && 
            ((horaActual == 12 && minutosActuales >= 15) ||
             (horaActual == 1 && minutosActuales == 45)))
        {
            return "Ingles IV\nA1-201";
        }
        else if (diaActual == "Jueves" && 
            ((horaActual == 7 && minutosActuales >= 00) ||
             (horaActual == 8 && minutosActuales == 30)))
        {
            return "Matematicas Basica\nA2-204";
        }
        else if (diaActual == "Jueves" && 
            ((horaActual == 12 && minutosActuales >= 15) ||
             (horaActual == 1 && minutosActuales == 45)))
        {
            return "Introducción a la Ingenieria\nA1-202";
        }
        else if (diaActual == "Jueves" && 
            ((horaActual == 2 && minutosActuales >= 00) ||
             (horaActual == 3 && minutosActuales == 30)))
        {
            return "Pensamiento Computacional\nA1-203";
        }
        else if (diaActual == "Viernes" && 
            ((horaActual == 7 && minutosActuales >= 00) ||
             (horaActual == 8 && minutosActuales == 30)))
        {
            return "Matematicas Basica\nA2-204";
        }

        return "Clase\nSalon";
    }
}