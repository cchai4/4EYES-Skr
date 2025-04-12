using UnityEngine;

public class FireScriptBlue : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;          // Reference to your fireball prefab.
    public Transform fireballSpawnPoint;       // Spawn point Transform (e.g., a child object of the blue character).
    public float fireballSpeed = 10f;          // Speed at which the fireball travels.
    public float fireballCoolDownRed = 1f;     // Cooldown time between shots.

    [Header("Blue Character Direction")]
    public FaceVelocityArrows faceDirection;   // Reference to the directional tracking script (adjusted name if needed).

    private float fireballCoolDownRedTimer = 0f;

    void Update()
    {
        // Count down the cooldown timer.
        if (fireballCoolDownRedTimer > 0f)
        {
            fireballCoolDownRedTimer -= Time.deltaTime;
        }

        // If Right Control is pressed and cooldown is finished, fire a projectile.
        if (Input.GetKeyDown(KeyCode.RightControl) && fireballCoolDownRedTimer <= 0f)
        {
            // You might consider renaming this method to FireBlueFireball for clarity.
            FireRedFireball();
            fireballCoolDownRedTimer = fireballCoolDownRed;
        }
    }

    void FireRedFireball() // Consider renaming to FireBlueFireball.
    {
        // Check that all needed references are provided.
        if (fireballPrefab == null || fireballSpawnPoint == null || faceDirection == null)
        {
            Debug.LogError("Missing fireball prefab, spawn point, or faceDirection reference.");
            return;
        }

        // Instantiate the fireball at the designated spawn point.
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

        // Set its velocity based on the character's current facing direction.
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 shootDirection = faceDirection.LastDirection.normalized;
            // Set the velocity property rather than linearVelocity.
            rb.linearVelocity = shootDirection * fireballSpeed;
        }
        else
        {
            Debug.LogError("Fireball prefab must have a Rigidbody2D component.");
        }
    }
}
