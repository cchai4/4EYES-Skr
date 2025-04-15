using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject goldPrefab;
    public GameObject silverPrefab;

    public float spawnRadius = 5f;        
    public LayerMask resourceLayer;         
    public int maxRetries = 10;                  
    public float spawnRangeXMin = -8.6f;
    public float spawnRangeXMax = -2f;
    public float spawnRangeYMin = -3f;
    public float spawnRangeYMax = 5f;

    public void spawnResources(GameObject spawn_obj)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            float rand_x_pos = Random.Range(spawnRangeXMin, spawnRangeXMax);
            float rand_y_pos = Random.Range(spawnRangeYMin, spawnRangeYMax);
            Vector2 spawnPos = new Vector2(rand_x_pos, rand_y_pos);

            if (!Physics2D.OverlapCircle(spawnPos, spawnRadius, resourceLayer))
            {
                Instantiate(spawn_obj, spawnPos, Quaternion.identity);
                return; 
            }
        }

        Debug.LogWarning("Spawner: Could not find valid spawn position after retries.");
    }
}
