using UnityEngine;

public class RedStun : MonoBehaviour
{
    [HideInInspector] public bool isStunned = false;
    public float stunTimer = 1f;

    // Call this method to stun Red.
    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        Debug.Log("Red is stunned for " + duration + " seconds.");

        // ADDED: Immediately zero out velocity at the moment of stun.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                Debug.Log("Red is no longer stunned.");
            }
        }
    }
}
