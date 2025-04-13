using UnityEngine;

public class RedDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 7f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float doubleTapThreshold = 0.3f;

    [Header("Knock?back Settings")]
    public float knockbackForce = 10f;

    [Header("References")]
    public GameObject dashHitbox; // Child hitbox object

    // ADDED: Reference the RedStun script.
    private RedStun redStun;

    private Rigidbody2D rb;
    private float lastSpaceTime = -1f;
    private bool isDashing;
    private float dashTimer;
    private bool dashOnCooldown;
    private float cooldownTimer;
    private Vector2 lastInputDir = Vector2.up; // last WASD direction
    [HideInInspector] public Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (dashHitbox) dashHitbox.SetActive(false);

        // ADDED: Grab the RedStun component.
        redStun = GetComponent<RedStun>();
    }

    void Update()
    {
        // Always allow the player to face different directions, even if stunned.
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) input += Vector2.up;
        if (Input.GetKey(KeyCode.S)) input += Vector2.down;
        if (Input.GetKey(KeyCode.A)) input += Vector2.left;
        if (Input.GetKey(KeyCode.D)) input += Vector2.right;
        if (input.sqrMagnitude > 0.001f) lastInputDir = input.normalized;

        // ADDED: If stunned, skip any dash movement/cooldown logic,
        // but keep the direction updates above.
        if (redStun != null && redStun.isStunned)
        {
            return;
        }

        // -------- Original dash cooldown logic --------
        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            {
                dashOnCooldown = false;
                cooldownTimer = 0f;
            }
        }

        // -------- Double-tap Space to dash --------
        if (!isDashing && !dashOnCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            float t = Time.time;
            if (t - lastSpaceTime <= doubleTapThreshold)
            {
                dashDirection = lastInputDir;
                isDashing = true;
                dashTimer = 0f;
                dashOnCooldown = true;
                if (dashHitbox) dashHitbox.SetActive(true);
            }
            lastSpaceTime = t;
        }

        // -------- Perform dash --------
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
