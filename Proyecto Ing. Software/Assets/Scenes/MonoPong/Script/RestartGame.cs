/*
this script restarts the game
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestartGame : MonoBehaviour {

    private GameManager gameManager;

    public Sprite ResetSprite;

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
	}
	
    //method used when the button is pressed
    public void Press()
    {
        if (gameManager.State != GameManager.GameStates.PLAYING)
        {
            gameManager.SetState(GameManager.GameStates.PLAYING);
            Invoke("ChangeButtonImage",1f);
        }
    }

    //changes the image from the play icon to the restart icon
    private void ChangeButtonImage()
    {
        gameObject.GetComponent<Image>().sprite = ResetSprite;
    }
}
