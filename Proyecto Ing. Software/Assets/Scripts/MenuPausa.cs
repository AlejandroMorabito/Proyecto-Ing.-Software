using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuPausaCanvas;
    public PlayerController playerController; // Referencia a tu script de control del jugador
    public Button botonReanudar;
    public Button botonCronograma;
    public Button botonGuardar;
    public Button botonSalir;

    private bool juegoPausado = false;

    void Start()
    {
        // Configurar el estado inicial
        menuPausaCanvas.SetActive(false);
        
        // Asignar listeners a los botones
        botonReanudar.onClick.AddListener(ReanudarJuego);
        botonCronograma.onClick.AddListener(MostrarCronograma);
        botonGuardar.onClick.AddListener(GuardarPartida);
        botonSalir.onClick.AddListener(SalirDelJuego);
    }

    void Update()
    {
        // Detectar cuando se presiona ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                ReanudarJuego();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    void PausarJuego()
    {
        juegoPausado = true;
        Time.timeScale = 0f; // Pausar el tiempo del juego
        
        // Mostrar menú de pausa
        menuPausaCanvas.SetActive(true);
        
        // Deshabilitar controles del jugador
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Opcional: Bloquear y mostrar el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ReanudarJuego()
    {
        juegoPausado = false;
        Time.timeScale = 1f; // Reanudar el tiempo del juego
        
        // Ocultar menú de pausa
        menuPausaCanvas.SetActive(false);
        
        // Habilitar controles del jugador
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Opcional: Ocultar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void MostrarCronograma()
    {
        Debug.Log("Cronograma presionado (función no implementada)");
        // Aquí puedes implementar la lógica del cronograma
    }

    void GuardarPartida()
    {
        Debug.Log("Guardar partida presionado (función no implementada)");
        // Aquí puedes implementar el sistema de guardado
    }

    void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
        
        // Si estás en el editor de Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}