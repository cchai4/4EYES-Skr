using UnityEngine;
using System.Collections;

public enum Team { Red, Blue }

public abstract class BuildingBase : MonoBehaviour
{
    public Team team;         // assigned by BuildingSlot
    [SerializeField] private int maxHP = 10;
    protected int currentHP;

    protected virtual void Awake()
    {
        currentHP = maxHP;
    }

    public virtual void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            DestroyBuilding();
        }
    }

    protected virtual void DestroyBuilding()
    {
        Debug.Log($"BuildingBase: Destroying {name} (Team {team}).");
        Destroy(gameObject);

        // Delay the territory refresh by one frame to ensure this object is fully destroyed
        if (GridManager.Instance)
        {
            StartCoroutine(DelayedTerritoryRefresh());
        }
    }

    private IEnumerator DelayedTerritoryRefresh()
    {
        yield return null; // wait one frame
        GridManager.Instance.RefreshAllTerritories();
    }
}
