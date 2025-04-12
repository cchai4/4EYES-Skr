using UnityEngine;

public class RedStun : MonoBehaviour
{
    // Option 1: If you disable an existing movement script:
    // public MonoBehaviour movementScript;

    // Option 2: If you want to just disable movement here, you can use a flag:
    [HideInInspector] public bool isStunned = false;
    private float stunTimer = 0f;

    // Call this method to stun Blue.
    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        // If you use an external movement script, disable it:
        // if(movementScript != null)
        //     movementScript.enabled = false;
    }

    void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                // Re-enable movement if you disabled an external script:
                // if(movementScript != null)
                //     movementScript.enabled = true;
            }
        }
    }
}
