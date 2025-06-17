using UnityEngine;

public class Shield : MonoBehaviour
{
    public int lives = 5;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateOpacity();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            // Si es del invasor, daña el shield
            if (projectile.direction == Vector3.down)
            {
                Destroy(projectile.gameObject);
                LoseLife();
            }
            // Si es del jugador, solo destruye el proyectil
            else if (projectile.direction == Vector3.up)
            {
                Destroy(projectile.gameObject);
            }
        }
    }

    private void LoseLife()
    {
        lives--;
        UpdateOpacity();

        if (lives <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void UpdateOpacity()
    {
        if (spriteRenderer != null)
        {
            float alpha = Mathf.Clamp01(lives / 3f);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}