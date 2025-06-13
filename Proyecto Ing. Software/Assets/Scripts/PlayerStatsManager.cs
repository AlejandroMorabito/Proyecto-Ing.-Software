using UnityEngine;
using System;

public class PlayerStatsManager : MonoBehaviour
{
    // Singleton
    public static PlayerStatsManager Instance { get; private set; }

    // Valores actuales (0-100)
    public int _conocimiento = 0;
    public int _estres = 0;
    public string _nombrePJ = null;
    public int _semana = 1; // Semana actual del juego

    // Sistema de reloj mejorado
    public DateTime _horaActual = new DateTime(1, 1, 1, 6, 0, 0); // 6:00 AM inicial
    public bool _relojPausado = false;
    public float _segundoContador = 0f;
    public event Action<string> OnHoraCambiada = delegate { }; // Inicializado para evitar null

    // Eventos para actualizar el HUD
    public delegate void StatChanged(int valor);
    public static event StatChanged OnConocimientoChanged;
    public static event StatChanged OnEstresChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"¡Múltiples instancias de PlayerStatsManager! Destruyendo {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"PlayerStatsManager inicializado. Instancia ID: {GetInstanceID()}");
        OnHoraCambiada?.Invoke(GetHoraFormateada());
    }

    private void Update()
    {
        if (!_relojPausado)
        {
            _segundoContador += Time.deltaTime;

            if (_segundoContador >= 1f)
            {
                _segundoContador = 0f;
                AvanzarReloj(1);

                if (_horaActual.Hour == 18 && _horaActual.Minute == 0)
                {
                    PausarReloj();
                }
            }
        }
    }

    #region Nombre PJ
    public void setnombre(string n)
    {
        _nombrePJ = n;
        Debug.Log($"Nombre del personaje establecido: {_nombrePJ}");
    }
    
    public string NombrePJ => _nombrePJ;

    #endregion

    #region Métodos del Reloj
    public void AvanzarReloj(int minutos)
    {
        _horaActual = _horaActual.AddMinutes(minutos);
        OnHoraCambiada?.Invoke(GetHoraFormateada());
        Debug.Log("Hora actual: " + GetHoraFormateada()); // Log para depuración
    }

    public void PausarReloj()
    {
        _relojPausado = true;
        Debug.Log("Reloj pausado a las " + GetHoraFormateada());
    }

    public void ReanudarReloj()
    {
        _relojPausado = false;
        Debug.Log("Reloj reanudado a las " + GetHoraFormateada());
    }

    public void ReiniciarReloj()
    {
        _horaActual = new DateTime(1, 1, 1, 6, 0, 0);
        _relojPausado = false;
        OnHoraCambiada?.Invoke(GetHoraFormateada());
    }

    public string GetHoraFormateada()
    {
        // Reemplaza el espacio por un espacio no separable
        return _horaActual.ToString("hh:mm tt").Replace(" ", "\u00A0").ToUpper();
    }

    public (int hora, int minutos) GetHoraYMinutosActual()
    {
        return (_horaActual.Hour, _horaActual.Minute);
    }
    #endregion

    #region Métodos de Estadísticas (existente)
    public void AddConocimiento(int cantidad)
    {
        int nuevoValor = Mathf.Clamp(_conocimiento + cantidad, 0, 100);
        Debug.Log($"Actualizando conocimiento: {_conocimiento} + {cantidad} = {nuevoValor}");
        _conocimiento = nuevoValor;
        OnConocimientoChanged?.Invoke(_conocimiento);
        Debug.Log($"Evento OnConocimientoChanged invocado con {_conocimiento}");
    }

    public void AddEstres(int cantidad)
    {
        int nuevoValor = Mathf.Clamp(_estres + cantidad, 0, 100);
        Debug.Log($"Actualizando estrés: {_estres} + {cantidad} = {nuevoValor}");
        _estres = nuevoValor;
        OnEstresChanged?.Invoke(_estres);
        Debug.Log($"Evento OnEstresChanged invocado con {_estres}");
    }

    public void SetConocimiento(int valor)
    {
        _conocimiento = Mathf.Clamp(valor, 0, 100);
        OnConocimientoChanged?.Invoke(_conocimiento);
    }

    public void SetEstres(int valor)
    {
        _estres = Mathf.Clamp(valor, 0, 100);
        OnEstresChanged?.Invoke(_estres);
    }

    public int Conocimiento => _conocimiento;
    public int Estres => _estres;
    public int Semana => _semana;
    #endregion

    public void DebugStats()
    {
        Debug.Log($"Estado actual - Conocimiento: {_conocimiento}, Estres: {_estres}, Semana: {_semana}, Hora: {GetHoraFormateada()}");
    }
}