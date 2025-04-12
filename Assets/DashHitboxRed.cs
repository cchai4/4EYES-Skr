using UnityEngine;

public class DashHitboxRed : MonoBehaviour
{
    public float recoilMultiplier = 0.2f;

    private RedDash redDash;

    void Awake() => redDash = GetComponentInParent<RedDash>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Blue")) return;

        // Get Blue's rigidbody and dash component.
        Rigidbody2D blueRb = other.GetComponent<Rigidbody2D>();
        if (blueRb == null) return;

        BlueDash blueDash = other.GetComponent<BlueDash>();
        if (blueDash != null)
        {
            blueDash.ApplyKnockBack(redDash.dashDirection, redDash.knockbackForce, 0.15f);
        }

        // Stop Red's dash and apply recoil.
        redDash.CancelDash();
        Rigidbody2D redRb = redDash.GetComponent<Rigidbody2D>();
        // Reset Red's momentum.
        redRb.linearVelocity = Vector2.zero; // Updated to use linearVelocity
                                             // Apply recoil force. Adjust recoilMultiplier as needed in the inspector.
        redRb.AddForce(-redDash.dashDirection * redDash.knockbackForce * recoilMultiplier,
                         ForceMode2D.Impulse);
    }
}
