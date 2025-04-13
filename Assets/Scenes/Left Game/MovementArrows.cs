using UnityEngine;

public class MovementArrows : MonoBehaviour
{
    private BlueStun blueStun;
    public float acceleration = 10f;  // Units per second^2
    public float maxSpeed = 5f;       // Maximum overall speed
    private Rigidbody2D rb2;

    void Awake()
    {
        rb2 = GetComponent<Rigidbody2D>();
        // Cache the BlueStun component from the same GameObject.
        blueStun = GetComponent<BlueStun>();
    }

    void Update()
    {
        // Check if blueStun is assigned and if the player is stunned.
        if (blueStun != null && blueStun.isStunned)
        {
            // Optionally, stop the player's movement immediately.
            rb2.linearVelocity = Vector2.zero;
            return;
        }

        // Gather arrow key input into a single vector.
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.RightArrow))
            input.x += 1;
        if (Input.GetKey(KeyCode.LeftArrow))
            input.x -= 1;
        if (Input.GetKey(KeyCode.UpArrow))
            input.y += 1;
        if (Input.GetKey(KeyCode.DownArrow))
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
