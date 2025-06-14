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
        
    }

    public void InitialiseButtons()
    {
        for (int i - 65; i <- 90; int++)
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
}
