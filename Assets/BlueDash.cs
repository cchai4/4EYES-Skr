using UnityEngine;

public class BlueDash : MonoBehaviour
{
    // Dash parameters
    public float dashSpeed = 7f;            // Speed during dash (e.g., 7 units)
    public float dashDuration = 0.2f;       // How long the dash lasts
    public float dashCooldown = 1f;         // Cooldown period between dashes
    public float doubleTapThreshold = 0.3f; // Maximum time between Right Shift taps to trigger a dash

    // Reference to the dash hitbox (if needed)
    public GameObject dashHitboxBlue;

    // Optional: Knockback force (if your dash hitbox will handle knockback on Red)
    public float knockbackForce = 10f;

    private Rigidbody2D rb;
    private float lastShiftTime = -1f;      // Time when Right Shift was last pressed
    private bool isDashing = false;
    private float dashTimer = 0f;
    private bool dashOnCooldown = false;
    private float cooldownTimer = 0f;

    // Use this variable to store the last recorded arrow key input.
    private Vector2 lastInputDir = Vector2.up;
    // This is the direction for the dash (set when dash is triggered)
    [HideInInspector] public Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure the dash hitbox is disabled by default.
        if (dashHitboxBlue != null)
            dashHitboxBlue.SetActive(false);
    }

    void Update()
    {
        // Continuously update the arrow key input from arrow keys.
        Vector2 currentInput = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow))
            currentInput += Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow))
            currentInput += Vector2.down;
        if (Input.GetKey(KeyCode.LeftArrow))
            currentInput += Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow))
            currentInput += Vector2.right;
        // If there is any input, update the lastInputDir.
        if (currentInput.sqrMagnitude > 0.001f)
            lastInputDir = currentInput.normalized;

        // Handle dash cooldown.
        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            {
                dashOnCooldown = false;
                cooldownTimer = 0f;
            }
        }

        // Check for a double-tap of Right Shift if not currently dashing and cooldown is over.
        if (!isDashing && !dashOnCooldown)
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                float currentTime = Time.time;
                if (currentTime - lastShiftTime <= doubleTapThreshold)
                {
                    // Set dashDirection using the last recorded arrow input direction.
                    dashDirection = lastInputDir;

                    // Start the dash.
                    isDashing = true;
                    dashTimer = 0f;
                    dashOnCooldown = true;

                    // Activate the dash hitbox if assigned.
                    if (dashHitboxBlue != null)
                        dashHitboxBlue.SetActive(true);
                }
                lastShiftTime = currentTime;
            }
        }

        // During the dash, override the velocity.
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
                // Deactivate the dash hitbox when the dash ends.
                if (dashHitboxBlue != null)
                    dashHitboxBlue.SetActive(false);
            }
        }
    }
}
