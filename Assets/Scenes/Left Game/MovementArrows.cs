using UnityEngine;

public class MovementArrows : MonoBehaviour
{
    [Header("Key Assignments")]
    public PlayerControlsSO controls;  // embed your new scriptable
    private BlueStun blueStun;

    public float acceleration = 10f;
    public float maxSpeed = 5f;

    private Rigidbody2D rb2;

    void Awake()
    {
        rb2 = GetComponent<Rigidbody2D>();
        blueStun = GetComponent<BlueStun>();
    }

    void Update()
    {
        // If stunned, skip input-based movement
        if (blueStun != null && blueStun.isStunned)
        {
            return;
        }

        Vector2 input = Vector2.zero;

        // Instead of Input.GetKey(KeyCode.RightArrow), we do:
        if (Input.GetKey(controls.rightKey)) input.x += 1;
        if (Input.GetKey(controls.leftKey)) input.x -= 1;
        if (Input.GetKey(controls.upKey)) input.y += 1;
        if (Input.GetKey(controls.downKey)) input.y -= 1;

        if (input != Vector2.zero)
            input = input.normalized;

        Vector2 targetVelocity = input * maxSpeed;

        rb2.linearVelocity = Vector2.MoveTowards(
            rb2.linearVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );
    }
}
