using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool spriteFacesRight = true;   // NEW: which way the art looks
    [SerializeField] private GameObject deathEffectPrefab;              // NEW: effect to play when killed
    private int currentHealth;
    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;                   // NEW

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();     // NEW
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

        // NEW: face the walking direction
        // (only flip when clearly moving sideways, so it doesn't flicker
        //  when the player is almost straight above or below)
        if (Mathf.Abs(direction.x) > 0.1f)
        {
            bool movingLeft = direction.x < 0f;
            spriteRenderer.flipX = spriteFacesRight ? movingLeft : !movingLeft;
        }
    }

    // Called by the bullet when it hits this enemy
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // NEW: play the hit sound
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyHit();
        }
    }

    // Called by the WaveSpawner to make later waves faster
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    void Die()
    {
        // Give points before disappearing
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // NEW: play the death effect
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyDeath();
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
    public void SetScoreValue(int value)
    {
        scoreValue = value;
    }
}