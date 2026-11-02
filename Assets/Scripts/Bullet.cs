 using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

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

        // NEW: hit a wall: just disappear
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}