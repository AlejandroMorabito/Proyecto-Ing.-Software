using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Notas : MonoBehaviour
{
    public GameObject NotaCanvas;
    public GameObject FinalCanvas;
    public int nota;
    public TMP_Text Nota;
    public TMP_Text textoNota;
    public PlayerController playerController;
    public Button exitButton;
    public TMP_Text historialNotasText; // Nuevo campo para mostrar el historial
    public HUDController hudController;
    private string notasFilePath;

    void Start()
    {
        NotaCanvas.SetActive(false);
        FinalCanvas.SetActive(false);
        exitButton.onClick.AddListener(CerrarCanvas);
        notasFilePath = Path.Combine(Application.persistentDataPath, "notas_por_materia.txt");
    }

    void Update()
    {
        if (NotaCanvas.activeSelf)
        {
            if (textoNota != null && Nota != null)
            {
                int conocimiento = PlayerStatsManager.Instance?.Conocimiento ?? 0;
                string materia = hudController.MostrarMensajeSiEnHorarioYDia();
                float notaCalculada = (conocimiento / 100f) * 20f;
                nota = Mathf.RoundToInt(notaCalculada);
                Nota.text = nota.ToString();
                textoNota.text = "Tu nota en el examen de " + materia + " es: " + nota.ToString() + "/20";
                GuardarNotaEnArchivo(materia, nota);
                MostrarHistorialNotas(); // Actualizar el historial cada vez que se abre el Canvas
            }
        }
        if (PlayerStatsManager.Instance.Semana == 13 && PlayerStatsManager.Instance.nDia == 5)
        {
            FinalCanvas.SetActive(true);
        }
    }

    public void CerrarCanvas()
    {
        NotaCanvas.SetActive(false);
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private void GuardarNotaEnArchivo(string materia, int nuevaNota)
    {
        List<NotasPorMateria> todasLasNotas = new List<NotasPorMateria>();

        if (File.Exists(notasFilePath))
        {
            string jsonData = File.ReadAllText(notasFilePath);
            todasLasNotas = JsonUtility.FromJson<List<NotasPorMateria>>(jsonData) ?? new List<NotasPorMateria>();
        }

        NotasPorMateria materiaExistente = todasLasNotas.FirstOrDefault(m => m.Materia == materia);

        if (materiaExistente != null)
        {
            materiaExistente.Notas.Add(nuevaNota);
        }
        else
        {
            NotasPorMateria nuevaMateria = new NotasPorMateria
            {
                Materia = materia,
                Notas = new List<int> { nuevaNota }
            };
            todasLasNotas.Add(nuevaMateria);
        }

        string json = JsonUtility.ToJson(todasLasNotas, true);
        File.WriteAllText(notasFilePath, json);
    }

    // Nueva función para leer y mostrar el historial de notas
    private void MostrarHistorialNotas()
    {
        if (!File.Exists(notasFilePath) || historialNotasText == null)
            return;

        string jsonData = File.ReadAllText(notasFilePath);
        List<NotasPorMateria> todasLasNotas = JsonUtility.FromJson<List<NotasPorMateria>>(jsonData);

        if (todasLasNotas == null || todasLasNotas.Count == 0)
        {
            historialNotasText.text = "No hay notas registradas.";
            return;
        }

        string textoHistorial = "Historial de Notas:\n\n";
        foreach (var materia in todasLasNotas)
        {
            textoHistorial += $"- {materia.Materia}:\n";
            textoHistorial += $"  • Promedio: {materia.Notas.Average():F1}/20\n";
            textoHistorial += $"  • Notas: {string.Join(", ", materia.Notas)}\n\n";
        }

        historialNotasText.text = textoHistorial;
    }
    public void BorrarDatosONuevoArchivo()
    {
        // Si el archivo existe, lo borramos
        if (File.Exists(notasFilePath))
        {
            File.Delete(notasFilePath);
            Debug.Log("Archivo de notas borrado.");
        }

        // Creamos un archivo nuevo vacío (o lo sobreescribimos)
        File.WriteAllText(notasFilePath, "[]"); // JSON vacío (lista vacía)
        Debug.Log("Archivo de notas reiniciado (vacío).");
    }
}
[System.Serializable]
public class NotasPorMateria
{
    public string Materia;
    public List<int> Notas = new List<int>();
}