using UnityEngine;

public class DashHitboxBlue : MonoBehaviour
{
    // Reference to the parent's BlueDash script.
    private BlueDash blueDash;

    // Recoil multiplier determines how strong the backward impulse is relative to knockbackForce.
    public float recoilMultiplier = 0.2f; // 20% of knockback force as recoil

    void Awake()
    {
        blueDash = GetComponentInParent<BlueDash>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is the Red character.
        if (collision.CompareTag("Red"))
        {
            Rigidbody2D redRb = collision.GetComponent<Rigidbody2D>();
            if (redRb != null)
            {
                // Apply knockback to Red based on Blue's dash direction.
                Vector2 knockbackDir = blueDash.dashDirection;
                redRb.AddForce(knockbackDir * blueDash.knockbackForce, ForceMode2D.Impulse);

                // Now stop Blue's momentum and apply a slight recoil in the opposite direction.
                Rigidbody2D blueRb = blueDash.GetComponent<Rigidbody2D>();
                if (blueRb != null)
                {
                    blueRb.linearVelocity = Vector2.zero;
                    float recoilForce = blueDash.knockbackForce * recoilMultiplier;
                    blueRb.AddForce(-blueDash.dashDirection * recoilForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
