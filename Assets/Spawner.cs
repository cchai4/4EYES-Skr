using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    // Prefabs to spawn:
    public GameObject goldPrefab;
    public GameObject silverPrefab;

    // Spawning settings:
    public float spawnRadius = 0.4f;           // Radius to check for overlap
    public LayerMask resourceLayer;            // Layer where gold/silver exist
    public int maxRetries = 10;                // Maximum number of attempts for a valid spawn position

    // Spawning range boundaries:
    public float spawnRangeXMin = -8f;
    public float spawnRangeXMax = -2f;
    public float spawnRangeYMin = -4f;
    public float spawnRangeYMax = 4f;

    // Track spawned resources:
    public List<GameObject> spawnedResources = new List<GameObject>();

    public void SpawnResources(GameObject spawn_obj)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            float rand_x_pos = Random.Range(spawnRangeXMin, spawnRangeXMax);
            float rand_y_pos = Random.Range(spawnRangeYMin, spawnRangeYMax);
            Vector2 spawnPos = new Vector2(rand_x_pos, rand_y_pos);

            // Only spawn if position is clear (no overlap with gold/silver)
            if (!Physics2D.OverlapCircle(spawnPos, spawnRadius, resourceLayer))
            {
                GameObject newResource = Instantiate(spawn_obj, spawnPos, Quaternion.identity);
                spawnedResources.Add(newResource);
                Debug.Log($"Spawner: Spawned object at {spawnPos}");
                return; // Success: exit once an object is spawned
            }
        }

        Debug.LogWarning("Spawner: Could not find valid spawn position after retries.");
    }

    public List<Vector2> GetSpawnedResourcePositions()
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (GameObject resource in spawnedResources)
        {
            if (resource != null)
            {
                positions.Add(resource.transform.position);
            }
        }

        // Debug out the positions to verify the list
        foreach (Vector2 p in positions)
        {
            Debug.Log("Spawned resource position: " + p);
        }

        return positions;
    }
}
