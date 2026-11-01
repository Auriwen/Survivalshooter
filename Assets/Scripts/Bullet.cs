using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;

    void Start()
    {
        // Fly in the direction the bullet is facing ("up" = the triangle's tip direction)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;

        // Delete itself after 2 seconds so bullets don't pile up forever
        Destroy(gameObject, lifetime);
    }
}