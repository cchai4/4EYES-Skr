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

    [Header("References")]
    public GameObject dashHitbox; // Child hitbox object

    // Reference the RedStun script.
    private RedStun redStun;

    private Rigidbody2D rb;
    private float lastDashKeyTime = -1f; // was lastSpaceTime
    private bool isDashing;
    private float dashTimer;
    private bool dashOnCooldown;
    private float cooldownTimer;

    // Store last movement input direction (WASD)
    private Vector2 lastInputDir = Vector2.up;
    [HideInInspector] public Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (dashHitbox) dashHitbox.SetActive(false);

        redStun = GetComponent<RedStun>();
    }

    void Update()
    {
        // Read directional input from controls
        Vector2 input = Vector2.zero;
        if (Input.GetKey(controls.upKey)) input += Vector2.up;
        if (Input.GetKey(controls.downKey)) input += Vector2.down;
        if (Input.GetKey(controls.leftKey)) input += Vector2.left;
        if (Input.GetKey(controls.rightKey)) input += Vector2.right;
        if (input.sqrMagnitude > 0.001f)
            lastInputDir = input.normalized;

        // If stunned, skip dash logic
        if (redStun && redStun.isStunned)
            return;

        // If on cooldown, track time
        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            {
                dashOnCooldown = false;
                cooldownTimer = 0f;
            }
        }

        // Double-tap dash key
        if (!isDashing && !dashOnCooldown && Input.GetKeyDown(controls.dashDoubleTapKey))
        {
            float t = Time.time;
            if (t - lastDashKeyTime <= doubleTapThreshold)
            {
                dashDirection = lastInputDir;
                isDashing = true;
                dashTimer = 0f;
                dashOnCooldown = true;
                if (dashHitbox) dashHitbox.SetActive(true);
            }
            lastDashKeyTime = t;
        }

        // If currently dashing, apply velocity
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer <= dashDuration)
            {
                rb.linearVelocity = dashDirection * dashSpeed;
            }
            else
            {
                isDashing = false;
                if (dashHitbox) dashHitbox.SetActive(false);
            }
        }
    }

    public void CancelDash()
    {
        isDashing = false;
        if (dashHitbox) dashHitbox.SetActive(false);
    }
}
