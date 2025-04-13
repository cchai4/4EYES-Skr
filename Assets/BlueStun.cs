using UnityEngine;

public class BlueStun : MonoBehaviour
{
    public bool isStunned = false;
    private float stunTimer = 1;

    // Call this method to (re)apply a stun.
    public void Stun(float duration)
    {
        stunTimer = duration; // Reset the timer.

        if (!isStunned)
        {
            isStunned = true;
            Debug.Log("Blue is stunned for " + duration + " seconds.");

            // ADDED: Immediately zero out velocity at the moment of stun.
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            Debug.Log("Blue stun timer reset to " + duration + " seconds.");
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
                Debug.Log("Blue is no longer stunned.");
            }
        }
    }
}
