using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public AudioClip footstepSound; // Asigna el sonido de pasos en el Inspector
    public float footstepDelay = 0.3f; // Tiempo entre pasos
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private string lastDirection = "Front";
    private AudioSource audioSource;
    private float footstepTimer = 0f;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        this.enabled = true;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        bool wasMoving = isMoving;
        isMoving = movement != Vector2.zero;

        if (isMoving)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("IsMoving", true);

            // Actualizar última dirección
            if (movement.y > 0) lastDirection = "Back";
            else if (movement.y < 0) lastDirection = "Front";
            else if (movement.x > 0) lastDirection = "Right";
            else if (movement.x < 0) lastDirection = "Left";

            // Controlar sonido de pasos
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstepSound();
                footstepTimer = footstepDelay;
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.Play("Idle" + lastDirection);
            
            // Resetear el temporizador cuando se detiene
            if (wasMoving && !isMoving)
            {
                footstepTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * speed;
    }

    private void PlayFootstepSound()
    {
        if (footstepSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(footstepSound);
        }
    }
}