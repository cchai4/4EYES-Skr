using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject goldPrefab;
    public GameObject silverPrefab;

    public float spawnRadius = 5f;           // Radius to check for overlap
    public LayerMask resourceLayer;          // Layer where gold/silver exist
    public int maxRetries = 10;              // Prevent infinite attempts
    public float spawnRangeXMin = -8.6f;
    public float spawnRangeXMax = -2f;
    public float spawnRangeYMin = -3f;
    public float spawnRangeYMax = 5f;

    public List<GameObject> spawnedResources = new List<GameObject>();

    public void spawnResources(GameObject spawn_obj)
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
                // Debug.Log($"Spawner: Spawned object at {spawnPos}");
                return; // success, exit early
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
                Vector2 pos = resource.transform.position;
                positions.Add(pos);
            }
        }

        // Debug out the positions to verify the list.
        foreach (Vector2 p in positions)
        {
            Debug.Log("Spawned resource position: " + p);
        }

        return positions;
    }
}
