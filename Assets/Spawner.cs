using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject goldPrefab;
    public GameObject silverPrefab;

    public float spawnRadius = 0.4f;             // Radius to check for overlap
    public LayerMask resourceLayer;              // Layer where gold/silver exist
    public int maxRetries = 10;                  // Prevent infinite attempts
    public float spawnRangeXMin = -8f;
    public float spawnRangeXMax = -2f;
    public float spawnRangeYMin = -4f;
    public float spawnRangeYMax = 4f;

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
                Instantiate(spawn_obj, spawnPos, Quaternion.identity);
                return; // success, exit early
            }
        }

        Debug.LogWarning("Spawner: Could not find valid spawn position after retries.");
    }
}
