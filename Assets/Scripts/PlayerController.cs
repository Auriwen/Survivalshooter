using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Shooting")]                              // NEW
    [SerializeField] private GameObject bulletPrefab; // NEW
    [SerializeField] private Transform firePoint;     // NEW
    [SerializeField] private float fireRate = 8f;     // NEW: shots per second

    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 moveInput;
    private Vector2 mouseWorldPos;
    private float nextFireTime = 0f;                  // NEW

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
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

        // 2. Mouse position: screen pixels -> world units
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 screenPos = mouse.position.ReadValue();
            mouseWorldPos = cam.ScreenToWorldPoint(screenPos);

            // 3. NEW: hold left mouse button to shoot, limited by fire rate
            if (mouse.leftButton.isPressed && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void FixedUpdate()
    {
        // Move
        rb.linearVelocity = moveInput * moveSpeed;

        // Rotate to face the mouse
        Vector2 aimDir = mouseWorldPos - rb.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(angle);
    }

    // NEW: create a bullet at the fire point, facing the same way as the player
    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}