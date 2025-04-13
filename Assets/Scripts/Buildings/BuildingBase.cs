using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    [Header("Common Stats")]
    public int maxHealth = 100;
    protected int currentHealth;

    protected virtual void Awake() => currentHealth = maxHealth;

    public virtual void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}
