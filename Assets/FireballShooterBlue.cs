using UnityEngine;
using System.Collections;

public class FireballShooterBlue : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireballSpeed = 10f;

    [Header("Firing Controls")]
    public PlayerControlsSO controls; // Assign BlueControls asset here

    public float fireballCooldown = 1f;
    private float lastFireTime = -Mathf.Infinity;

    void Update()
    {
        if (Input.GetKey(controls.cancelKey)) // This should be RightShift for Blue
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
