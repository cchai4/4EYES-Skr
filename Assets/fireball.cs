using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
    [Tooltip("Delay before the hitbox becomes active.")]
    public float hitboxDelay = 0.5f;
    private Collider2D fireballCollider;

    void Awake()
    {
        fireballCollider = GetComponent<Collider2D>();
        if (fireballCollider == null)
        {
            Debug.LogError("Fireball prefab is missing a Collider2D component.");
        }
    }

    IEnumerator Start()
    {
        // Disable the collider when the fireball spawns.
        if (fireballCollider != null)
        {
            fireballCollider.enabled = false;
        }

        // Wait for the designated delay.
        yield return new WaitForSeconds(hitboxDelay);

        // Enable the hitbox after the delay.
        if (fireballCollider != null)
        {
            fireballCollider.enabled = true;
        }
    }

    // Optional: Destroy the fireball after a certain time, to avoid lingering projectiles.
    // void Update()
    // {
    //     // Example: destroy after 5 seconds.
    //     Destroy(gameObject, 5f);
    // }
}
