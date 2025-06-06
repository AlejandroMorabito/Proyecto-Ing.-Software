/*
this script manages the sets the Borders at the edges of the screen
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class SetBorder : MonoBehaviour {

	/*<summary>Possible positions for the Border<summary>*/
	public enum BorderTypes {Top = 0,Right = 1, Bottom = 2, Left = 3};

	/*<summary>the current position for the Border<summary>*/
	public BorderTypes BorderType = BorderTypes.Top;

    /*<summary>this number is basically how much you want to see the border, note: it should be small<summary>*/
    public float extraOffset = 0.1f;

	//on awake move the borders to the correct locations
	void Awake()
	{

		//get current position and scale
		Vector3 Pos = gameObject.transform.position;
		Vector3 Scale = gameObject.transform.localScale;

		//depending on the Border move and place where it belongs
		if (BorderType == BorderTypes.Bottom)
		{

            Pos = Camera.main.ScreenToWorldPoint(new Vector3((float)Camera.main.pixelWidth/2f,0f,10f));

            Pos.y -= (Scale.y/(2f + extraOffset));

            gameObject.transform.position = Pos;

            Scale.x += 20f; //Screen.width / (Screen.dpi/4f);
            gameObject.transform.localScale = Scale;
       
		}
		else if (BorderType == BorderTypes.Top)
		{
            Pos = Camera.main.ScreenToWorldPoint(new Vector3((float)Camera.main.pixelWidth/2f,(float)Camera.main.pixelHeight,10f));

            Pos.y += (Scale.y/(2f + extraOffset));

            gameObject.transform.position = Pos;

            Scale.x += 20f; //Screen.width / (Screen.dpi/4f);
            gameObject.transform.localScale = Scale;
		}
		else if (BorderType == BorderTypes.Left)
		{
            Pos = Camera.main.ScreenToWorldPoint(new Vector3(0f,(float)Camera.main.pixelHeight/2f,10f));

            Pos.x -= (Scale.x/(2f + extraOffset));
            gameObject.transform.position = Pos;

            Scale.y += 20f; //Screen.height / (Screen.dpi/4f);
			gameObject.transform.localScale = Scale;
		}
		else if (BorderType == BorderTypes.Right)
		{
            Pos = Camera.main.ScreenToWorldPoint(new Vector3((float)Camera.main.pixelWidth,(float)Camera.main.pixelHeight/2f,10f));

            Pos.x += (Scale.x/(2f + extraOffset));
            gameObject.transform.position = Pos;

            Scale.y += 20f; //Screen.height / (Screen.dpi/4f);
			gameObject.transform.localScale = Scale;

		}

	}
	
}
