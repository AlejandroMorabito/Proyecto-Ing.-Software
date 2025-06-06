using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float initialVelocity = 4f;
    [SerializeField] private float maxVelocity = 10f; // L�mite m�ximo de velocidad
    [SerializeField] private float velocityMultiplier = 1.1f;
    [SerializeField] private float minDirectionValue = 0.2f; // Valor m�nimo para direcci�n X/Y
    private Rigidbody2D ballRb;

    void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
        Launch();
    }

    private void Launch()
    {
        float xVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        float yVelocity = Random.Range(-1f, 1f);

        // Asegurar que la direcci�n Y no sea demasiado peque�a
        if (Mathf.Abs(yVelocity) < minDirectionValue)
        {
            yVelocity = minDirectionValue * Mathf.Sign(yVelocity);
        }

        Vector2 direction = new Vector2(xVelocity, yVelocity).normalized;
        ballRb.linearVelocity = direction * initialVelocity;
    }

    private void FixedUpdate()
    {
        // Limitar la velocidad m�xima
        if (ballRb.linearVelocity.magnitude > maxVelocity)
        {
            ballRb.linearVelocity = ballRb.linearVelocity.normalized * maxVelocity;
        }

        // Prevenir movimiento completamente horizontal/vertical
        PreventStraightLines();
    }

    private void PreventStraightLines()
    {
        Vector2 velocity = ballRb.linearVelocity;

        // Si la pelota est� movi�ndose casi horizontalmente
        if (Mathf.Abs(velocity.y) < minDirectionValue)
        {
            // A�adir un peque�o componente vertical
            float newY = Mathf.Sign(Random.Range(-1f, 1f)) * minDirectionValue;
            ballRb.linearVelocity = new Vector2(velocity.x, newY).normalized * velocity.magnitude;
        }
        // Si la pelota est� movi�ndose casi verticalmente
        else if (Mathf.Abs(velocity.x) < minDirectionValue)
        {
            // A�adir un peque�o componente horizontal
            float newX = Mathf.Sign(Random.Range(-1f, 1f)) * minDirectionValue;
            ballRb.linearVelocity = new Vector2(newX, velocity.y).normalized * velocity.magnitude;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            // Aplicar multiplicador con l�mite
            Vector2 newVelocity = ballRb.linearVelocity * velocityMultiplier;
            if (newVelocity.magnitude <= maxVelocity)
            {
                ballRb.linearVelocity = newVelocity;
            }

            // Asegurar direcci�n despu�s del rebote
            PreventStraightLines();
        }
        else if (collision.gameObject.CompareTag("BackWall"))
        {
            ResetBall();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GoalZone"))
        {
            ResetBall();
        }
    }

    private void ResetBall()
    {
        transform.position = Vector2.zero;
        ballRb.linearVelocity = Vector2.zero;
        Invoke(nameof(Launch), 1f);
    }
}