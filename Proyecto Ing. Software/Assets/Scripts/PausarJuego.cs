using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PausarJuego : MonoBehaviour
{
    public GameObject Inicio;
    public bool juegoPausado = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        }
        
            if (juegoPausado)
            {
                Reanudar();

            }
            else
            {
                Pausar();
            }
    }

public void Reanudar()
        {
            menuPausa.SetActive(false);
            Time.timeScale = 1;
            juegoPausado = false;
        }
public void Pausar()
        {
            menuPausa.SetActive(true);
            Time.timeScale = 0;
            juegoPausado = true;
        }
}