using UnityEngine;

public class DashHitboxBlue : MonoBehaviour
{
    // Recoil multiplier determines the strength of recoil relative to knockbackForce.
    public float recoilMultiplier = 0.2f; // 20% of knockbackForce as recoil

    // Reference to the parent's BlueDash script.
    private BlueDash blueDash;

    void Awake()
    {
        blueDash = GetComponentInParent<BlueDash>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Red")) return;

        // Get Red's rigidbody and dash component.
        Rigidbody2D redRb = other.GetComponent<Rigidbody2D>();
        if (redRb == null) return;

        RedDash redDash = other.GetComponent<RedDash>();
        if (redDash != null)
        {
            redDash.ApplyKnockBack(blueDash.dashDirection, blueDash.knockbackForce, 0.15f);
        }

        // Stop Blue's dash and apply recoil.
        blueDash.CancelDash();
        Rigidbody2D blueRb = blueDash.GetComponent<Rigidbody2D>();
        // Reset Blue's momentum.
        blueRb.linearVelocity = Vector2.zero; // Updated to use linearVelocity
                                             // Apply recoil force. Adjust recoilMultiplier as needed in the inspector.
        blueRb.AddForce(-blueDash.dashDirection * blueDash.knockbackForce * recoilMultiplier,
                         ForceMode2D.Impulse);
    }
}
