using UnityEngine;

public class ActivarCanvasDE : MonoBehaviour
{
    public GameObject canvasUI;
    private bool jugadorDentro = false;
    public PlayerController playerController; // Referencia al script de movimiento del jugador

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

    private void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            bool canvasActivo = !canvasUI.activeSelf;
            canvasUI.SetActive(canvasActivo);

            if (playerController != null)
            {
                playerController.enabled = !canvasActivo; // Desactiva el control del jugador
            }
        }
    }
}




