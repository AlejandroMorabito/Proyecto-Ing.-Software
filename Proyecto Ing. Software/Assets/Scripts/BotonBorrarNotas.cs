using UnityEngine;
using UnityEngine.UI;
using System.IO; // Necesario para File y Path

public class BotonBorrarNotas : MonoBehaviour
{
    private string notasFilePath;

    private void Start()
    {
        notasFilePath = Path.Combine(Application.persistentDataPath, "notas_por_materia.txt");
        
        // Asigna automáticamente la función al botón (opcional)
        Button boton = GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.AddListener(BorrarDatosONuevoArchivo);
        }
    }

    public void BorrarDatosONuevoArchivo()
    {
        try
        {
            // Borra el archivo si existe
            if (File.Exists(notasFilePath))
            {
                File.Delete(notasFilePath);
                Debug.Log("Archivo de notas borrado: " + notasFilePath);
            }

            // Crea un nuevo archivo vacío (formato JSON de lista vacía)
            File.WriteAllText(notasFilePath, "[]");
            Debug.Log("Archivo de notas reiniciado (vacío).");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al borrar/crear el archivo: " + e.Message);
        }
    }
}