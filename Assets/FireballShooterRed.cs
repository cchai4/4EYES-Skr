using UnityEngine;
using System.Collections;

public class FireballShooterRed : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireballSpeed = 10f;

    [Header("Firing Controls")]
    public PlayerControlsSO controls; 

    // Cooldown handling
    public float fireballCooldown = 1f;
    private float lastFireTime = -Mathf.Infinity;

    [Header("Sound Effect")]
    public AudioClip fireballShootSfx;  
    [Range(0f, 1f)]
    public float shootVolume = 1.0f;      

    void Update()
    {
        
        if (Input.GetKey(controls.cancelKey))
        {
            if (Time.time - lastFireTime < fireballCooldown)
                return;

            int activeFireballs = GameObject.FindGameObjectsWithTag("Fireball").Length;
            if (activeFireballs >= 2)
                return;

            lastFireTime = Time.time;
            StartCoroutine(ShootFireballWithDelay());
        }
    }

    IEnumerator ShootFireballWithDelay()
    {
        yield return new WaitForSeconds(0.01f);

        
        if (fireballShootSfx != null)
        {
            AudioSource.PlayClipAtPoint(fireballShootSfx, firePoint.position, shootVolume);
        }
        else
        {
            Debug.LogWarning("FireballShooterRed: No fireball shoot sound assigned!");
        }

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        fireball.tag = "Fireball";

        
        fireball.transform.Rotate(0, 0, 90);

        SpriteRenderer fireballRenderer = fireball.GetComponent<SpriteRenderer>();
        if (fireballRenderer != null)
            fireballRenderer.sortingLayerName = "Fireball";

        Collider2D shooterCollider = GetComponent<Collider2D>();
        Collider2D fireballCollider = fireball.GetComponent<Collider2D>();
        if (shooterCollider != null && fireballCollider != null)
            Physics2D.IgnoreCollision(fireballCollider, shooterCollider);

        Vector2 direction = firePoint.up;
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * fireballSpeed;
    }
}
