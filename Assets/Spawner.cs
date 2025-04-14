using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public float rand_x_pos;
    public float rand_y_pos;

    // List to store all spawned resource objects
    public List<GameObject> spawnedResources = new List<GameObject>();

<<<<<<< Updated upstream
=======
    // List to store all spawned resource objects
    public List<GameObject> spawnedResources = new List<GameObject>();

>>>>>>> Stashed changes
    // Method to spawn a resource (e.g., gold or silver)
    public void spawnResources(GameObject spawn_obj)
    {
        rand_x_pos = Random.Range(-2f, -8f);
        rand_y_pos = Random.Range(-4f, 4.1f);

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        Vector3 rand_pos = new Vector3(rand_x_pos, rand_y_pos, 0);
=======
=======
>>>>>>> Stashed changes
            // Only spawn if the position is clear (no overlap with gold/silver)
            if (!Physics2D.OverlapCircle(spawnPos, spawnRadius, resourceLayer))
            {
                GameObject spawned = Instantiate(spawn_obj, spawnPos, Quaternion.identity);
                // Add the spawned object to the list for future reference
                spawnedResources.Add(spawned);
                return; // success, exit early
            }
        }
>>>>>>> Stashed changes

        Instantiate(spawn_obj, rand_pos, Quaternion.identity);
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
        return positions;
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
        return positions;
    }
}
