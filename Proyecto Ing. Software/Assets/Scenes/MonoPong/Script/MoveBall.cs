/*
this script launches the ball
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MoveBall : MonoBehaviour {

    /*<summary>Ball's Launch Speed<summary>*/
	public float speed = 5;
    
    /*<summary>Previous Speed of the ball<summary>*/
	public float speedPrev = 0;
    
    /*<summary>Ball's Max Speed<summary>*/
    public float MaxSpeed = 15;

    /*<summary>Whether or not the ball should launch instantly<summary>*/
	public bool InstantLaunch = false;

    /*<summary>Particles to display after bounce<summary>*/
	public GameObject Particles1;

    /*<summary>Particles to display at Death<summary>*/
    public GameObject Particles2;
	
    /*<summary>Audio to play on collision<summary>*/
	public AudioClip CollisionAudio;

    /*<summary>Audio to play on Death<summary>*/
	public AudioClip DeathAudio;

    /*<summary>Score script to add points<summary>*/
    public ScoreScript scoreScript;
    
    /*<summary>Count of times the ball is slower than normal<summary>*/
    private int SlowDownCheckCount = 0;

    private float LastBounceTime = -100f;

	void Start() 
	{
        Invoke("LaunchBall",3f);

        scoreScript = GameObject.FindObjectOfType<ScoreScript>();
	}


    void FixedUpdate()
    {
        //get the current speed
        float CurrentSpeed = Vector2.Distance(new Vector2(0f,0f),gameObject.GetComponent<Rigidbody2D>().linearVelocity);
        
        //if the ball has slowed down increment the SlowDownCheckCount, else set it to zero.
        if (CurrentSpeed < speedPrev)
        {
            SlowDownCheckCount += 1;
        }
        else
        {
            SlowDownCheckCount = 0;
        }
        
        //if the ball has been detected as a slowdown 5 times...speed it back up
        if (SlowDownCheckCount >= 5)
        {
            float Angle = GetAngleDirection(new Vector2(0f,0f),gameObject.GetComponent<Rigidbody2D>().linearVelocity);
            Vector2 CurrentXY = GetXYDirection(Angle,speedPrev);
            
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = CurrentXY;

            SlowDownCheckCount = 0;
            
            print("Velocity Fixed");
        }

        //make sure it's not going over max speed
        if (CurrentSpeed >= MaxSpeed)
        {
            float A = GetAngleDirection(new Vector2(0f,0f),gameObject.GetComponent<Rigidbody2D>().linearVelocity);

            gameObject.GetComponent<Rigidbody2D>().linearVelocity = GetXYDirection(A,MaxSpeed);
        }
        
        //set the 
        if (SlowDownCheckCount == 0)
        {
            speedPrev = CurrentSpeed;
        }

    }

    
    //used for launching the ball
    private void LaunchBall()
    {
        GetComponent<Rigidbody2D>().linearVelocity = gameObject.transform.up * speed;
    }

    //Method to get the XY coordinates 
    private Vector2 GetXYDirection( float angle, float magnitude)
    {
        angle *= -1f;
        angle -= 90f;
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * magnitude;
    }

    //Method to get the angle between two points
    private float GetAngleDirection( Vector2 point1, Vector2 point2)
    {
        Vector2 v = point1 - point2;

        return (float)Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg; 
    }

    //method used on collision
    void OnCollisionEnter2D(Collision2D col) 
    {
        if (col.gameObject.tag == "Racket") 
        {
            if (Time.time - LastBounceTime >= 0.05f)
            {
                scoreScript.UpdateScore(1);

                Vector3 ParticlesPosition = new Vector3(col.contacts[0].point.x,col.contacts[0].point.y,0f);
                ParticlesPosition =  Vector3.Lerp(ParticlesPosition,gameObject.transform.position,0.5f);
                GameObject NewParticles = Instantiate(Particles1,ParticlesPosition,Quaternion.Euler(0f,0f,0f)) as GameObject;

                NewParticles.GetComponent<ParticleSystem>().startColor = gameObject.GetComponent<SpriteRenderer>().color;

                GameObject.Find("AudioSource").GetComponent<AudioSource>().PlayOneShot(CollisionAudio);

                LastBounceTime = Time.time;
            }

        }
        else if (col.gameObject.tag == "Border") 
        {
            Vector3 ParticlesPosition = new Vector3(col.contacts[0].point.x,col.contacts[0].point.y,0f);
            ParticlesPosition =  Vector3.Lerp(ParticlesPosition,gameObject.transform.position,0.5f);
            GameObject NewParticles = Instantiate(Particles2,ParticlesPosition,Quaternion.Euler(0f,0f,0f)) as GameObject;

            NewParticles.GetComponent<ParticleSystem>().startColor = gameObject.GetComponent<SpriteRenderer>().color;

            GameObject.Find("AudioSource").GetComponent<AudioSource>().PlayOneShot(DeathAudio);

            GameObject.FindObjectOfType<GameManager>().SetState(GameManager.GameStates.DEATH);

            Destroy(gameObject);
        }
    }
        
}
