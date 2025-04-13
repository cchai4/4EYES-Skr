using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Cannonball : MonoBehaviour
{
    public Team team;             // This will be set by Init() based on the firing cannon.
    public float speed = 8f;      // Speed (set by cannon).
    public float lifetime = 5f;   // Lifetime before auto-destroy.
    public float knockback = 4f;  // Knockback force.
    public int damage = 1;        // Damage dealt on hit.

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;   // projectile is a trigger
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var enemyTroop = col.GetComponent<Troop>();
        if (enemyTroop && enemyTroop.team != team)
        {
            enemyTroop.TakeDamage(damage);

            Vector2 away = (enemyTroop.transform.position - transform.position).normalized;
            var rbEnemy = enemyTroop.GetComponent<Rigidbody2D>();
            if (rbEnemy) rbEnemy.AddForce(away * knockback, ForceMode2D.Impulse);
            Destroy(gameObject);
        }
    }

    public void Init(Vector2 dir, Team ownerTeam)
    {
        team = ownerTeam;
        rb.linearVelocity = dir.normalized * speed;
        // Rotate the cannonball so it faces the shooting direction.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
