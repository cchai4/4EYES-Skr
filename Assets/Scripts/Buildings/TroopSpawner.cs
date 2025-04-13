// TroopSpawner.cs
using UnityEngine;

public class TroopSpawner : BuildingBase
{
    public GameObject troopPrefab;
    public float spawnInterval = 3f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            Instantiate(troopPrefab, transform.position, Quaternion.identity);
            timer = 0f;
        }
    }
}
