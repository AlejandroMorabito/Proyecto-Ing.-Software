using UnityEngine;

public class Invader : MonoBehaviour
{
    public Sprite[] animationSprites;
    public float animationTime = 1.0f;
    public System.Action killed;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float minShootInterval = 2f;
    public float maxShootInterval = 5f;

    [HideInInspector]
    public Invaders invadersGroup; // Referencia al grupo de invaders

    private SpriteRenderer _spriteRenderer;
    private int _animationFrame;
    private float _nextShootTime;
    public Player player; // Referencia al jugador

    public AudioClip shootClip; // Asigna el audio en el inspector
    private AudioSource audioSource;

    private bool _isDead = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), this.animationTime, this.animationTime);
        ScheduleNextShot();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void AnimateSprite()
    {
        _animationFrame++;
        if (_animationFrame >= this.animationSprites.Length)
        {
            _animationFrame = 0;
        }

        _spriteRenderer.sprite = this.animationSprites[_animationFrame];
    }

    private void Update()
    {
        if (projectilePrefab != null && Time.time >= _nextShootTime && this.gameObject.activeInHierarchy)
        {
            if (CanShoot())
            {
                Shoot();
                ScheduleNextShot();
            }
            else
            {
                // Si no puede disparar, reintenta pronto
                _nextShootTime = Time.time + 0.5f;
            }
        }
    }

    private bool CanShoot()
    {
        if (invadersGroup == null)
            return true; // fallback: permite disparar si no hay grupo

        float epsilon = 0.1f;
        foreach (Transform child in invadersGroup.transform)
        {
            if (child == this.transform || !child.gameObject.activeInHierarchy)
                continue;

            // Misma columna (localPosition.x casi igual) y está debajo (localPosition.y menor)
            if (Mathf.Abs(child.localPosition.x - this.transform.localPosition.x) < epsilon &&
                child.localPosition.y < this.transform.localPosition.y)
            {
                return false;
            }
        }
        return true;
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        var proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.direction = Vector3.down;
        }

        // Reproducir sonido de disparo
        if (shootClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootClip);
        }
    }

    private void ScheduleNextShot()
    {
        _nextShootTime = Time.time + Random.Range(minShootInterval, maxShootInterval);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDead) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            _isDead = true;
            this.killed?.Invoke();
            this.gameObject.SetActive(false);
            return;
        }
        CheckGameOverByCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            _isDead = true;
            this.killed?.Invoke();
            this.gameObject.SetActive(false);
            return;
        }
        CheckGameOverByCollision(other.gameObject);
    }

    private void CheckGameOverByCollision(GameObject obj)
    {
        // Si choca con el jugador
        Player player = obj.GetComponent<Player>();
        if (player != null)
        {
            while (player.lives > 0)
            {
                player.LoseLife();
            }
            return;
        }

        // Si choca con un escudo
        Shield shield = obj.GetComponent<Shield>();
        if (shield != null)
        {
            Player foundPlayer = FindObjectOfType<Player>();
            if (foundPlayer != null)
            {
                while (foundPlayer.lives > 0)
                {
                    foundPlayer.LoseLife();
                }
            }
        }
    }
}
