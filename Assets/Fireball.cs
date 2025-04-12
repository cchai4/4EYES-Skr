using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifeTime = 5f; // fallback timer, just in case
    public GameObject destructionEffectPrefab;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Just in case it flies forever and never hits anything
        Destroy(gameObject, lifeTime);
    }

    void OnBecameInvisible()
    {
        // Called automatically when object leaves the camera view
        PlayDestructionEffect();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Optional: check for specific tag if you want
        // if (other.CompareTag("Enemy")) { ... }

        // Play destruction animation and destroy fireball on collision
        PlayDestructionEffect();
        Destroy(gameObject);
    }

    void PlayDestructionEffect()
    {
        if (destructionEffectPrefab != null)
        {
            // Instantiate the destruction effect at the fireball's position
            GameObject effect = Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);

            // Rotate the effect to face the fireball's direction
            if (rb != null)
            {
                Vector2 direction = rb.linearVelocity.normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                effect.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}