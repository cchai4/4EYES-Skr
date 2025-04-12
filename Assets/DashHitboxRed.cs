using UnityEngine;

public class DashHitboxRed : MonoBehaviour
{
    public float recoilMultiplier = 0.2f;   // 20?% of knockback as recoil
    private RedDash redDash;

    void Awake() => redDash = GetComponentInParent<RedDash>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Blue")) return;

        Rigidbody2D blueRb = other.GetComponent<Rigidbody2D>();
        if (blueRb)
            blueRb.AddForce(redDash.dashDirection * redDash.knockbackForce,
                            ForceMode2D.Impulse);

        // cancel dash & apply recoil to Red
        redDash.CancelDash();
        Rigidbody2D redRb = redDash.GetComponent<Rigidbody2D>();
        if (redRb)
        {
            redRb.linearVelocity = Vector2.zero;
            redRb.AddForce(-redDash.dashDirection *
                           redDash.knockbackForce * recoilMultiplier,
                           ForceMode2D.Impulse);
        }
    }
}
