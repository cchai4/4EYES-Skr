using UnityEngine;

public class RedDashWithKnockback : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 7f;           // Speed during dash.
    public float dashDuration = 0.2f;      // How long the dash lasts.
    public float dashCooldown = 1f;        // Cooldown period between dashes.
    public float doubleTapThreshold = 0.3f;// Maximum time between space taps.
    public float knockbackForce = 10f;     // Impulse force applied to Blue.

    [Header("Dash Hitbox")]
    public GameObject dashHitbox;          // Reference to the hitbox child object.

    private Rigidbody2D rb;
    private float lastSpaceTime = -1f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private bool dashOnCooldown = false;
    private float cooldownTimer = 0f;
    [HideInInspector]
    public Vector2 dashDirection;          // Set by input; visible in inspector for debugging.

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ensure the dash hitbox is disabled at start.
        if (dashHitbox != null)
            dashHitbox.SetActive(false);
    }

    void Update()
    {
        // --- Dash Cooldown ---
        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            {
                dashOnCooldown = false;
                cooldownTimer = 0f;
            }
        }

        // --- Dash Initiation ---
        // Check for double-tap of Space.
        if (!isDashing && !dashOnCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            float currentTime = Time.time;
            if (currentTime - lastSpaceTime <= doubleTapThreshold)
            {
                // Determine dash direction from arrow key input.
                Vector2 inputDir = Vector2.zero;
                if (Input.GetKey(KeyCode.UpArrow))
                    inputDir += Vector2.up;
                if (Input.GetKey(KeyCode.DownArrow))
                    inputDir += Vector2.down;
                if (Input.GetKey(KeyCode.LeftArrow))
                    inputDir += Vector2.left;
                if (Input.GetKey(KeyCode.RightArrow))
                    inputDir += Vector2.right;

                // If there is input, normalize; otherwise default to upward.
                dashDirection = (inputDir != Vector2.zero) ? inputDir.normalized : Vector2.up;

                isDashing = true;
                dashTimer = 0f;
                dashOnCooldown = true;

                // Enable the dash hitbox.
                if (dashHitbox != null)
                    dashHitbox.SetActive(true);
            }
            lastSpaceTime = currentTime;
        }

        // --- Dash Execution ---
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
                // Disable the dash hitbox once dash is over.
                if (dashHitbox != null)
                    dashHitbox.SetActive(false);
            }
        }
    }

    // This method applies knockback to Blue.
    public void ApplyKnockback(GameObject blueObject)
    {
        Rigidbody2D blueRb = blueObject.GetComponent<Rigidbody2D>();
        if (blueRb != null)
        {
            // Apply an instantaneous impulse in the dash direction.
            blueRb.AddForce(dashDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
