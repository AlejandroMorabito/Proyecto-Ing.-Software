using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float playerSpeedMultiplier = 1.5f; // Multiplicador de velocidad para jugador
    [SerializeField] private bool isPaddle1;
    [SerializeField] private bool isAI = false;
    private float yBound = 3.75f;
    private Rigidbody2D rb;
    private Transform ball;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ball = GameObject.FindGameObjectWithTag("Ball").transform;
    }

    void Update()
    {
        float movement = 0f;

        if (isPaddle1)
        {
            // Control manual para jugador 1 con mayor velocidad
            float input = Input.GetAxisRaw("Vertical");
            movement = input * playerSpeedMultiplier;

            // Opcional: Auto-ajuste cuando la pelota se acerca
            if (BallIsApproaching())
            {
                movement *= 1.3f; // Aumenta velocidad un 30% extra
            }
        }
        else if (isAI)
        {
            movement = CalculateAIMovement();
        }
        else
        {
            movement = Input.GetAxisRaw("Vertical2");
        }

        Vector2 newPosition = transform.position;
        newPosition.y = Mathf.Clamp(newPosition.y + movement * speed * Time.deltaTime, -yBound, yBound);
        transform.position = newPosition;
    }

    private float CalculateAIMovement()
    {
        if (ball != null)
        {
            float targetY = Mathf.Clamp(ball.position.y, -yBound, yBound);
            float currentY = transform.position.y;
            return Mathf.Clamp((targetY - currentY) * 0.1f, -1f, 1f);
        }
        return 0f;
    }

    private bool BallIsApproaching()
    {
        if (ball == null) return false;

        // Detectar si la pelota viene hacia este paddle
        Vector2 ballDirection = ball.GetComponent<Rigidbody2D>().linearVelocity.normalized;
        Vector2 toPaddle = (transform.position - ball.position).normalized;

        // �ngulo entre la direcci�n de la pelota y la direcci�n hacia el paddle
        float angle = Vector2.Angle(ballDirection, toPaddle);

        // Considerar que viene hacia el paddle si el �ngulo es menor a 45 grados
        return angle < 45f && ballDirection.magnitude > 0.1f;
    }
}