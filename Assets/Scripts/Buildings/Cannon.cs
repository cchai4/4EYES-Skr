using UnityEngine;
using System.Linq;

public class Cannon : BuildingBase
{
    [Header("Cannon Settings")]
    public GameObject cannonballPrefab;   // Assign the Cannonball prefab in the Inspector.
    public float shootInterval = 2f;      // Seconds between shots.
    public float projectileSpeed = 8f;    // Speed for the fired cannonball.
    public float projectileKnockback = 4f; // Knockback force applied by the cannonball.

    private float shootTimer = 0f;
    private Transform currentTarget;

    void Update()
    {
        shootTimer += Time.deltaTime;

        // Look for the closest enemy troop each frame
        currentTarget = FindClosestEnemyTroop();

        if (currentTarget != null)
        {
            RotateTowards(currentTarget);

            if (shootTimer >= shootInterval)
            {
                ShootAt(currentTarget);
                shootTimer = 0f;
            }
        }
        else
        {
            // No valid target ? stop firing
            shootTimer = 0f;
        }
    }

    void RotateTowards(Transform target)
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void ShootAt(Transform target)
    {
        if (cannonballPrefab == null) return;

        Vector2 shootDir = (target.position - transform.position).normalized;
        float offset = 0.5f;
        Vector3 spawnPos = transform.position + (Vector3)(shootDir * offset);

        GameObject proj = Instantiate(cannonballPrefab, spawnPos, Quaternion.identity);
        Cannonball cb = proj.GetComponent<Cannonball>();
        if (cb != null)
        {
            cb.speed = projectileSpeed;
            cb.knockback = projectileKnockback;
            cb.Init(shootDir, team);
        }
    }

    Transform FindClosestEnemyTroop()
    {
        var enemies = FindObjectsByType<Troop>(FindObjectsSortMode.None)
            .Where(t => t.team != team)
            .Select(t => t.transform);

        float bestDist = float.MaxValue;
        Transform closest = null;

        foreach (Transform t in enemies)
        {
            float dist = (t.position - transform.position).sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                closest = t;
            }
        }

        return closest;
    }
}
