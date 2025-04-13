using UnityEngine;

public class FlagBuilding : BuildingBase
{
    protected override void Awake()
    {
        // Call the base Awake to initialize BuildingBase variables.
        base.Awake();
        Debug.Log("FlagBuilding: Awake called.");

        // Example: Ensure that a SpriteRenderer is present.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("FlagBuilding: SpriteRenderer is missing on flag prefab!");
        }
        else
        {
            // Optionally, set properties such as sorting order.
            sr.sortingOrder = 5; // Adjust as needed to ensure visibility.
        }

        // Add any other initialization code here with proper null checks.
        // For example, if you need to reference an AudioSource:
        // AudioSource aud = GetComponent<AudioSource>();
        // if (aud == null)
        //     Debug.LogWarning("FlagBuilding: AudioSource is missing on flag prefab!");
    }
}
