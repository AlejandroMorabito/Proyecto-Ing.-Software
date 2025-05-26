using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private string lastDirection = "Front"; // Variable para guardar la última dirección

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Capturar entrada del usuario
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("IsMoving", true);

            // Guardar la última dirección dentro del script
            if (movement.y > 0) lastDirection = "Back";
            else if (movement.y < 0) lastDirection = "Front";
            else if (movement.x > 0) lastDirection = "Right";
            else if (movement.x < 0) lastDirection = "Left";
        }
        else
        {
            Debug.Log("Estado a reproducir: " + "Idle" + lastDirection);
            animator.SetBool("IsMoving", false);
            animator.Play("Idle" + lastDirection); // Reproducir la animación correcta
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * speed;
    }
}