using UnityEngine;

public class BlueDash : MonoBehaviour
{
    // Dash parameters
    public float dashSpeed = 7f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float doubleTapThreshold = 0.3f;
    public float knockbackForce = 10f;

    public GameObject dashHitboxBlue;

    private Rigidbody2D rb;
    private float lastShiftTime = -Mathf.Infinity;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private bool dashOnCooldown = false;
    private float cooldownTimer = 0f;

    private Vector2 lastInputDir = Vector2.up;   // updated from arrow keys
    [HideInInspector] public Vector2 dashDirection;

    /* new */
    private float knockBackTimer = 0f; // freezes movement during recoil

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (dashHitboxBlue != null)
        {
            // Ensure the dash hitbox does not collide with Red's own collider.
            Collider2D hitboxCollider = dashHitboxBlue.GetComponent<Collider2D>();
            Collider2D selfCollider = GetComponent<Collider2D>();
            if (hitboxCollider != null && selfCollider != null)
            {
                Physics2D.IgnoreCollision(hitboxCollider, selfCollider, true);
            }
            dashHitboxBlue.SetActive(false);
        }
    }


    // Called by DashHitboxRed when Blue is knocked back
    public void ApplyKnockBack(Vector2 dir, float force, float freezeTime)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        knockBackTimer = freezeTime;
    }

    void Update()
    {
        /* freeze while stunned */
        BlueStun st = GetComponent<BlueStun>();
        // BlueDash uses BlueStun
        if (st != null && st.isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (knockBackTimer > 0f)
        {
            knockBackTimer -= Time.deltaTime;
            return;              // skip writing rb.velocity this frame
        }

        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) input += Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow)) input += Vector2.down;
        if (Input.GetKey(KeyCode.LeftArrow)) input += Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow)) input += Vector2.right;
        if (input.sqrMagnitude > 0.001f) lastInputDir = input.normalized;

        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            { dashOnCooldown = false; cooldownTimer = 0f; }
        }

        if (!isDashing && !dashOnCooldown && Input.GetKeyDown(KeyCode.RightShift))
        {
            if (Time.time - lastShiftTime <= doubleTapThreshold)
            {
                dashDirection = lastInputDir;
                isDashing = true;
                dashTimer = 0f;
                dashOnCooldown = true;
                if (dashHitboxBlue) dashHitboxBlue.SetActive(true);
            }
            lastShiftTime = Time.time;
        }

        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer <= dashDuration)
                rb.linearVelocity = dashDirection * dashSpeed;
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
