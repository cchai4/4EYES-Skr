using UnityEngine;

public class RedDash : MonoBehaviour
{
    [Header("Key Assignments")]
    public PlayerControlsSO controls;

    [Header("Dash Settings")]
    public float dashSpeed = 7f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float doubleTapThreshold = 0.3f;

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;

    [Header("Dash Distance Settings")]
    // Minimum distance required to the target for initiating a dash (applies only if target is not Gold).
    public float minDashDistance = 2f;

    [Header("References")]
    public GameObject dashHitbox; // Child hitbox object

    // Reference to the MovementWASD script.
    public MovementWASD movementScript;

    // Reference to the RedStun script.
    private RedStun redStun;

    private Rigidbody2D rb;
    private float lastDashKeyTime = -1f;
    private bool isDashing;
    private float dashTimer;
    private bool dashOnCooldown;
    private float cooldownTimer;

    // Store last movement input direction (WASD).
    private Vector2 lastInputDir = Vector2.up;
    [HideInInspector] public Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (dashHitbox)
            dashHitbox.SetActive(false);
        redStun = GetComponent<RedStun>();
    }

    void Update()
    {
        // Fallback input (in case movementScript is not available).
        Vector2 input = Vector2.zero;
        if (Input.GetKey(controls.upKey)) input += Vector2.up;
        if (Input.GetKey(controls.downKey)) input += Vector2.down;
        if (Input.GetKey(controls.leftKey)) input += Vector2.left;
        if (Input.GetKey(controls.rightKey)) input += Vector2.right;
        if (input.sqrMagnitude > 0.001f)
            lastInputDir = input.normalized;

        // If stunned, skip dash logic.
        if (redStun && redStun.isStunned)
            return;

        // Handle cooldown timing.
        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            {
                dashOnCooldown = false;
                cooldownTimer = 0f;
            }
        }

        // Double-tap dash key logic.
        if (!isDashing && !dashOnCooldown)
        {
            float t = Time.time;
            if (t - lastDashKeyTime <= doubleTapThreshold)
            {
                // Prefer using movementScript if available.
                if (movementScript != null)
                {
                    // Check if the current target is gold.
                    if (movementScript.CurrentTargetIsGold)
                    {
                        Vector2 targetPosition = movementScript.CurrentTargetPosition;
                        Vector2 currentPosition = (Vector2)transform.position;
                        // Always dash toward the target if it is gold.
                        dashDirection = (targetPosition - currentPosition).normalized;
                        StartDash();
                    }
                    else
                    {
                        // The current target is NOT gold (e.g., it's a diamond) so do not dash.
                        // You can optionally add a debug log here if desired.
                        // Debug.Log("Dash canceled: target is a diamond.");
                    }
                }
                else
                {
                    // If no movementScript reference exists, dash using the last input direction.
                    dashDirection = lastInputDir;
                    StartDash();
                }
            }
            lastDashKeyTime = t;
        }

        // Process active dash.
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer <= dashDuration)
            {
                rb.linearVelocity = dashDirection * dashSpeed;
            }
            else
            {
                EndDash();
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = 0f;
        dashOnCooldown = true;
        if (dashHitbox)
            dashHitbox.SetActive(true);
    }

    private void EndDash()
    {
        isDashing = false;
        if (dashHitbox)
            dashHitbox.SetActive(false);
    }

    public void CancelDash()
    {
        isDashing = false;
        if (dashHitbox)
            dashHitbox.SetActive(false);
    }
}
