using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HagmanController : MonoBehaviour
{
    [SerializeField] GameObject wordContainer;
    [SerializeField] GameObject keyboardContainer;
    [SerializeField] GameObject letterContainer;
    [SerializeField] GameObject[] hagmanStages;
    [SerializeField] GameObject letterButton;
    [SerializeField] TextAsset possibleWord;

    private string word;
    private int incorrectGuesses, correctGuesses;

    void Start()
    {
        // Puedes inicializar aquí si lo necesitas
    }

    public void InitialiseButtons()
    {
        // Corrección: Usar '=' en vez de '-' y '<=' para incluir la Z (ASCII 90)
        for (int i = 65; i <= 90; i++)
        {
            CreateButton(i);
        }
    }

    public void CreateButton(int i)
    {
        GameObject temp = Instantiate(letterButton, keyboardContainer.transform);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = ((char)i).ToString();
        temp.GetComponent<Button>().onClick.AddListener(delegate { CheckLetter(((char)i).ToString()); });
    }

    // Asegúrate de tener este método implementado en tu clase
    public void CheckLetter(string letter)
    {
        // Lógica para comprobar la letra
    }
}
