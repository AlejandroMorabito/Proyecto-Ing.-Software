using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuPausaCanvas;
    public GameObject calendarioCanvas;
    public GameObject horarioCanvas;
    public GameObject cronogramaCanvas; 
    public PlayerController playerController;
    public Button botonReanudar;
    public Button botonCronograma;
    public Button botonGuardar;
    public Button botonSalir;

    [Header("Canvas Adicionales")]
    public List<GameObject> canvasAdicionales = new List<GameObject>(); // Lista de canvas que se deben cerrar al pausar

    private bool juegoPausado = false;

    void Start()
    {
        // Configurar el estado inicial
        menuPausaCanvas.SetActive(false);
        calendarioCanvas.SetActive(false);
        horarioCanvas.SetActive(false); 
        cronogramaCanvas.SetActive(false);
    
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
                // Verificar si hay algún canvas adicional abierto
                bool algunCanvasAbierto = VerificarCanvasAbiertos();

                if (algunCanvasAbierto)
                {
                    CerrarTodosLosCanvas();
                }
                else
                {
                    PausarJuego();
                }
            }
        }
    }

    bool VerificarCanvasAbiertos()
    {
        // Verificar canvas predefinidos
        if (calendarioCanvas.activeSelf || horarioCanvas.activeSelf || cronogramaCanvas.activeSelf)
            return true;

        // Verificar canvas adicionales de la lista
        foreach (GameObject canvas in canvasAdicionales)
        {
            if (canvas != null && canvas.activeSelf)
                return true;
        }

        return false;
    }

    void CerrarTodosLosCanvas()
    {
        // Cerrar canvas predefinidos
        calendarioCanvas.SetActive(false);
        horarioCanvas.SetActive(false);
        cronogramaCanvas.SetActive(false);

        // Cerrar canvas adicionales
        foreach (GameObject canvas in canvasAdicionales)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }

        // Reactivar controles del jugador si no estamos en pausa
        if (!juegoPausado && playerController != null)
        {
            playerController.enabled = true;
        }
    }

    void PausarJuego()
    {
        juegoPausado = true;
        Time.timeScale = 0f;
        
        // Asegurarse que todos los canvas están cerrados
        CerrarTodosLosCanvas();
        
        // Mostrar menú de pausa
        menuPausaCanvas.SetActive(true);
        
        // Deshabilitar controles del jugador
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ReanudarJuego()
    {
        juegoPausado = false;
        Time.timeScale = 1f;
        
        // Ocultar menú de pausa
        menuPausaCanvas.SetActive(false);
        
        // Habilitar controles del jugador
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    void MostrarCronograma()
    {
        menuPausaCanvas.SetActive(false);
        cronogramaCanvas.SetActive(true);
    }

    void GuardarPartida()
    {
        Debug.Log("Guardar partida presionado (función no implementada)");
        // Implementar sistema de guardado aquí
    }

    void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}