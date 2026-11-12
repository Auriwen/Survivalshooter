using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invincibilityTime = 1f;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;

    private bool isInvincible;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        CurrentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        // Ignore hits while blinking, or if already dead
        if (isInvincible || CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        Debug.Log($"Player HP: {CurrentHealth}/{maxHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }
        if (AudioManager.Instance != null) AudioManager.Instance.PlayPlayerHurt();

        StartCoroutine(InvincibilityRoutine());
    }

    // Blink for a moment; no damage can be taken during this time
    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;  // visible <-> invisible
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    void Die()
    {
        GameManager.Instance.GameOver();
        Destroy(gameObject);
    }
}