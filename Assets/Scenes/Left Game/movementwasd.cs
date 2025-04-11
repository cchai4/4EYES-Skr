using UnityEngine;

public class movementwasd : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb2;

    void Update()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move.y += 1;
        if (Input.GetKey(KeyCode.S))
            move.y -= 1;
        if (Input.GetKey(KeyCode.A))
            move.x -= 1;
        if (Input.GetKey(KeyCode.D))
            move.x += 1;
        if (move != Vector3.zero)
            move.Normalize();

        transform.position += move * speed * Time.deltaTime;
    }
}
