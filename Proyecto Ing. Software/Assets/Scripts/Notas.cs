using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;


public class Notas : MonoBehaviour
{
    public GameObject NotaCanvas;
    public GameObject FinalCanvas;
    public int nota;
    public TMP_Text Nota;
    public TMP_Text textoNota;
    public PlayerController playerController;
    public Button exitButton;
    public TMP_Text NFText;
    public float NF;
    public HUDController hudController;
    private string notasFilePath;
    private string materiaActual;
    private int notaActual;
    private NotasWrapper notasWrapper;
    public TMP_Text textoFinal; // Añade esta referencia para el texto del FinalCanvas

    void Start()
    {
        NotaCanvas.SetActive(false);
        FinalCanvas.SetActive(false);
        exitButton.onClick.AddListener(OnExitButtonPressed);
        notasFilePath = Path.Combine(Application.persistentDataPath, "notas_por_materia.txt");

        CargarNotasDesdeArchivo();

        if (PlayerStatsManager.Instance.Semana == 12 && PlayerStatsManager.Instance.nDia == 5)
        {
            MostrarPantallaFinal();
        }
    }

    private void CargarNotasDesdeArchivo()
    {
        notasWrapper = new NotasWrapper { NotasPorMateria = new List<NotasPorMateria>() };

        if (File.Exists(notasFilePath))
        {
            string jsonData = File.ReadAllText(notasFilePath);
            try
            {
                notasWrapper = JsonUtility.FromJson<NotasWrapper>(jsonData);
                if (notasWrapper == null || notasWrapper.NotasPorMateria == null)
                {
                    notasWrapper = new NotasWrapper { NotasPorMateria = new List<NotasPorMateria>() };
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error al leer el archivo JSON: " + e.Message);
                notasWrapper = new NotasWrapper { NotasPorMateria = new List<NotasPorMateria>() };
            }
        }
    }

    public void GuardarNotasAArchivo()
    {
        try
        {
            string json = JsonUtility.ToJson(notasWrapper, true);
            File.WriteAllText(notasFilePath, json);
            Debug.Log("Notas guardadas en archivo: " + json);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al guardar notas: " + e.Message);
        }
    }

    void Update()
    {
        if (NotaCanvas.activeSelf)
        {
            if (textoNota != null && Nota != null)
            {
                int conocimiento = PlayerStatsManager.Instance?.Conocimiento ?? 0;
                materiaActual = hudController.MostrarMensajeSiEnHorarioYDia().clase;
                float notaCalculada = (conocimiento / 100f) * 20f;
                notaActual = Mathf.RoundToInt(notaCalculada);
                Nota.text = notaActual.ToString();
                textoNota.text = "Tu nota en el examen de " + materiaActual + " es: " + notaActual.ToString() + "/20";
            }
        }
    }

    public void OnExitButtonPressed()
    {
        if (!string.IsNullOrEmpty(materiaActual))
        {
            AgregarNotaAMemoria(materiaActual, notaActual);
            // Opcional: guardar automáticamente al salir
            // GuardarNotasAArchivo();
        }
        CerrarCanvas();
    }

    private void AgregarNotaAMemoria(string materia, int nuevaNota)
    {
        // Buscar si la materia ya existe
        NotasPorMateria materiaExistente = notasWrapper.NotasPorMateria.FirstOrDefault(m => m.Materia == materia);

        if (materiaExistente != null)
        {
            // Si existe, agregar la nueva nota
            materiaExistente.Notas.Add(nuevaNota);
        }
        else
        {
            // Si no existe, crear nueva entrada
            NotasPorMateria nuevaMateria = new NotasPorMateria
            {
                Materia = materia,
                Notas = new List<int> { nuevaNota }
            };
            notasWrapper.NotasPorMateria.Add(nuevaMateria);
        }

        Debug.Log($"Nota agregada en memoria: {materia} - {nuevaNota}/20");
    }

    public void CerrarCanvas()
    {
        NotaCanvas.SetActive(false);
        Destroy(NotaCanvas);
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    public void BorrarDatosONuevoArchivo()
    {
        // Crear un wrapper vacío
        NotasWrapper wrapper = new NotasWrapper { NotasPorMateria = new List<NotasPorMateria>() };
        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(notasFilePath, json);
        Debug.Log("Archivo de notas reiniciado: " + json);
    }

    // private void MostrarHistorialNotas()
    // {
    //     if (historialNotasText == null)
    //     {
    //         Debug.LogError("El componente historialNotasText no está asignado en el inspector");
    //         return;
    //     }

    //     try
    //     {
    //         if (notasWrapper == null || notasWrapper.NotasPorMateria == null || notasWrapper.NotasPorMateria.Count == 0)
    //         {
    //             historialNotasText.text = "No hay notas registradas.";
    //             return;
    //         }

    //         StringBuilder sb = new StringBuilder();
    //         sb.AppendLine("<b>Historial de Notas:</b>\n");

    //         foreach (var materia in notasWrapper.NotasPorMateria)
    //         {
    //             sb.AppendLine($"<b>- {materia.Materia}:</b>");

    //             int sumaTotal = materia.Notas.Sum();
    //             float promedio = sumaTotal / 4f;

    //             sb.AppendLine($"  • Promedio (suma/4): <color=#FFA500>{promedio:F1}</color>/20");
    //             sb.AppendLine($"  • Notas: {string.Join(", ", materia.Notas)}");
    //             sb.AppendLine($"  • Suma total: {sumaTotal}/80");
    //             sb.AppendLine();
    //         }

    //         historialNotasText.text = sb.ToString();
    //         historialNotasText.ForceMeshUpdate();
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError("Error al mostrar historial de notas: " + e.Message);
    //         historialNotasText.text = "Error al cargar el historial de notas";
    //     }
    // }
    
    private void MostrarPantallaFinal()
    {
        GuardarNotasAArchivo();
        FinalCanvas.SetActive(true);
        
        // Deshabilitar el control del jugador
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Calcular y mostrar información final
        if (textoFinal != null)
        {
            // Configurar auto-ajuste del texto
            textoFinal.enableAutoSizing = true;
            textoFinal.fontSizeMin = 12;  // Tamaño mínimo de fuente
            textoFinal.fontSizeMax = 24;  // Tamaño máximo de fuente
            textoFinal.overflowMode = TextOverflowModes.Truncate;  // O usar TextOverflowModes.Ellipsis
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<size=36><b>RESULTADOS FINALES</b></size>\n");
            
            try
            {
                if (File.Exists(notasFilePath))
                {
                    string jsonData = File.ReadAllText(notasFilePath);
                    NotasWrapper datosDesdeArchivo = JsonUtility.FromJson<NotasWrapper>(jsonData);
                    
                    if (datosDesdeArchivo != null && datosDesdeArchivo.NotasPorMateria != null && datosDesdeArchivo.NotasPorMateria.Count > 0)
                    {
                        foreach (var materia in datosDesdeArchivo.NotasPorMateria)
                        {
                            if (materia.Notas == null || materia.Notas.Count == 0)
                            {
                                sb.AppendLine($"<b>{materia.Materia}:</b> No hay notas registradas\n");
                                continue;
                            }

                            int sumaTotal = materia.Notas.Sum();
                            float promedio = sumaTotal / (float)materia.Notas.Count;

                            sb.AppendLine($"<b>{materia.Materia}:</b>");
                            sb.AppendLine($"Nota Final: <color=#FFD700>{promedio:F1}</color>/20");
                            sb.AppendLine($"Notas: {string.Join(", ", materia.Notas)}");
                            sb.AppendLine();
                            NF += promedio; // Sumar al total de NF
                        }
                    }
                    else
                    {
                        sb.AppendLine("No hay datos de notas disponibles en el archivo");
                    }
                }
                else
                {
                    sb.AppendLine("No se encontró el archivo de notas");
                }
            }
            catch (Exception e)
            {
                sb.AppendLine("Error al leer el archivo de notas");
                Debug.LogError("Error al mostrar pantalla final: " + e.Message);
            }
            
            sb.AppendLine("\n<size=28><i>¡Gracias por jugar!</i></size>");
            textoFinal.text = sb.ToString();
            NFText.text = $"{(NF/5):F1}/20"; // Mostrar NF como un porcentaje de 20

            // Forzar la actualización del texto y su contenedor
            LayoutRebuilder.ForceRebuildLayoutImmediate(textoFinal.rectTransform);
            textoFinal.ForceMeshUpdate();
        }
        
        // Opcional: Ajustar el Content Size Fitter si lo estás usando
        var contentFitter = textoFinal.GetComponent<ContentSizeFitter>();
        if (contentFitter != null)
        {
            contentFitter.SetLayoutVertical();
            contentFitter.SetLayoutHorizontal();
        }
    }
}

[System.Serializable]
public class NotasPorMateria
{
    public string Materia;
    public List<int> Notas = new List<int>();
}

[System.Serializable]
public class NotasWrapper
{
    public List<NotasPorMateria> NotasPorMateria;
}