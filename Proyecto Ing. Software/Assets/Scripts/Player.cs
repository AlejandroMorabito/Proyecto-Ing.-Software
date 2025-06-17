using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Projectile laserPrefab;
    public float speed = 5.0f;
    public int lives = 3; // Vidas del jugador
    public TMP_Text gameoverText; // Asigna el TMP_Text en el inspector
    public TMP_Text livesText; // Asigna el TMP_Text en el inspector
    public AudioClip shootClip; // Asigna el audio en el inspector

    private bool _laserActive;
    private AudioSource audioSource;

    // Agrega estas variables:
    private float minX;
    private float maxX;

    private void Start()
    {
        gameoverText.gameObject.SetActive(false);
        UpdateLivesText();

        // Calcula los límites igual que los invaders
        int columns = 11; // Debe coincidir con Invaders.cs
        float width = 2.0f * (columns - 1);
        minX = -width / 2;
        maxX = width / 2;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += Vector3.left * this.speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += Vector3.right * this.speed * Time.deltaTime;
        }

        // Limita la posición del jugador a los mismos límites que los invaders
        Vector3 pos = this.transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        this.transform.position = pos;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!_laserActive)
        {
            Projectile projectile = Instantiate(this.laserPrefab, this.transform.position, Quaternion.identity);
            projectile.destroyed += LaserDestroyed;
            _laserActive = true;

            // Reproducir sonido de disparo
            if (shootClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootClip);
            }
        }
    }

    private void LaserDestroyed()
    {
        _laserActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile != null && projectile.direction == Vector3.down)
        {
            LoseLife();
            Destroy(projectile.gameObject);
        }
    }

    public void LoseLife()
    {
        lives--;
        UpdateLivesText();
        Debug.Log("Vidas restantes: " + lives);
        StartCoroutine(Blink());
        if (lives <= 0)
        {
            Debug.Log("Game Over");
            this.gameObject.SetActive(false);
            Time.timeScale = 0f; // Detiene el juego
            gameoverText.gameObject.SetActive(true);
        }
    }

    private System.Collections.IEnumerator Blink()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int blinkCount = 6;
        float blinkDuration = 0.1f;
        for (int i = 0; i < blinkCount; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkDuration);
        }
        sr.enabled = true;
    }

    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "Vidas: " + lives;
        }
    }
}
