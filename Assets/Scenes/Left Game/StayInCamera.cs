using UnityEngine;

public class StayInCamera : MonoBehaviour
{
    private Camera cam;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        cam = Camera.main;

        // Calculate object width and height using the SpriteRenderer’s bounds
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            objectWidth = sr.bounds.extents.x;
            objectHeight = sr.bounds.extents.y;
        }
        else
        {
            objectWidth = 0.5f;
            objectHeight = 0.5f;
        }
    }

    void LateUpdate()
    {
        // Get camera boundaries in world space
        Vector3 camMin = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z - cam.transform.position.z));
        Vector3 camMax = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z - cam.transform.position.z));

        float clampedX = Mathf.Clamp(transform.position.x, camMin.x + objectWidth, camMax.x - objectWidth);
        float clampedY = Mathf.Clamp(transform.position.y, camMin.y + objectHeight, camMax.y - objectHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
