using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Space = a short, fast dash in the direction the player is moving
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;

    // Other scripts check this (movement pauses, health ignores damage)
    public bool IsDashing { get; private set; }

    private Rigidbody2D rb;
    private Vector2 lastMoveDirection = Vector2.right;   // used if you dash while standing still
    private Vector2 dashDirection;
    private float nextDashTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (PauseMenu.IsPaused || Keyboard.current == null) return;

        // Remember the last direction the player moved in
        Vector2 input = ReadMoveInput();
        if (input != Vector2.zero) lastMoveDirection = input;

        // Space starts a dash, if not already dashing and the cooldown is over
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !IsDashing && Time.time >= nextDashTime)
        {
            StartCoroutine(DashRoutine());
        }
    }

    void FixedUpdate()
    {
        // During the dash, keep the speed fixed
        if (IsDashing) rb.linearVelocity = dashDirection * dashSpeed;
    }

    private IEnumerator DashRoutine()
    {
        IsDashing = true;
        dashDirection = lastMoveDirection;
        nextDashTime = Time.time + dashCooldown;

        yield return new WaitForSeconds(dashDuration);

        IsDashing = false;
        rb.linearVelocity = Vector2.zero;   // PlayerController takes over again next frame
    }

    private Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        return input.normalized;   // so diagonal dashes aren't longer
    }
}