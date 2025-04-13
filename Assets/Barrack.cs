using UnityEngine;

public class Barrack : BuildingBase
{
    [Header("Troop Settings")]
    public GameObject troopPrefab;   // assign in Inspector
    public float spawnInterval = 3f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTroop();
            timer = 0f;
        }
    }

    void SpawnTroop()
    {
        Vector3 spawnPos = transform.position;
        var unit = Instantiate(troopPrefab, spawnPos, Quaternion.identity);

        var troop = unit.GetComponent<Troop>();
        if (troop) troop.team = team;           // pass ownership
    }
}
