using UnityEngine;

public class MovementWASD : MonoBehaviour
{
    // Reference to the RedStun component.
    private RedStun redStun;
    public float acceleration = 10f;  // Units per second^2
    public float maxSpeed = 5f;       // Maximum overall speed
    private Rigidbody2D rb2;

    void Awake()
    {
        rb2 = GetComponent<Rigidbody2D>();
        // Cache the RedStun component from the same GameObject.
        redStun = GetComponent<RedStun>();
    }

    void Update()
    {
        // If the red character is stunned, stop movement immediately.
        if (redStun != null && redStun.isStunned)
        {
            rb2.linearVelocity = Vector2.zero;
            return;
        }

        // Gather WASD input into a single vector.
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.D))
            input.x += 1;
        if (Input.GetKey(KeyCode.A))
            input.x -= 1;
        if (Input.GetKey(KeyCode.W))
            input.y += 1;
        if (Input.GetKey(KeyCode.S))
            input.y -= 1;

        // Normalize the input so that diagonal movement isn't faster.
        if (input != Vector2.zero)
            input = input.normalized;

        // Calculate the target velocity based on input.
        Vector2 targetVelocity = input * maxSpeed;

        // Smoothly move the current velocity toward the target velocity.
        rb2.linearVelocity = Vector2.MoveTowards(rb2.linearVelocity, targetVelocity, acceleration * Time.deltaTime);
    }
}
