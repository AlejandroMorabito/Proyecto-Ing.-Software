using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Notas : MonoBehaviour
{
    public GameObject NotaCanvas;
    public int nota;
    public TMP_Text Nota;
    public TMP_Text textoNota;
    public PlayerController playerController;
    public Button exitbutton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NotaCanvas.SetActive(false); // Asegurarse de que el Canvas esté inactivo al inicio
        exitbutton.onClick.AddListener(CerrarCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        if (NotaCanvas != null)
        {
            if (NotaCanvas.activeSelf)
            {
                Debug.Log("El Canvas hijo está activo.");

                if (textoNota != null && Nota != null)
                {
                    // Importa y muestra el conocimiento del PlayerStatsManager
                    int conocimiento = PlayerStatsManager.Instance != null ? PlayerStatsManager.Instance.Conocimiento : 0;
                    // Calcular nota como un porcentaje del conocimiento (máximo 20)
                    float notaCalculada = (conocimiento / 100f) * 20f;
                    nota = Mathf.RoundToInt(notaCalculada);
                    Nota.text = nota.ToString();
                    textoNota.text = "Tu nota en el examen es: " + nota.ToString() + "/20";
                }
            }
        }
    }

    public void CerrarCanvas()
    {
        NotaCanvas.SetActive(false);
        if (playerController != null)
        {
            playerController.enabled = true;
            GetComponent<Canvas>().enabled = true;
        }
    }
}
