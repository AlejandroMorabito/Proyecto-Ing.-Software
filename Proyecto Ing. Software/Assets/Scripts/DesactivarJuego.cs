using UnityEngine;
using System.Collections.Generic;

public class CanvasInitialDeactivator : MonoBehaviour
{
    [Header("Lista de Canvases a Desactivar")]
    [Tooltip("Arrastra aquí todos los canvases que quieres desactivar al inicio")]
    public List<GameObject> canvasesToDeactivate;

    void Start()
    {
        DeactivateAllCanvases();
    }

    /// <summary>
    /// Desactiva todos los canvases en la lista
    /// </summary>
    public void DeactivateAllCanvases()
    {
        if (canvasesToDeactivate == null || canvasesToDeactivate.Count == 0)
        {
            Debug.LogWarning("La lista de canvases está vacía o no asignada", this);
            return;
        }

        foreach (GameObject canvas in canvasesToDeactivate)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
                Debug.Log($"Canvas {canvas.name} desactivado", canvas);
            }
            else
            {
                Debug.LogWarning("Se encontró un elemento nulo en la lista de canvases", this);
            }
        }
    }

    /// <summary>
    /// Método público para reactivar todos los canvases
    /// </summary>
    public void ActivateAllCanvases()
    {
        foreach (GameObject canvas in canvasesToDeactivate)
        {
            if (canvas != null)
            {
                canvas.SetActive(true);
            }
        }
    }
}