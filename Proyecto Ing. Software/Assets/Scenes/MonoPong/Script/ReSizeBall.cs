using UnityEngine;
using System.Collections;

public class ReSizeBall : MonoBehaviour {


    /*<summary>Ball's Launch Speed<summary>*/
    public float speed = 2.5f;

    /*<summary>how much the ball will chnage the size of the racket<summary>*/
    public float DeltaSizeChange = -2f;

    /*<summary>Particles to display after collision with Racket<summary>*/
    public GameObject Particles0;

    /*<summary>Particles to display after collision with Racket<summary>*/
    public GameObject Particles1;

    /*<summary>Audio to play on collision<summary>*/
    public AudioClip CollisionAudio;

    //the time the ball was created
    private float CreatedTime;

    //number of seconds the ball should exist for before automatically destorying itself
    public float LifeTime = 30f;

    void Start()
    {
        CreatedTime = Time.time;

        Launch();
    }

    //destory itself if over lifetime
    void FixedUpdate()
    {
        if (Time.time - CreatedTime >= LifeTime)
        {
            Destroy(gameObject);
        }
    }

	// Use launch the ball
	public void Launch() 
    {
        GameObject NewParticles = Instantiate(Particles0,gameObject.transform.position,Quaternion.Euler(0f,0f,0f)) as GameObject;
        NewParticles.GetComponent<ParticleSystem>().startColor = gameObject.GetComponent<SpriteRenderer>().color;

        gameObject.transform.rotation = Quaternion.Euler(0f,0f,Random.Range(0f,360f));
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = gameObject.transform.up * speed;
	}

    //method used on collision
    void OnCollisionEnter2D(Collision2D col) 
    {
        if (col.gameObject.tag == "Racket") 
        {

            RacketController RC = GameObject.FindObjectOfType<RacketController>();
            RC.Size += DeltaSizeChange;

            Vector3 ParticlesPosition = new Vector3(col.contacts[0].point.x,col.contacts[0].point.y,0f);
            ParticlesPosition =  Vector3.Lerp(ParticlesPosition,gameObject.transform.position,0.5f);
            GameObject NewParticles = Instantiate(Particles1,ParticlesPosition,Quaternion.Euler(0f,0f,0f)) as GameObject;

            NewParticles.GetComponent<ParticleSystem>().startColor = gameObject.GetComponent<SpriteRenderer>().color;

            GameObject.Find("AudioSource").GetComponent<AudioSource>().PlayOneShot(CollisionAudio);

            Destroy(gameObject);
        }
    }
        
    //destroys the object and produces particles
    public void RemoveMe()
    {
        GameObject NewParticles = Instantiate(Particles1,gameObject.transform.position,Quaternion.Euler(0f,0f,0f)) as GameObject;
        NewParticles.GetComponent<ParticleSystem>().startColor = gameObject.GetComponent<SpriteRenderer>().color;
        Destroy(gameObject);
    }
}
