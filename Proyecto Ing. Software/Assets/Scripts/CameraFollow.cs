using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Referencia al personaje
    public Vector3 offset;   // Offset opcional para ajustar la posición de la cámara
    public float smoothSpeed = 5f; // Velocidad de suavizado

    void LateUpdate()
    {
        if (target != null)
        {
            // Posición deseada de la cámara con suavizado
            Vector3 desiredPosition = target.position + offset;
            transform.position = target.position + offset;
        }
    }
}