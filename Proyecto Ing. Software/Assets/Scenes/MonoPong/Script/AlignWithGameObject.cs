using UnityEngine;
using System.Collections;

public class AlignWithGameObject : MonoBehaviour {

    public GameObject Target;

	// Use this for initialization
	void Start () 
    {
        Vector2 v2 = Camera.main.WorldToScreenPoint(Target.transform.position);

        gameObject.transform.position = v2;
	
	}
	

}
