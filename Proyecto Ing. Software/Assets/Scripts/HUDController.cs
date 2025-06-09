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
        }
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

    private void ActualizarHoraUI(string hora)
    {
        if (textoHora != null)
        {
            textoHora.text = hora;
            Debug.Log("Hora actualizada en UI: " + hora);
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
}