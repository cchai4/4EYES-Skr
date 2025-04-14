using UnityEngine;

public class MovementWASD : MonoBehaviour
{
    public float acceleration = 10f;  // Units per second^2
    public float maxSpeed = 5f;       // Maximum overall speed
    private Rigidbody2D rb2;

    void Awake()
    {
        rb2 = GetComponent<Rigidbody2D>();
    }

    // Existing Update method for WASD player input.
    void Update()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // Gather WASD input into a single vector.
=======
=======
>>>>>>> Stashed changes
        // If the red character is stunned, skip input-based movement.
        // Allowing knockback to persist.
        if (redStun != null && redStun.isStunned)
        {
            return;
        }

        // Get movement input from keyboard.
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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

        ApplyMovement(input);
    }

    // New method to be called from your agent.
    public void MoveAgent(Vector2 input)
    {
        // Optionally, check if the agent is stunned (same as in Update)
        if (redStun != null && redStun.isStunned)
        {
            return;
        }

        // Normalize input if non-zero.
        if (input != Vector2.zero)
        {
            input = input.normalized;
        }

        ApplyMovement(input);
    }

    // Centralized movement logic to avoid code duplication.
    private void ApplyMovement(Vector2 input)
    {
        // Calculate the target velocity based on input.
        Vector2 targetVelocity = input * maxSpeed;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // Smoothly move the current velocity toward the target velocity.
=======
        // Smoothly adjust the current velocity towards the target velocity.
>>>>>>> Stashed changes
=======
        // Smoothly adjust the current velocity towards the target velocity.
>>>>>>> Stashed changes
        rb2.linearVelocity = Vector2.MoveTowards(rb2.linearVelocity, targetVelocity, acceleration * Time.deltaTime);
    }
}
