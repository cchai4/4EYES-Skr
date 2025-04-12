using UnityEngine;

public class fireScriptRed : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;          // Reference to your fireball prefab.
    public Transform fireballSpawnPoint;       // Spawn point Transform (e.g., a child object of the red character).
    public float fireballSpeed = 10f;          // Speed at which the fireball travels.
    public float fireballCoolDownRed = 1f;     // Cooldown time between shots.

    [Header("Red Character Direction")]
    public FaceLastDirection faceDirection;    // Reference to the red character's directional tracking script.

    private float fireballCoolDownRedTimer = 0f;

    void Update()
    {
        // Count down the cooldown timer.
        if (fireballCoolDownRedTimer > 0f)
        {
            fireballCoolDownRedTimer -= Time.deltaTime;
        }

        // If Left Shift is pressed and cooldown is finished, fire a projectile.
        if (Input.GetKeyDown(KeyCode.LeftShift) && fireballCoolDownRedTimer <= 0f)
        {
            FireRedFireball();
            fireballCoolDownRedTimer = fireballCoolDownRed;
        }
    }

    void FireRedFireball()
    {
        // Check that all needed references are provided.
        if (fireballPrefab == null || fireballSpawnPoint == null || faceDirection == null)
        {
            Debug.LogError("Missing fireball prefab, spawn point, or faceDirection reference.");
            return;
        }

        // Instantiate the fireball at the designated spawn point.
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

        // Set its velocity based on the red character's current facing direction.
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 shootDirection = faceDirection.LastDirection.normalized;
            rb.linearVelocity = shootDirection * fireballSpeed;
        }
        else
        {
            Debug.LogError("Fireball prefab must have a Rigidbody2D component.");
        }
    }
}
