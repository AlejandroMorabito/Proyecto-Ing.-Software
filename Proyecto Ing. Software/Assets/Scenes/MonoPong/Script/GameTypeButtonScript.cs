/*
This Script can change the GameType when button is pressed
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameTypeButtonScript : MonoBehaviour {

    /*<summary>number that corresponds to the GameType<summary>*/
    public int GameTypeInt = 0;

    /*<summary>text to change<summary>*/
    private Text Txt;
//    private List<Image> Icons = new List<Image>();

    //other scripts/gameObject to reference
    private GameManager gameManager;

    private GameObject notifyObject;
    private Image notifyObjectBackground;
    private Text notifyObjectText;


    private float PressTime = -100f;

	// Use this for initialization
	void Start () 
    {
        notifyObject = GameObject.Find("notifyObject");

        if (notifyObject != null)
        {
            notifyObjectBackground = notifyObject.GetComponent<Image>();
            notifyObjectText = notifyObject.GetComponentInChildren<Text>();
        }

        gameManager = GameObject.FindObjectOfType<GameManager>();

        Txt =  gameObject.transform.Find("Text").GetComponent<Text>();


        if (GameTypeInt == 0)
        {
            gameManager.GameType = GameManager.GameTypes.NoResizing;
            Txt.text = "Mode: Normal";
        }
        else if (GameTypeInt == 1)
        {
            gameManager.GameType = GameManager.GameTypes.ResizeOverTime;
            Txt.text = "Mode: Shrink";
        }
        else if (GameTypeInt ==  2)
        {
            gameManager.GameType = GameManager.GameTypes.PlusMinusBalls;
            Txt.text = "Mode: +/-";
        }

	
	}

    //used for updating the button's icon and interactable variable
    void FixedUpdate()
    {

        if (gameManager.State == GameManager.GameStates.IDLE)
        {
            gameObject.GetComponent<Button>().interactable = true;
            Txt.color = Color.Lerp(Txt.color,new Color(1f,1f,1f,1f), Time.deltaTime * 2.5f);
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = false;
            Txt.color = Color.Lerp(Txt.color,new Color(1f,1f,1f,0.25f), Time.deltaTime * 2.5f);
        }


        if (Time.time - PressTime > 3f)
        {
            if (notifyObjectBackground != null)
            {
                notifyObjectBackground.color =  Color.Lerp(notifyObjectBackground.color, new Color(0f,0f,0f,0f), Time.deltaTime * 1f);
            }

            if (notifyObjectText != null)
            {
                notifyObjectText.color =  Color.Lerp(notifyObjectText.color, new Color(1f,1f,1f,0f), Time.deltaTime * 1f);
            }
        }


    }
	
    //executed if button is pressed
    public void Press()
    {
        GameTypeInt = (GameTypeInt + 1) % 3;

        if (GameTypeInt == 0)
        {
            gameManager.GameType = GameManager.GameTypes.NoResizing;

            notifyObjectBackground.color = new Color(0f,0f,0f,0.75f);
            notifyObjectText.color = new Color(1f,1f,1f,1f);
            notifyObjectText.text = "Resizing Disabled";
            Txt.text = "Mode: Normal";
        }
        else if (GameTypeInt == 1)
        {
            gameManager.GameType = GameManager.GameTypes.ResizeOverTime;

            notifyObjectBackground.color = new Color(0f,0f,0f,0.75f);
            notifyObjectText.color = new Color(1f,1f,1f,1f);
            notifyObjectText.text = "Shrink Over Time";
            Txt.text = "Mode: Shrink";
        }
        else if (GameTypeInt ==  2)
        {
            gameManager.GameType = GameManager.GameTypes.PlusMinusBalls;

            notifyObjectBackground.color = new Color(0f,0f,0f,0.75f);
            notifyObjectText.color = new Color(1f,1f,1f,1f);
            notifyObjectText.text = "+ Good | - Bad";
            Txt.text = "Mode: +/-";
        }


        PressTime =  Time.time;

    }


}
