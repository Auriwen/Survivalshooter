 using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;
        [SerializeField] private GameObject wallHitEffectPrefab;   // NEW: little flame puff on walls

    void Start()
    {
        // Fly in the direction the bullet is facing
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;

        // Delete itself after a while so bullets don't pile up
        Destroy(gameObject, lifetime);
    }

    // Runs when the bullet's trigger touches another collider
    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit an enemy: damage it and disappear
        if (other.CompareTag("Enemy") && other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

           // Hit a wall: show the flame, then disappear
        if (other.CompareTag("Wall"))
        {
            // NEW: spawn the flame where the arrow hit
            if (wallHitEffectPrefab != null)
            {
                Instantiate(wallHitEffectPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}