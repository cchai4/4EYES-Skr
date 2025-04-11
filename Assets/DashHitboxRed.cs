using UnityEngine;

public class DashHitboxRed : MonoBehaviour
{
    // Reference to the parent's RedDash script.
    private RedDash redDash;

    // Recoil multiplier determines the recoil impulse as a fraction of knockbackForce.
    public float recoilMultiplier = 0.2f; // 20% of knockback force for recoil

    void Awake()
    {
        redDash = GetComponentInParent<RedDash>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is the Blue character.
        if (other.CompareTag("Blue"))
        {
            Rigidbody2D blueRb = other.GetComponent<Rigidbody2D>();
            if (blueRb != null)
            {
                // Apply knockback impulse to Blue based on Red's dash direction.
                Vector2 knockbackDir = redDash.dashDirection;
                blueRb.AddForce(knockbackDir * redDash.knockbackForce, ForceMode2D.Impulse);

                // Stop all momentum of Red and apply a slight recoil backwards.
                Rigidbody2D redRb = redDash.GetComponent<Rigidbody2D>();
                if (redRb != null)
                {
                    redRb.linearVelocity = Vector2.zero;
                    float recoilForce = redDash.knockbackForce * recoilMultiplier;
                    redRb.AddForce(-redDash.dashDirection * recoilForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
