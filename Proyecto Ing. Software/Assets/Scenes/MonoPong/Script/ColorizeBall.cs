/*
This Script changes the Ball color after each collision
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorizeBall : MonoBehaviour {

	public List<Color> Colors = new List<Color>();
	public int ColorIndex = 0;
	public float ColorSpeed;

    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;

	// Use this for initialization
	void Start () 
    {
		ColorIndex = Random.Range(0,Colors.Count-1);

        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = gameObject.transform.Find("Trail").GetComponent<TrailRenderer>();

        spriteRenderer.color = Colors[ColorIndex];
        
        if (trailRenderer != null)
        {
            trailRenderer.material.color = Colors[ColorIndex];
        }
            
	}
	
	// Update is called once per frame
	void Update()
	{
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, Colors[ColorIndex], Time.time * ColorSpeed);
		
        if (trailRenderer != null)
        {
            trailRenderer.material.color = Color.Lerp(trailRenderer.material.color, new Color(Colors[ColorIndex].r,Colors[ColorIndex].g,Colors[ColorIndex].b,0.5f), Time.time * ColorSpeed);
        }
            
    }
    
    
    void OnCollisionEnter2D(Collision2D col) 
	{
        ColorIndex += 1;
		ColorIndex = ColorIndex % Colors.Count;
    }    
    
}
