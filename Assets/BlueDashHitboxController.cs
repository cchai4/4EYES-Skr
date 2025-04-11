using UnityEngine;

public class BlueDashHitboxController : MonoBehaviour
{
    // Reference to the Red dash script. Drag your Red character (or its relevant component) into this field in the Inspector.
    public RedDashWithKnockback redDash;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is the Blue character.
        if (collision.CompareTag("Blue"))
        {
            if (redDash != null)
            {
                redDash.ApplyKnockback(collision.gameObject);
                // Optionally, you can disable the hitbox right after applying knockback to prevent multiple triggers.
                gameObject.SetActive(false);
            }
        }
    }
}
