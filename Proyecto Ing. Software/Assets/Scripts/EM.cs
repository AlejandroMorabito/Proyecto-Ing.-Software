using UnityEngine;

public class EM : MonoBehaviour
{
    private bool jugadorDentro = false;
    public HUDController hudController; // Asigna en el Inspector

    private void Update()
    {
        if (jugadorDentro)
        {
            if (hudController != null)
                hudController.MostrarMensaje("Presiona E");

            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerStatsManager.Instance.AddConocimiento(10);
                jugadorDentro = false; // Evita sumar varias veces
                // Opcional: Desactivar el objeto tras obtener el conocimiento
                // gameObject.SetActive(false);
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
}
