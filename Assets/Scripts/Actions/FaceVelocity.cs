using UnityEngine;

public class FaceLastDirection : MonoBehaviour
{
    // Reference to MovementWASD script to obtain target velocity.
    public MovementWASD movementScript;

    // Fallback if movementScript is not provided.
    private Vector2 lastDirection = Vector2.up;

    void Update()
    {
        if (movementScript != null)
        {
            Vector2 targetVelocity = movementScript.CurrentTargetVelocity;
            // Use the target velocity's normalized direction if significant.
            if (targetVelocity.sqrMagnitude > 0.01f)
            {
                lastDirection = targetVelocity.normalized;
            }
        }

        // Calculate angle to face; subtract 90 degrees if your sprite faces up by default.
        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Debug log to see the current target velocity and calculated angle.
        // Debug.Log($"Target Velocity: {movementScript?.CurrentTargetVelocity}, Facing direction: {lastDirection}, Angle: {angle}");
    }
}
