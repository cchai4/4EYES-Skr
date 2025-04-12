using UnityEngine;

public class FaceLastDirection : MonoBehaviour
{
    private Rigidbody2D rb;
    // This holds the last input direction (or "intended" direction) when movement keys were pressed.
    private Vector2 lastDirection = Vector2.up; // start facing up

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from keyboard (WASD)
        Vector2 inputDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            inputDir.y += 1;
        if (Input.GetKey(KeyCode.S))
            inputDir.y -= 1;
        if (Input.GetKey(KeyCode.A))
            inputDir.x -= 1;
        if (Input.GetKey(KeyCode.D))
            inputDir.x += 1;

        // If there is active input, update the stored direction.
        if (inputDir.sqrMagnitude > 0.01f)
        {
            lastDirection = inputDir.normalized;
        }
        // Otherwise, keep using the lastDirection even if rb.velocity is decaying or noisy.

        // Calculate the angle.
        // Atan2 returns an angle with 0° along the positive x-axis.
        // Subtract 90° so that a 0° angle means facing upward (the sprite's original direction).
        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
