using UnityEngine;

public enum Team { Red, Blue }

public abstract class BuildingBase : MonoBehaviour
{
    [Header("Team + Health")]
    public Team team;                      // <— set during placement
    [SerializeField] private int maxHP = 10;
    private int currentHP;

    protected virtual void Awake() => currentHP = maxHP;

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0) DestroyBuilding();
    }

    protected virtual void DestroyBuilding()
    {
        Destroy(gameObject);
    }
}
