using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class Troop : MonoBehaviour
{
    [Header("Team & Stats")]
    public Team team;            // set by Barrack when spawned
    public int maxHP = 5; // tweak per prefab
    public float moveSpeed = 2f;
    public float decayInterval = 3f;   // lose 1?HP every 3?s
    public int contactDamage = 1;    // dmg dealt to buildings on hit

    /* private runtime */
    int currentHP;
    float decayTimer;
    float retargetTimer;
    const float retargetInterval = 1.0f;

    Rigidbody2D rb;
    BuildingBase target;

    void Awake()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        /* passive HP decay */
        decayTimer += Time.deltaTime;
        if (decayTimer >= decayInterval)
        {
            decayTimer = 0f;
            TakeDamage(1);
        }

        /* retarget every second */
        retargetTimer += Time.deltaTime;
        if (retargetTimer >= retargetInterval)
        {
            retargetTimer = 0f;
            target = FindClosestEnemyBuilding();
        }

        /* move toward target */
        if (target != null)
        {
            Vector2 dir = (target.transform.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // no enemy buildings � stay idle
        }
    }

    /* search all buildings, pick nearest enemy */
    BuildingBase FindClosestEnemyBuilding()
    {
        var all = FindObjectsOfType<BuildingBase>();
        var enemies = all.Where(b => b.team != team);
        float bestDist = float.MaxValue;
        BuildingBase closest = null;

        foreach (var b in enemies)
        {
            float d = Vector2.SqrMagnitude(b.transform.position - transform.position);
            if (d < bestDist) { bestDist = d; closest = b; }
        }
        return closest;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var b = col.GetComponent<BuildingBase>();
        if (b && b.team != team)
        {
            b.TakeDamage(contactDamage);
            TakeDamage(maxHP); // die after hit (kamikaze). Remove if you want survival.
        }
    }

    void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0) Destroy(gameObject);
    }
}
