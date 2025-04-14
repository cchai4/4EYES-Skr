using UnityEngine;

public class BlueDash : MonoBehaviour
{
    [Header("Key Assignments")]
    public PlayerControlsSO controls; // This will store dashDoubleTapKey, etc.

    [Header("Dash Settings")]
    public float dashSpeed = 7f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float doubleTapThreshold = 0.3f;

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;

    [Header("References")]
    public GameObject dashHitboxBlue; // Child hitbox object

    private BlueStun blueStun;
    private Rigidbody2D rb;

    private float lastShiftTime = -1f;
    private bool isDashing;
    private float dashTimer;
    private bool dashOnCooldown;
    private float cooldownTimer;

    private Vector2 lastInputDir = Vector2.up; // last arrow-key direction
    [HideInInspector] public Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (dashHitboxBlue) dashHitboxBlue.SetActive(false);

        blueStun = GetComponent<BlueStun>();
    }

    void Update()
    {
        // always allow turning
        Vector2 input = Vector2.zero;
        if (Input.GetKey(controls.upKey)) input += Vector2.up;
        if (Input.GetKey(controls.downKey)) input += Vector2.down;
        if (Input.GetKey(controls.leftKey)) input += Vector2.left;
        if (Input.GetKey(controls.rightKey)) input += Vector2.right;

        if (input.sqrMagnitude > 0.001f)
            lastInputDir = input.normalized;

        // skip dash logic if stunned
        if (blueStun && blueStun.isStunned)
            return;

        // dash cooldown
        if (dashOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= dashCooldown)
            {
                dashOnCooldown = false;
                cooldownTimer = 0f;
            }
        }

        // double-tap dash
        if (!isDashing && !dashOnCooldown && Input.GetKeyDown(controls.dashDoubleTapKey))
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

        // perform dash
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
