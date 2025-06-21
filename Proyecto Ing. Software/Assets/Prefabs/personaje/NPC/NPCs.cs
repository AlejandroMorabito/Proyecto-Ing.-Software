using UnityEngine;
using TMPro; // Agrega esto

public class NPCs : MonoBehaviour
{
    private bool jugadorDentro = false;
    public string nombreNPC; // Nombre del NPC
    public GameObject globodialogo; // Asignar en el Inspector
    public string[] dialogos; // Asignar en el Inspector
    public HUDController hudController; // Asigna en el Inspector
    public TextMeshProUGUI textoDialogo; // Asigna en el Inspector

    private void Start()
    {
        // Aquí podrías inicializar algo si es necesario
        if (hudController == null)
        {
            hudController = FindObjectOfType<HUDController>();
            if (hudController == null)
            {
                Debug.LogError("HUDController no encontrado en la escena.");
            }
        }
        if (string.IsNullOrEmpty(nombreNPC))
        {
            nombreNPC = "NPC Desconocido"; // Valor por defecto si no se asigna
        }
        globodialogo.SetActive(false); // Asegurarse de que el globo de diálogo esté oculto al inicio
        
    }
    private void Update()
    {
        if (jugadorDentro)
        {
            if (hudController != null)
                hudController.MostrarMensaje($"Presiona E para hablar con {nombreNPC}");

            if (Input.GetKeyDown(KeyCode.E))
            {
                globodialogo.SetActive(true);
                if (dialogos.Length > 0 && textoDialogo != null)
                {
                    int indice = Random.Range(0, dialogos.Length);
                    textoDialogo.enableAutoSizing = true; // Habilita el autoajuste
                    textoDialogo.fontSizeMin = 0;        // Tamaño mínimo recomendado
                    textoDialogo.fontSizeMax = 1;        // Tamaño máximo recomendado
                    textoDialogo.text = dialogos[indice];
                }
            }
        }
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
            globodialogo.SetActive(false);
        }
    }
}
