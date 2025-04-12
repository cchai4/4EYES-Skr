using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float spawnRate = 5;
    private float timer = 0;
    public float rand_x_pos;
    public float rand_y_pos;
    public GameObject gold;
    public GameObject silver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            spawnResources(gold);
            timer = 0;
        }
    }
    
    void spawnResources(GameObject spawn_obj)
    {
        rand_x_pos = Random.Range(-2, -8);
        rand_y_pos = Random.Range(-4, 5);

        Vector3 rand_pos = new Vector3(rand_x_pos, rand_y_pos, 0);

        Instantiate(spawn_obj, rand_pos, Quaternion.identity);
    }
}
