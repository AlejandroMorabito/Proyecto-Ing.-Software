using System.Collections;
using UnityEngine;
using UnityEngine.UI; // <-- Agrega esto
using TMPro;

public class NPCsAC : MonoBehaviour
{
    private bool jugadorDentro = false;
    public string nombreNPC; // Nombre del NPC
    public GameObject globodialogo; // Asignar en el Inspector
    public string dialogo; // Asignar en el Inspector
    public string dialogo2; // Asignar en el Inspector
    public string dialogo3; // Asignar en el Inspector
    public Image blinkImage; // Asigna una imagen negra con alpha 0 en el Canvas
    public GameObject CanvasConfirmacion; // Asignar en el Inspector
    public HUDController hudController; // Asigna en el Inspector
    public TextMeshProUGUI textoDialogo; // Asigna en el Inspector
    [SerializeField] public Vector3 destinoTP; // Ahora aparecerá en el Inspector
    public int horas;
    public int minutos;
    public Sprite nuevoSprite; // Asigna el sprite deseado en el Inspector

    private void Start()
    {
        CanvasConfirmacion.SetActive(false); // Asegúrate de que el NPC esté desactivado al inicio
        blinkImage.gameObject.SetActive(false); // Asegúrate de que la imagen está desactivada al inicio
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
                StartCoroutine(MostrarDialogoYDesaparecer());
            }
        }
    }

    private System.Collections.IEnumerator MostrarDialogoYDesaparecer()
    {
        globodialogo.SetActive(true);
        if (textoDialogo != null)
        {
            textoDialogo.enableAutoSizing = true;
            textoDialogo.fontSizeMin = 0; // Ajusta según tu UI
            textoDialogo.fontSizeMax = 1; // Ajusta según tu UI
            textoDialogo.text = dialogo;
            yield return new WaitForSeconds(3f);
            CanvasConfirmacion.SetActive(true);
        }
    }

    // Métodos públicos para los botones
    public void BotonSI()
    {
        StartCoroutine(SI());
    }

    public void BotonNO()
    {
        StartCoroutine(NO());
    }

    // Corrutinas privadas
    private IEnumerator SI()
    {
        CanvasConfirmacion.SetActive(false);
        StartCoroutine(Blink());
        yield return new WaitForSeconds(1f);
        PlayerStatsManager.Instance.AgregarTiempo(horas, minutos);
        PlayerStatsManager.Instance.AddConocimiento(5);
        textoDialogo.text = dialogo2;
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    private IEnumerator NO()
    {
        CanvasConfirmacion.SetActive(false);
        textoDialogo.text = dialogo3;
        yield return new WaitForSeconds(3f);
        globodialogo.SetActive(false);
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
    
    IEnumerator Blink()
    {
        if (blinkImage == null)
        {
            Debug.LogError("blinkImage no asignada. Asegúrate de asignar una imagen en el inspector.");
            yield break;
        }

        blinkImage.gameObject.SetActive(true);

        float fadeDuration = 0.8f; // Duración del fade in y fade out (en segundos)
        float holdDuration = 0.8f; // Tiempo que permanece completamente negro

        // Fade in (transparente a negro)
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            blinkImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blinkImage.color = new Color(0, 0, 0, 1);

        // --- AQUÍ SE HACE EL TP ---
        TeletransportarJugadorYNPC();

        // Mantener pantalla negra
        yield return new WaitForSeconds(holdDuration);

        // Fade out (negro a transparente)
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            blinkImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blinkImage.color = new Color(0, 0, 0, 0);
        blinkImage.gameObject.SetActive(false);
    }

    public void TeletransportarJugadorYNPC()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            jugador.transform.position = destinoTP;
        }
        // Teletransporta este NPC
        transform.position = new Vector3(destinoTP.x - 1, destinoTP.y + 1, destinoTP.z);

        // Cambia el sprite si se asignó uno nuevo
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && nuevoSprite != null)
        {
            sr.sprite = nuevoSprite;
        }
    }
}
