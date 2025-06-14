using UnityEngine;
using System;

/// <summary>
/// Administra las estadísticas del jugador y el sistema de reloj.
/// </summary>
public class PlayerStatsManager : MonoBehaviour
{
    // Singleton para acceso global a la instancia de PlayerStatsManager
    public static PlayerStatsManager Instance { get; private set; }

    // Estadísticas del jugador (valores entre 0 y 100)
    public int _conocimiento = 0;
    public int _estres = 0;
    public string _nombrePJ = null;
    public int _semana = 1; // Semana actual del juego
    private int _dia = 1;   // Día de la semana (1 = lunes, ..., 7 = domingo)

    // Sistema de reloj mejorado
    public DateTime _horaActual = new DateTime(1, 1, 1, 6, 0, 0); // Hora inicial: 6:00 AM
    public bool _relojPausado = false; // Indica si el reloj está pausado
    public float _segundoContador = 0f; // Acumula segundos para avanzar el reloj
    public event Action<string> OnHoraCambiada = delegate { }; // Evento para notificar cambios de hora

    // Eventos para actualizar el HUD cuando cambian las estadísticas
    public delegate void StatChanged(int valor);
    public static event StatChanged OnConocimientoChanged;
    public static event StatChanged OnEstresChanged;

    /// <summary>
    /// Inicializa el Singleton y configura el objeto para que no se destruya al cambiar de escena.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"¡Múltiples instancias de PlayerStatsManager! Destruyendo {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Mantiene el objeto al cambiar de escena
        Debug.Log($"PlayerStatsManager inicializado. Instancia ID: {GetInstanceID()}");
        OnHoraCambiada?.Invoke(GetHoraFormateada()); // Notifica la hora inicial
    }

    /// <summary>
    /// Actualiza el reloj cada segundo si no está pausado.
    /// </summary>
    private void Update()
    {
        if (!_relojPausado)
        {
            _segundoContador += Time.deltaTime;

            // Avanza el reloj cada segundo real
            if (_segundoContador >= 1f)
            {
                _segundoContador = 0f;
                AvanzarReloj(1); // Avanza 1 minuto

                // Pausa el reloj a las 18:00
                if (_horaActual.Hour == 18 && _horaActual.Minute == 0)
                {
                    PausarReloj();
                }
            }
        }
    }

    #region Nombre PJ
    /// <summary>
    /// Establece el nombre del personaje.
    /// </summary>
    public void setnombre(string n)
    {
        _nombrePJ = n;
        Debug.Log($"Nombre del personaje establecido: {_nombrePJ}");
    }

    /// <summary>
    /// Obtiene el nombre del personaje.
    /// </summary>
    public string NombrePJ => _nombrePJ;
    #endregion

    #region Métodos del Reloj

    /// <summary>
    /// Avanza el reloj en la cantidad de minutos indicada.
    /// </summary>
    public void AvanzarReloj(int minutos)
    {
        _horaActual = _horaActual.AddMinutes(minutos);
        OnHoraCambiada?.Invoke(GetHoraFormateada()); // Notifica el cambio de hora
        Debug.Log("Hora actual: " + GetHoraFormateada());
    }

    /// <summary>
    /// Pausa el avance del reloj.
    /// </summary>
    public void PausarReloj()
    {
        _relojPausado = true;
        Debug.Log("Reloj pausado a las " + GetHoraFormateada());
    }

    /// <summary>
    /// Reanuda el avance del reloj.
    /// </summary>
    public void ReanudarReloj()
    {
        _relojPausado = false;
        Debug.Log("Reloj reanudado a las " + GetHoraFormateada());
    }

    /// <summary>
    /// Reinicia el reloj a las 6:00 AM y lo reanuda.
    /// </summary>
    public void ReiniciarReloj()
    {
        _horaActual = new DateTime(1, 1, 1, 6, 0, 0);
        _relojPausado = false;
        OnHoraCambiada?.Invoke(GetHoraFormateada());
    }

    /// <summary>
    /// Devuelve la hora actual en formato hh:mm AM/PM.
    /// </summary>
    public string GetHoraFormateada()
    {
        // Reemplaza el espacio por un espacio no separable para evitar saltos de línea
        return _horaActual.ToString("hh:mm tt").Replace(" ", "\u00A0").ToUpper();
    }

    /// <summary>
    /// Devuelve la hora y los minutos actuales como tupla.
    /// </summary>
    public (int hora, int minutos) GetHoraYMinutosActual()
    {
        return (_horaActual.Hour, _horaActual.Minute);
    }
    #endregion

    #region Métodos de Estadísticas

    /// <summary>
    /// Suma la cantidad indicada al conocimiento, limitado entre 0 y 100.
    /// </summary>
    public void AddConocimiento(int cantidad)
    {
        int nuevoValor = Mathf.Clamp(_conocimiento + cantidad, 0, 100);
        Debug.Log($"Actualizando conocimiento: {_conocimiento} + {cantidad} = {nuevoValor}");
        _conocimiento = nuevoValor;
        OnConocimientoChanged?.Invoke(_conocimiento);
        Debug.Log($"Evento OnConocimientoChanged invocado con {_conocimiento}");
    }

    /// <summary>
    /// Suma la cantidad indicada al estrés, limitado entre 0 y 100.
    /// </summary>
    public void AddEstres(int cantidad)
    {
        int nuevoValor = Mathf.Clamp(_estres + cantidad, 0, 100);
        Debug.Log($"Actualizando estrés: {_estres} + {cantidad} = {nuevoValor}");
        _estres = nuevoValor;
        OnEstresChanged?.Invoke(_estres);
        Debug.Log($"Evento OnEstresChanged invocado con {_estres}");
    }

    /// <summary>
    /// Establece el valor del conocimiento directamente (0-100).
    /// </summary>
    public void SetConocimiento(int valor)
    {
        _conocimiento = Mathf.Clamp(valor, 0, 100);
        OnConocimientoChanged?.Invoke(_conocimiento);
    }

    /// <summary>
    /// Establece el valor del estrés directamente (0-100).
    /// </summary>
    public void SetEstres(int valor)
    {
        _estres = Mathf.Clamp(valor, 0, 100);
        OnEstresChanged?.Invoke(_estres);
    }

    // Propiedades para acceder a las estadísticas
    public int Conocimiento => _conocimiento;
    public int Estres => _estres;
    public int Semana => _semana;
    #endregion

    /// <summary>
    /// Muestra en consola el estado actual de las estadísticas.
    /// </summary>
    public void DebugStats()
    {
        Debug.Log($"Estado actual - Conocimiento: {_conocimiento}, Estres: {_estres}, Semana: {_semana}, Hora: {GetHoraFormateada()}");
    }

    /// <summary>
    /// Devuelve el nombre del día de la semana actual.
    /// </summary>
    public string ObtenerDiaSemana()
    {
        switch (_dia)
        {
            case 1: return "Lunes";
            case 2: return "Martes";
            case 3: return "Miércoles";
            case 4: return "Jueves";
            case 5: return "Viernes";
            case 6: return "Sábado";
            case 7: return "Domingo";
            default: return "Día inválido";
        }
    }

    /// <summary>
    /// Avanza el día de la semana y la semana si corresponde.
    /// </summary>
    public void AvanzarDia(int cantidad)
    {
        _dia += cantidad;
        if (_dia > 7)
        {
            _dia = 1;
            _semana++;
        }
        Debug.Log($"Día avanzado a: {ObtenerDiaSemana()}, Semana: {_semana}");
    }
}