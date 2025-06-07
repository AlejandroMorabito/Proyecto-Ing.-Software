using UnityEngine;

public class ActivarCanvas : MonoBehaviour
{
    public GameObject canvasUI;
    public GameObject HUDCanvas;
    private bool jugadorDentro = false;
    public int Estres = 0;
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
        if (Estres == 100)
        {
            Debug.Log("Nivel de estres alto");
        }
        else if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            bool canvasActivo = !canvasUI.activeSelf;
            canvasUI.SetActive(canvasActivo);
            HUDCanvas.SetActive(false);

            if (playerController != null)
            {
                playerController.enabled = !canvasActivo; // Desactiva el control del jugador
            }
        }
    }
}




