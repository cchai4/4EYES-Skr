using UnityEngine;

public class BlueStun : MonoBehaviour
{
    public bool isStunned = false;
    private float stunTimer = 0f;

    // Call this method to (re)apply a stun.
    public void Stun(float duration)
    {
        stunTimer = duration; // Reset the timer every time a new stun happens.
        if (!isStunned)
        {
            isStunned = true;
            Debug.Log("Blue is stunned for " + duration + " seconds.");
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
