using UnityEngine;

public class RedDash : MonoBehaviour
{
    public float dashSpeed = 7f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float doubleTapThreshold = 0.3f;
    public float knockbackForce = 10f;

    public GameObject dashHitbox;

    private Rigidbody2D rb;
    private float lastSpaceTime = -Mathf.Infinity;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private bool dashOnCooldown = false;
    private float cooldownTimer = 0f;

    private Vector2 lastInputDir = Vector2.up;
    [HideInInspector] public Vector2 dashDirection;

    private float knockBackTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (dashHitbox != null)
        {
            Collider2D hitboxCollider = dashHitbox.GetComponent<Collider2D>();
            Collider2D selfCollider = GetComponent<Collider2D>();
            if (hitboxCollider != null && selfCollider != null)
            {
                Physics2D.IgnoreCollision(hitboxCollider, selfCollider, true);
            }
            dashHitbox.SetActive(false);
        }
    }


    public void ApplyKnockBack(Vector2 dir, float force, float freezeTime)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        knockBackTimer = freezeTime;
    }

    public void CancelDash()
    {
        isDashing = false;
        if (dashHitbox) dashHitbox.SetActive(false);
    }

    void Update()
    {
        /* freeze while stunned */
        RedStun st = GetComponent<RedStun>();          // BlueDash uses BlueStun
        if (st != null && st.isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        /* >>> freeze while a recoil impulse is still playing <<< */
        if (knockBackTimer > 0f)
        {
            knockBackTimer -= Time.deltaTime;
            return;              // skip writing rb.velocity this frame
        }

        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) input += Vector2.up;
        if (Input.GetKey(KeyCode.S)) input += Vector2.down;
        if (Input.GetKey(KeyCode.A)) input += Vector2.left;
        if (Input.GetKey(KeyCode.D)) input += Vector2.right;
        if (input.sqrMagnitude > 0.001f) lastInputDir = input.normalized;

        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            { dashOnCooldown = false; cooldownTimer = 0f; }
        }

        if (!isDashing && !dashOnCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastSpaceTime <= doubleTapThreshold)
            {
                dashDirection = lastInputDir;
                isDashing = true;
                dashTimer = 0f;
                dashOnCooldown = true;
                if (dashHitbox) dashHitbox.SetActive(true);
            }
            lastSpaceTime = Time.time;
        }

        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer <= dashDuration)
                rb.linearVelocity = dashDirection * dashSpeed;
            else
            {
                isDashing = false;
                if (dashHitbox) dashHitbox.SetActive(false);
            }
        }
    }
}
