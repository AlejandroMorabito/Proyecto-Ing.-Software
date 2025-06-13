using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BotonIniciar : MonoBehaviour
{
    public TMP_InputField nombreInput; // Cambia a TMP_InputField
    public PlayerStatsManager PlayerStatsManager; // Asigna el script destino desde el inspector
    public Button boton; // Asigna el botón desde el inspector

    void Start()
    {
        // Asegúrate de que PlayerStatsManager esté asignado
        if (PlayerStatsManager == null)
        {
            PlayerStatsManager = FindObjectOfType<PlayerStatsManager>();
            if (PlayerStatsManager == null)
            {
                Debug.LogError("PlayerStatsManager no encontrado en la escena.");
            }
        }

        // Deshabilita el botón si el input está vacío al inicio
        boton.interactable = !string.IsNullOrWhiteSpace(nombreInput.text);

        // Escucha cambios en el input
        nombreInput.onValueChanged.AddListener(delegate { ValidarInput(); });
    }

    void ValidarInput()
    {
        boton.interactable = !string.IsNullOrWhiteSpace(nombreInput.text);
    }

    public void AlPresionarBoton()
    {
        Debug.Log("Botón presionado");
        string textoIngresado = nombreInput.text;
        PlayerStatsManager.setnombre(textoIngresado);
    }
}
