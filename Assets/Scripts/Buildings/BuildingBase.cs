using UnityEngine;

public enum Team { Red, Blue }

public abstract class BuildingBase : MonoBehaviour
{
    [Header("Team + Health")]
    public Team team;                      // Set during placement
    [SerializeField] private int maxHP = 10;
    protected int currentHP;

    protected virtual void Awake() => currentHP = maxHP;

    // ? Marked as virtual so child classes can override it
    public virtual void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
            DestroyBuilding();
    }

    protected virtual void DestroyBuilding()
    {
        Destroy(gameObject);
    }
}
