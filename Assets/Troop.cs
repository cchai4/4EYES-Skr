using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Troop : MonoBehaviour
{
    [Header("Team & Stats")]
    public Team team;
    public int maxHP = 5;
    public float moveSpeed = 2f;
    public float decayInterval = 3f;
    public int contactDamage = 1;

    [Header("Knockback Settings")]
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.4f;
    public float knockbackDamping = 15f;

    private int currentHP;
    private float decayTimer;
    private float retargetTimer;
    private const float retargetInterval = 1f;

    private Rigidbody2D rb;
    private Transform target;

    private bool isKnockedBack = false;
    private Vector2 knockbackVelocity = Vector2.zero;
    private float knockbackTimer = 0f;
    Animator animator;


    public void PlayAnimation()
    {
        animator.SetTrigger("PlayAnim");
    }
    void Awake()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Passive decay over time
        decayTimer += Time.deltaTime;
        if (decayTimer >= decayInterval)
        {
            TakeDamage(1);
            decayTimer = 0f;
        }

        // Handle knockback
        if (isKnockedBack)
        {
            knockbackTimer += Time.deltaTime;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * knockbackDamping);

            RotateTowards(rb.linearVelocity); // ✅ Face knockback direction

            if (knockbackTimer >= knockbackDuration)
            {
                isKnockedBack = false;
                knockbackTimer = 0f;
            }
            return;
        }

        // Target tracking
        retargetTimer += Time.deltaTime;
        if (retargetTimer >= retargetInterval)
        {
            target = FindClosestEnemy();
            retargetTimer = 0f;
        }

        // Normal movement
        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;

            RotateTowards(rb.linearVelocity); // ✅ Face movement direction
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    Transform FindClosestEnemy()
    {
        var enemies =
            FindObjectsByType<BuildingBase>(FindObjectsSortMode.None).Where(b => b.team != team).Select(b => b.transform)
            .Concat(FindObjectsByType<Troop>(FindObjectsSortMode.None).Where(t => t.team != team).Select(t => t.transform));

        float bestDist = float.MaxValue;
        Transform closest = null;

        foreach (var t in enemies)
        {
            float d = (t.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                closest = t;
            }
        }

        return closest;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Enemy building
        var building = col.GetComponent<BuildingBase>();
        if (building && building.team != team)
        {
            building.TakeDamage(contactDamage);
            KnockBack(col.transform.position);
            return;
        }

        // Enemy troop
        var enemyTroop = col.GetComponent<Troop>();
        if (enemyTroop && enemyTroop.team != team)
        {
            enemyTroop.TakeDamage(contactDamage);
            KnockBack(col.transform.position);
        }
    }

    void KnockBack(Vector3 hitSource)
    {
        Vector2 knockDir = (transform.position - hitSource).normalized;

        isKnockedBack = true;
        knockbackVelocity = knockDir * knockbackForce;
        rb.linearVelocity = knockbackVelocity;

        RotateTowards(rb.linearVelocity); // ✅ Immediately face knockback direction

        TakeDamage(1); // Optional: self-damage
    }

    void RotateTowards(Vector2 velocity)
    {
        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f; // Subtract 90 degrees
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
            Destroy(gameObject);
    }
}
