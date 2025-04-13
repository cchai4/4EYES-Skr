using UnityEngine;

public class BlueDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 7f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float doubleTapThreshold = 0.3f;

    [Header("Knock?back Settings")]
    public float knockbackForce = 10f;

    [Header("References")]
    public GameObject dashHitboxBlue;      // Child hitbox object

    // ADDED: Reference the BlueStun script.
    private BlueStun blueStun;

    private Rigidbody2D rb;
    private float lastShiftTime = -1f;
    private bool isDashing;
    private float dashTimer;
    private bool dashOnCooldown;
    private float cooldownTimer;
    private Vector2 lastInputDir = Vector2.up;   // last arrow-key direction
    [HideInInspector] public Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (dashHitboxBlue) dashHitboxBlue.SetActive(false);

        // ADDED: Grab the BlueStun component.
        blueStun = GetComponent<BlueStun>();
    }

    void Update()
    {
        // Always allow the player to face different directions, even if stunned.
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) input += Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow)) input += Vector2.down;
        if (Input.GetKey(KeyCode.LeftArrow)) input += Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow)) input += Vector2.right;
        if (input.sqrMagnitude > 0.001f) lastInputDir = input.normalized;

        // ADDED: If stunned, skip any dash movement/cooldown logic,
        // but keep the direction updates above.
        if (blueStun != null && blueStun.isStunned)
        {
            return;
        }

        // -------------- Original dash cooldown logic --------------
        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            {
                dashOnCooldown = false;
                cooldownTimer = 0f;
            }
        }

        // -------------- Double-tap RightShift to dash --------------
        if (!isDashing && !dashOnCooldown && Input.GetKeyDown(KeyCode.RightShift))
        {
            float t = Time.time;
            if (t - lastShiftTime <= doubleTapThreshold)
            {
                dashDirection = lastInputDir;
                isDashing = true;
                dashTimer = 0f;
                dashOnCooldown = true;
                if (dashHitboxBlue) dashHitboxBlue.SetActive(true);
            }
            lastShiftTime = t;
        }

        // -------------- If currently dashing, apply velocity --------------
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
                if (dashHitboxBlue) dashHitboxBlue.SetActive(false);
            }
        }
    }

    public void CancelDash()
    {
        isDashing = false;
        if (dashHitboxBlue) dashHitboxBlue.SetActive(false);
    }
}
