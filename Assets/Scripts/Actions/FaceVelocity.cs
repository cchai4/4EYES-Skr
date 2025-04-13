using UnityEngine;

public class FaceLastDirection : MonoBehaviour
{
    private Rigidbody2D rb;
    // Holds the last non-zero movement direction. By default, facing up.
    public Vector2 LastDirection { get; private set; } = Vector2.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 inputDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            inputDir.y += 1;
        if (Input.GetKey(KeyCode.S))
            inputDir.y -= 1;
        if (Input.GetKey(KeyCode.A))
            inputDir.x -= 1;
        if (Input.GetKey(KeyCode.D))
            inputDir.x += 1;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            LastDirection = inputDir.normalized;
        }

        // Rotate sprite so that 0° corresponds to up (optional).
        float angle = Mathf.Atan2(LastDirection.y, LastDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
