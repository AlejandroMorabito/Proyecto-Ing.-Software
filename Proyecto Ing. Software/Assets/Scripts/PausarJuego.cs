using UnityEngine;

public class PausarJuego : MonoBehaviour
{
    // public GameObject menuPausa; // Cambiado de "Inicio" a "menuPausa" para coincidir con tu código
    // public bool juegoPausado = false;

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         // if (juegoPausado)
    //         // {
    //         //     Reanudar();
    //         // }
    //         // else
    //         // {
    //         //     Pausar();
    //         // }
    //         bool canvasActivo = !canvasUI.activeSelf;
    //         canvasUI.SetActive(canvasActivo);

    //         if (playerController != null)
    //         {
    //             playerController.enabled = !canvasActivo; // Desactiva el control del jugador
    //         }
    //     }
    // }
}
//     public void Reanudar()
//     {
//         menuPausa.SetActive(false);
//         Time.timeScale = 1f;
//         juegoPausado = false;
//     }

//     public void Pausar()
//     {
//         menuPausa.SetActive(true);
//         Time.timeScale = 0f;
//         juegoPausado = true;
//     }
// }


// public Button exitButton; // Nuevo botón para salir
// exitButton.onClick.AddListener(ExitGame);

// public void ExitGame()
//    {
//        #if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//        #else
//        Application.Quit();
//        #endif
//     }
