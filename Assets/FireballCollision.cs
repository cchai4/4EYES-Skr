using UnityEngine;
using System.Collections;

public class FireballCollision : MonoBehaviour
{
    public float knockbackForce = 10f; // Impulse force to apply upon collision

    private Rigidbody2D rb;
    private Animator animator;
    private bool hasExploded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded)
            return; // Prevent multiple triggers

        hasExploded = true;

        // If the fireball hits Blue, apply stun and knockback.
        if (other.CompareTag("Blue"))
        {
            BlueStun blueStun = other.GetComponent<BlueStun>();
            if (blueStun != null)
            {
                blueStun.Stun(1f);

                Rigidbody2D blueRb = other.GetComponent<Rigidbody2D>();
                if (blueRb != null)
                {
                    // Use the fireball's current velocity (normalized) as the knockback direction.
                    Vector2 knockbackDir = rb.linearVelocity.normalized;
                    blueRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
        // If the fireball hits Red, do the same.
        else if (other.CompareTag("Red"))
        {
            RedStun redStun = other.GetComponent<RedStun>();
            if (redStun != null)
            {
                redStun.Stun(1f);

                Rigidbody2D redRb = other.GetComponent<Rigidbody2D>();
                if (redRb != null)
                {
                    Vector2 knockbackDir = rb.linearVelocity.normalized;
                    redRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }

        // Stop the fireball’s movement.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            // Optional: prevents further physics interactions.
        }

        // Play the explosion animation.
        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }

        // Do not disable the collider on the player. We are not disabling our own collider here,
        // so that the explosion logic and knockback are registered properly.
    }

    // Called via an Animation Event at the end of the explosion animation.
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}