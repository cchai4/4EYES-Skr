using UnityEngine;

public class FaceVelocityArrows : MonoBehaviour
{
    [Header("Key Assignments")]
    public PlayerControlsSO controls;

    private Rigidbody2D rb;
    private Vector2 lastDirection = Vector2.up; // start facing up

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 inputDir = Vector2.zero;
        if (Input.GetKey(controls.upKey)) inputDir.y += 1;
        if (Input.GetKey(controls.downKey)) inputDir.y -= 1;
        if (Input.GetKey(controls.leftKey)) inputDir.x -= 1;
        if (Input.GetKey(controls.rightKey)) inputDir.x += 1;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            lastDirection = inputDir.normalized;
        }

        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
