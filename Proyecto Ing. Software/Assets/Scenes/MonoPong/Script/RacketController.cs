/*
this script controls the Racket, including it's size
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacketController : MonoBehaviour {

    /*<summary>this will contain a list of all the RacketPieces<summary>*/
    private List<GameObject> RacketPieces =  new List<GameObject>();

//    [Range(0.0f, 6f)] //this can be used if you want a slider in the inspector
    /*<summary>Current Size<summary>*/
    public float Size = 6f;

//    [Range(0.0f, 6f)] //this can be used if you want a slider in the inspector
    /*<summary>Size when GameStarts<summary>*/
    public float DefaultSize = 4f;

    /*<summary>Maximum Size<summary>*/
    public float MaxSize = 6f;

    /*<summary>how fast the racket will resize<summary>*/
    public float ResizeSpeed = 1f;

    /*<summary>whether or not the Size is maxed out<summary>*/
    public bool IsMaxSize = false;

    /*<summary>this is actually the real current size<summary>*/
    private float _Size = 1f;

    /*<summary>the radius of the racket<summary>*/
    private float Radius = 2.78f;

    void Start()
    {
        ResetSize();

        RacketPieces.Clear();

        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            RacketPieces.Add(gameObject.transform.GetChild(i).gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        Size = Mathf.Clamp(Size,0f,MaxSize);

        //get the world position of the mouse
        Vector3 WorldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,10f));
        WorldMousePos.z = 0f;

        //get the angle between the gameobject and the WorldMousePos
        float Angle = GetAngleDirection(gameObject.transform.position,WorldMousePos);

        //Update size
        _Size = Mathf.Lerp(_Size,Size,Time.deltaTime * ResizeSpeed);

        //find the where the first RacketPiece should be
        float Angle2 = Angle - ((RacketPieces.Count  * _Size)/2f) ;
        Angle2 += _Size/2f;

        //move each RacketPiece based on the Angle2, and it's index number in the RacketPieces List
        for(int i = 0; i < RacketPieces.Count; i++)
        {
            //find the new position, and update the RacketPiece
            Vector3 V3 = GetXYDirection(Angle2 + (_Size * i),Radius);
            V3 = new Vector3(V3.x,V3.y - 2f,0f);
            RacketPieces[i].transform.position = V3;

            //rotate the RacketPiece to look at the center.
            float A = GetAngleDirection( gameObject.transform.position, V3);
            RacketPieces[i].transform.rotation = Quaternion.Euler(new Vector3(0f,0f,(180f - A)));

        }

        if (Size ==  MaxSize)
        {
            IsMaxSize = true;
        }
        else
        {
            IsMaxSize = false;
        }

	
	}

    //set the racket to it's default size
    public void ResetSize()
    {
        Size =  DefaultSize;
    }

    //gets the XY corrdinates based on angle
    private Vector2 GetXYDirection( float angle, float magnitude)
    {
        angle *= -1f;
        angle -= 90f;
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * magnitude;
    }
     
    //gets the angle based on two points
    private float GetAngleDirection( Vector2 point1, Vector2 point2)
    {
        Vector2 v = point1 - point2;

        return (float)Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg; 
    }
}
