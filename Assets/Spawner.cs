using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float rand_x_pos;
    public float rand_y_pos;

    public void spawnResources(GameObject spawn_obj)
    {
        rand_x_pos = Random.Range(-2f, -8f);
        rand_y_pos = Random.Range(-4f, 4.1f);

        Vector3 rand_pos = new Vector3(rand_x_pos, rand_y_pos, 0);

        Instantiate(spawn_obj, rand_pos, Quaternion.identity);
    }
}