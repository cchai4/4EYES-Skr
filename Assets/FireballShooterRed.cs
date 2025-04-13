using UnityEngine;
using System.Collections;

public class FireballShooterRed : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireballSpeed = 10f;

    // Added cooldown variables:
    public float fireballCooldown = 1f;  // seconds between shots
    private float lastFireTime = -Mathf.Infinity;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) // Left click to fire
        {
            // Check if sufficient cooldown has passed.
            if (Time.time - lastFireTime < fireballCooldown)
                return;

            // Count the active fireballs (they should be tagged "Fireball").
            int activeFireballs = GameObject.FindGameObjectsWithTag("Fireball").Length;
            if (activeFireballs >= 2)
                return;

            lastFireTime = Time.time;
            StartCoroutine(ShootFireballWithDelay());
        }
    }

    IEnumerator ShootFireballWithDelay()
    {
        // Wait briefly if needed (here 0.01 seconds)
        yield return new WaitForSeconds(0.01f);

        // Create the fireball at the firePoint's position and rotation
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);

        // Tag the fireball so it can be counted
        fireball.tag = "Fireball";

        // Rotate the fireball by 90 degrees (if needed).
        fireball.transform.Rotate(0, 0, 90);

        SpriteRenderer fireballRenderer = fireball.GetComponent<SpriteRenderer>();
        if (fireballRenderer != null)
        {
            fireballRenderer.sortingLayerName = "Fireball"; // Ensure this matches your layer name
        }

        Collider2D shooterCollider = GetComponent<Collider2D>();
        Collider2D fireballCollider = fireball.GetComponent<Collider2D>();
        if (shooterCollider != null && fireballCollider != null)
        {
            Physics2D.IgnoreCollision(fireballCollider, shooterCollider);
        }

        // Get the direction the character is facing. (Using firePoint.up.)
        Vector2 direction = firePoint.up;
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * fireballSpeed;
        }
    }
}