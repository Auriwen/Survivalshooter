using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool spriteFacesRight = true;      // which way the art looks
    [SerializeField] private GameObject deathEffectPrefab;      // particle burst on death

    private int currentHealth;
    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private HitFlash hitFlash;                                  // NEW: white flash when hit

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitFlash = GetComponent<HitFlash>();                    // NEW
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Find the object tagged "Player" once at the start
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void FixedUpdate()
    {
        // If there's no player (e.g. it died), stand still
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Move toward the player
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Face the walking direction (only when clearly moving sideways, to avoid flicker)
        if (Mathf.Abs(direction.x) > 0.1f)
        {
            bool movingLeft = direction.x < 0f;
            spriteRenderer.flipX = spriteFacesRight ? movingLeft : !movingLeft;
        }
    }

    // Called by the bullet when it hits this enemy
    public void TakeDamage(int amount)
    {
        // NEW: flash white
        if (hitFlash != null)
        {
            hitFlash.Flash();
        }
        else
        {
            Debug.LogWarning("Enemy has no HitFlash component!");   // tells you if it's missing
        }

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyHit();
        }
    }

    // Called by the WaveSpawner to make later waves faster
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    // Called by the WaveSpawner to give later waves more points
    public void SetScoreValue(int value)
    {
        scoreValue = value;
    }

    void Die()
    {
        // Give points before disappearing
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyDeath();

        // Particle burst where the enemy was
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // Runs every physics step while touching something
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(contactDamage);
        }
    }
}