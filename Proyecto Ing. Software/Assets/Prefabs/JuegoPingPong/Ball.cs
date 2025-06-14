using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float initialVelocity = 4f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float velocityMultiplier = 1.1f;
    [SerializeField] private float minDirectionValue = 0.2f;
    private Rigidbody2D ballRb;

    private Game_Manager gameManager;

    void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<Game_Manager>();
        Launch();
    }

    private void Launch()
    {
        float xVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        float yVelocity = Random.Range(-1f, 1f);

        if (Mathf.Abs(yVelocity) < minDirectionValue)
        {
            yVelocity = minDirectionValue * Mathf.Sign(yVelocity == 0 ? 1 : yVelocity);
        }

        Vector2 direction = new Vector2(xVelocity, yVelocity).normalized;
        ballRb.linearVelocity = direction * initialVelocity;
    }

    private void FixedUpdate()
    {
        if (ballRb.linearVelocity.magnitude > maxVelocity)
        {
            ballRb.linearVelocity = ballRb.linearVelocity.normalized * maxVelocity;
        }
        PreventStraightLines();
    }

    private void PreventStraightLines()
    {
        Vector2 velocity = ballRb.linearVelocity;

        if (Mathf.Abs(velocity.y) < minDirectionValue)
        {
            float newY = Mathf.Sign(Random.Range(-1f, 1f)) * minDirectionValue;
            ballRb.linearVelocity = new Vector2(velocity.x, newY).normalized * velocity.magnitude;
        }
        else if (Mathf.Abs(velocity.x) < minDirectionValue)
        {
            float newX = Mathf.Sign(Random.Range(-1f, 1f)) * minDirectionValue;
            ballRb.linearVelocity = new Vector2(newX, velocity.y).normalized * velocity.magnitude;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector2 newVelocity = ballRb.linearVelocity * velocityMultiplier;
            if (newVelocity.magnitude <= maxVelocity)
            {
                ballRb.linearVelocity = newVelocity;
            }
            PreventStraightLines();
        }
        else if (collision.gameObject.CompareTag("BackWall"))
        {
            ResetBall();
            gameManager.AddPointToPlayer(1); // Suma punto al jugador 1
        }
        else if (collision.gameObject.CompareTag("BackWall2"))
        {
            ResetBall();
            gameManager.AddPointToPlayer(2); // Suma punto al jugador 2
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detecta zona de gol para cada jugador
        if (other.CompareTag("GoalZone1"))
        {
            if (gameManager != null)
                gameManager.AddPointToPlayer(1); // Suma punto al jugador 1
            ResetBall();
        }
        else if (other.CompareTag("GoalZone2"))
        {
            if (gameManager != null)
                gameManager.AddPointToPlayer(2); // Suma punto al jugador 2
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