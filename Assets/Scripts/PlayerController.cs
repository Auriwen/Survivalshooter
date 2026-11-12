using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Aiming")]                                   // NEW
    [SerializeField] private Transform aimPivot;         // NEW: rotates toward the mouse
    [SerializeField] private bool spriteFacesRight = true; // NEW: which way the art looks

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 8f;

    private Rigidbody2D rb;
    private Camera cam;
    private SpriteRenderer spriteRenderer;               // NEW
    private Vector2 moveInput;
    private Vector2 mouseWorldPos;
    private float nextFireTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>(); // NEW
    }

    void Update()
    {
        // 1. Keyboard -> movement direction
        Vector2 input = Vector2.zero;
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    input.y += 1;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  input.y -= 1;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  input.x -= 1;
        }
        moveInput = input.normalized;

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            // 2. Mouse position: screen pixels -> world units
            Vector2 screenPos = mouse.position.ReadValue();
            mouseWorldPos = cam.ScreenToWorldPoint(screenPos);

            // 3. NEW: aim the pivot and flip the sprite (before shooting,
            //    so the bullet uses this frame's aim)
            Aim();

            // 4. Shoot while holding the left mouse button
            if (mouse.leftButton.isPressed && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void FixedUpdate()
    {
        // CHANGED: only movement here; the body no longer rotates
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // NEW
    void Aim()
    {
        Vector2 aimDir = mouseWorldPos - (Vector2)transform.position;

        // Rotate only the pivot (same angle math as before)
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
        aimPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip the body to face the mouse's side
        bool mouseIsLeft = aimDir.x < 0f;
        spriteRenderer.flipX = spriteFacesRight ? mouseIsLeft : !mouseIsLeft;
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
           // NEW: arrow sound
        if (AudioManager.Instance != null) AudioManager.Instance.PlayShoot();
    }
}