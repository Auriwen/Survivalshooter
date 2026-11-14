using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool spriteFacesRight = true;      // which way the art looks

    [Header("Health & score")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private GameObject deathEffectPrefab;      // particle burst on death

    [Header("Attack")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1.0f;     // starts attacking when this close
    [SerializeField] private float hitRange = 1.4f;        // player must still be this close when the strike lands
    [SerializeField] private float attackWindUp = 0.45f;   // time from start of the animation to the strike
    [SerializeField] private float attackRecovery = 0.35f; // time after the strike until the animation ends
    [SerializeField] private float attackCooldown = 0.6f;  // pause before the next attack can start

    private int currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private HitFlash hitFlash;
    private Transform player;
    private PlayerHealth playerHealth;

    private bool isAttacking;
    private float nextAttackTime;

    // NEW: walk to this point inside the room first, then start chasing
    private Vector2 entryPoint;
    private bool isEntering;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        hitFlash = GetComponent<HitFlash>();
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Find the player once, and remember its health script for the attack
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerHealth = p.GetComponent<PlayerHealth>();
        }
    }

    // NEW: called by the WaveSpawner right after spawning
    public void SetEntryPoint(Vector2 point)
    {
        entryPoint = point;
        isEntering = true;
    }

    void FixedUpdate()
    {
        // No player (dead) or busy attacking: stand still
        if (player == null || isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // NEW: still in the corridor? Walk straight to the entry point first
        if (isEntering)
        {
            Vector2 toEntry = entryPoint - rb.position;
            if (toEntry.magnitude < 0.1f)
            {
                isEntering = false;   // arrived inside the room: start chasing
            }
            else
            {
                FaceDirection(toEntry.x);
                rb.linearVelocity = toEntry.normalized * moveSpeed;
                return;
            }
        }

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float distance = toPlayer.magnitude;

        FaceDirection(toPlayer.x);

        // Close enough and cooldown over: attack
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
            return;
        }

        // Otherwise keep chasing
        rb.linearVelocity = toPlayer.normalized * moveSpeed;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        // 1. Start the attack animation
        if (animator != null) animator.SetTrigger("Attack");

        // 2. Wait until the strike frame appears
        yield return new WaitForSeconds(attackWindUp);

        // 3. Only hit if the player didn't step away in the meantime
        if (player != null && playerHealth != null)
        {
            float distance = Vector2.Distance(rb.position, player.position);
            if (distance <= hitRange)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        // 4. Let the animation finish, then allow chasing again
        yield return new WaitForSeconds(attackRecovery);
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
    }

    private void FaceDirection(float xDirection)
    {
        // Only flip when clearly moving sideways, so it doesn't flicker
        if (Mathf.Abs(xDirection) > 0.1f)
        {
            bool facingLeft = xDirection < 0f;
            spriteRenderer.flipX = spriteFacesRight ? facingLeft : !facingLeft;
        }
    }

    // Called by the bullet when it hits this enemy
    public void TakeDamage(int amount)
    {
        if (hitFlash != null) hitFlash.Flash();

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

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyDeath();

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);   // also stops a running attack
    }
}