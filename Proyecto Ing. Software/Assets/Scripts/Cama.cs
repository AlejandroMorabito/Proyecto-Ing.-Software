using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cama : MonoBehaviour
{
    private bool jugadorDentro = false;
    public Image blinkImage; // Asigna una imagen negra con alpha 0 en el Canvas
    public float blinkDuration = 0.2f;
    public HUDController hudController;
    public Guardado guardado; // <-- Asigna este campo manualmente en el Inspector

    private void Start()
    {
        blinkImage.gameObject.SetActive(false); // Asegúrate de que la imagen está desactivada al inicio
        // Buscar el HUDController al inicio
        hudController = FindObjectOfType<HUDController>();
        if (hudController == null)
        {
            Debug.LogWarning("HUDController no encontrado. Asegúrate de que existe en la escena.");
        }
        // Verificación segura de PlayerStatsManager
        if (PlayerStatsManager.Instance == null)
        {
            Debug.LogError("PlayerStatsManager no encontrado. Asegúrate de que está inicializado antes de Cama.");
        }
        else
        {
            PlayerStatsManager.Instance.OnHoraCambiada += hudController.ActualizarHoraUI;
        }
        // Asegúrate de que el canvas de la cama está desactivado al inicio
        if (blinkImage != null)
        {
            blinkImage.color = new Color(0, 0, 0, 0); // Asegúrate de que la imagen es transparente al inicio
        }
        else
        {
            Debug.LogError("blinkImage no asignada en el inspector. Asigna una imagen para el efecto de parpadeo.");
        }
    }
    private void Update()
    {
        if (jugadorDentro)
        {
            // Mostrar mensaje solo si tenemos referencia al HUDController
            if (hudController != null)
            {
                hudController.MostrarMensaje($"Presiona E para dormir");
            }
        }

        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Blink());
            PlayerStatsManager.Instance.AvanzarDia(1);
            PlayerStatsManager.Instance.ReiniciarReloj();

            // Llama a Guardar solo si está asignado en el inspector
            if (guardado != null)
            {
                guardado.Guardar();
            }
            else
            {
                Debug.LogWarning("No se ha asignado el objeto Guardado en el Inspector.");
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
}
