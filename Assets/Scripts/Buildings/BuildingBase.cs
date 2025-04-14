using UnityEngine;
using System.Collections;

public enum Team { Red, Blue }

public abstract class BuildingBase : MonoBehaviour
{
    public Team team;         // Assigned by BuildingSlot when placed.
    [SerializeField] private int maxHP = 10;
    protected int currentHP;

    // Flag to check if destruction was handled through our controlled method.
    private bool destructionHandled = false;

    protected virtual void Awake()
    {
        currentHP = maxHP;
        Debug.Log($"{name}: Awake, currentHP = {currentHP}");
    }

    // When damage is taken, if HP reaches or falls below 0, destroy the building.
    public virtual void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log($"{name}: Took damage {dmg}. CurrentHP now = {currentHP}");
        if (currentHP <= 0)
        {
            Debug.Log($"{name}: HP reached 0 or below, calling DestroyBuilding()");
            DestroyBuilding();
        }
    }

    /// <summary>
    /// Use this method for controlled destruction so that territory refresh happens.
    /// This method clears the reference from its parent BuildingSlot before destroying itself.
    /// </summary>
    protected virtual void DestroyBuilding()
    {
        destructionHandled = true;
        Debug.Log($"BuildingBase: Destroying {name} (Team {team}).");

        // Clear the parent's building reference.
        if (transform.parent != null)
        {
            var slot = transform.parent.GetComponent<BuildingSlot>();
            if (slot != null)
            {
                Debug.Log($"{name}: Clearing building reference from parent BuildingSlot '{slot.name}'.");
                slot.ClearBuilding();
            }
            else
            {
                Debug.LogWarning($"{name}: Parent does not have a BuildingSlot component.");
            }
        }
        else
        {
            Debug.LogWarning($"{name}: No parent found to clear building reference from.");
        }

        // Immediately refresh territory.
        if (GridManager.Instance != null)
        {
            Debug.Log($"{name}: Immediately refreshing territory via GridManager.");
            GridManager.Instance.RefreshAllTerritories();
        }
        else
        {
            Debug.LogWarning($"{name}: GridManager.Instance is null, cannot refresh territory.");
        }

        // Now schedule destruction.
        Destroy(gameObject);
    }


    private IEnumerator DelayedTerritoryRefresh()
    {
        Debug.Log($"{name}: Waiting one frame for territory refresh.");
        yield return null; // Wait one frame.
        if (GridManager.Instance != null)
        {
            Debug.Log($"{name}: Refreshing territory via GridManager.");
            GridManager.Instance.RefreshAllTerritories();
        }
        else
        {
            Debug.LogWarning($"{name}: GridManager.Instance is null, cannot refresh territory.");
        }
    }

    // Fallback: if this building is destroyed directly (without calling DestroyBuilding()),
    // then refresh territory here.
    private void OnDestroy()
    {
        if (!destructionHandled)
        {
            Debug.LogWarning($"{name}: OnDestroy() called without controlled destruction. Refreshing territory.");
            if (GridManager.Instance != null)
                GridManager.Instance.RefreshAllTerritories();
        }
    }
}
