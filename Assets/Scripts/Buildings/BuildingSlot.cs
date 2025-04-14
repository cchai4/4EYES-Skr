using UnityEngine;
using static GridCellTint;
using static GridCellTint.ColorType;

public class BuildingSlot : MonoBehaviour
{
    [System.Serializable]
    public struct PrefabSet
    {
        public BuildingType type;
        public GameObject redPrefab;
        public GameObject bluePrefab;
    }

    [Header("Prefab Mapping (set in Inspector)")]
    public PrefabSet[] prefabs;

    private GameObject current;
    private ColorType owner = ColorType.Red; // default

    /// <summary>
    /// Attempts to place a building in this slot.
    /// </summary>
    public void PlaceBuilding(ColorType who, BuildingType type, bool ignoreResources = false)
    {
        if (type == BuildingType.None) return;
        if (current != null)
        {
            Debug.Log("BuildingSlot: Slot already occupied.");
            return;
        }

        // (Insert resource and territory checks here if needed)

        Debug.Log($"BuildingSlot: Attempting to place {type} for {who} on cell {name}.");
        foreach (var p in prefabs)
        {
            if (p.type == type)
            {
                GameObject prefab = (who == ColorType.Red) ? p.redPrefab : p.bluePrefab;
                if (prefab == null)
                {
                    Debug.LogWarning($"BuildingSlot: No prefab for {who} {type}");
                    return;
                }

                Vector3 pos = transform.position + new Vector3(0, 0, -0.1f);
                current = Instantiate(prefab, pos, Quaternion.identity, transform);
                owner = who;
                Debug.Log($"BuildingSlot: Placed {type} for {who} on {name}.");

                // (Optionally refresh territory or update UI)
                if (GridManager.Instance != null)
                    GridManager.Instance.RefreshAllTerritories();

                if (BuildingSelectionUI.Instance != null)
                    BuildingSelectionUI.Instance.UpdateCostDisplay(who);

                return;
            }
        }
        Debug.LogWarning($"BuildingSlot: No mapping found for building type {type}.");
    }

    /// <summary>
    /// Returns true if this slot is blocked for the requesting owner.
    /// </summary>
    public bool IsBlocked(ColorType asker)
    {
        return (current != null && owner != asker);
    }

    /// <summary>
    /// Checks if there is a building in this slot.
    /// </summary>
    public bool HasBuilding()
    {
        return current != null;
    }

    /// <summary>
    /// Returns the owner of the building in this slot.
    /// </summary>
    public ColorType GetOwner() => owner;

    /// <summary>
    /// Clears the current building (destroys the instantiated building and marks this slot as empty).
    /// </summary>
    public void ClearBuilding()
    {
        if (current != null)
        {
            Destroy(current);
            current = null;
            Debug.Log($"BuildingSlot ({name}): Cleared building.");
        }
    }
}
