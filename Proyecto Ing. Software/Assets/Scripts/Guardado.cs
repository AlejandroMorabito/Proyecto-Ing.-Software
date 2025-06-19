using System.IO; // Agrega esto arriba
using TMPro; // Agrega esto arriba
using UnityEngine;
using UnityEngine.SceneManagement; // Agrega esto arriba

public class Guardado : MonoBehaviour
{
    public static bool cargarDesdeArchivo = false; // Variable estática para indicar carga
    public TMPro.TextMeshProUGUI mensajeGuardadoNoExiste; // Asigna este campo en el Inspector
    public HUDController hudController; // Asigna este campo en el Inspector

    public void Guardar()
    {
        // Obtén los valores del PlayerStatsManager
        int estres = PlayerStatsManager.Instance.Estres;
        int conocimiento = PlayerStatsManager.Instance.Conocimiento;
        int dia = PlayerStatsManager.Instance.nDia;
        int semana = PlayerStatsManager.Instance.Semana;
        string nombrePJ = PlayerStatsManager.Instance.NombrePJ;

        // Crea el contenido a guardar
        string datos = $"Nombre: {nombrePJ}\nEstrés: {estres}\nConocimiento: {conocimiento}\nDía: {dia}\nSemana: {semana}";

        // Ruta del archivo (en la carpeta del juego)
        string ruta = Application.persistentDataPath + "/guardado.txt";

        try
        {
            File.WriteAllText(ruta, datos); // Crea o sobreescribe el archivo
            Debug.Log("Juego guardado en: " + ruta);

            if (hudController != null)
                hudController.MostrarMensaje("Datos Guardados");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al guardar: " + e.Message);
        }
    }

    public void Cargar()
    {
        string ruta = Application.persistentDataPath + "/guardado.txt";
        if (!File.Exists(ruta))
        {
            Debug.LogWarning("No se encontró archivo de guardado.");
            if (mensajeGuardadoNoExiste != null)
            {
                mensajeGuardadoNoExiste.text = "No se encontró archivo de guardado.";
                mensajeGuardadoNoExiste.gameObject.SetActive(true);
            }
            return;
        }

        if (mensajeGuardadoNoExiste != null)
            mensajeGuardadoNoExiste.gameObject.SetActive(false);

        string[] lineas = File.ReadAllLines(ruta);
        string nombrePJ = "";
        int estres = 0, conocimiento = 0, dia = 1, semana = 1;

        foreach (string linea in lineas)
        {
            if (linea.StartsWith("Nombre:"))
                nombrePJ = linea.Replace("Nombre:", "").Trim();
            else if (linea.StartsWith("Estrés:"))
                int.TryParse(linea.Replace("Estrés:", "").Trim(), out estres);
            else if (linea.StartsWith("Conocimiento:"))
                int.TryParse(linea.Replace("Conocimiento:", "").Trim(), out conocimiento);
            else if (linea.StartsWith("Día:"))
                int.TryParse(linea.Replace("Día:", "").Trim(), out dia);
            else if (linea.StartsWith("Semana:"))
                int.TryParse(linea.Replace("Semana:", "").Trim(), out semana);
        }

        // Asigna los valores al PlayerStatsManager
        PlayerStatsManager.Instance.setnombre(nombrePJ);
        PlayerStatsManager.Instance.SetEstres(estres);
        PlayerStatsManager.Instance.SetConocimiento(conocimiento);
        PlayerStatsManager.Instance.SetDia(dia);
        PlayerStatsManager.Instance.SetSemana(semana);

        Debug.Log("Juego cargado desde: " + ruta);

        // Marca que se está cargando desde archivo y cambia de escena
        cargarDesdeArchivo = true;
        PlayerStatsManager.Instance.IniciarReloj();
        SceneManager.LoadScene("Casa"); // Cambia "Juego" por el nombre real de tu escena principal

    }
}
