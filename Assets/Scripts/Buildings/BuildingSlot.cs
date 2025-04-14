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
    private ColorType owner = ColorType.Red;   // default

    public void PlaceBuilding(ColorType who, BuildingType type, bool ignoreResources = false)
    {
        if (type == BuildingType.None) return;

        if (current != null)
        {
            Debug.Log("BuildingSlot: Slot already occupied.");
            return;
        }

        // Resource and territory checks if not ignoring
        if (!ignoreResources)
        {
            if (!CanAffordBuilding(who, type))
            {
                Debug.Log("Not enough resources");
                return;
            }

            if (type != BuildingType.FlagBuilding && !IsInRangeOfTeamTerritory(who, 1))
            {
                Debug.Log("Cannot place building here — not in your 3x3 territory.");
                return;
            }

            ColorType enemy = (who == ColorType.Red ? ColorType.Blue : ColorType.Red);
            if (type != BuildingType.FlagBuilding && IsInRangeOfTeamTerritory(enemy, 1))
            {
                Debug.Log("Cannot place building here — it's in the other player's territory.");
                return;
            }
        }

        // Subtract resources if not ignoring
        if (!ignoreResources)
        {
            SpendResourcesFor(who, type);
        }

        Debug.Log($"BuildingSlot: Attempting to place {type} for {who} on cell {name}.");

        // Instantiate the building prefab
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
                var baseScript = current.GetComponent<BuildingBase>();
                if (baseScript)
                {
                    baseScript.team = (who == ColorType.Red) ? Team.Red : Team.Blue;
                }
                Debug.Log($"BuildingSlot: Placed {type} for {who} on {name}.");

                if (GridManager.Instance != null)
                {
                    GridManager.Instance.RefreshAllTerritories();
                }

                // New: Update the cost display in the UI after spending resources.
                if (BuildingSelectionUI.Instance != null)
                {
                    BuildingSelectionUI.Instance.UpdateCostDisplay(who);
                }

                return;
            }
        }

        Debug.LogWarning($"BuildingSlot: No mapping found for building type {type}.");
    }

    public bool IsBlocked(ColorType asker)
    {
        return (current != null && owner != asker);
    }

    public bool HasBuilding()
    {
        return current != null;
    }

    public ColorType GetOwner() => owner;

    // Checks if there is a building belonging to teamOwner in this cell's 3×3 vicinity.
    private bool IsInRangeOfTeamTerritory(ColorType teamOwner, int radius)
    {
        string[] parts = gameObject.name.Split('_');
        if (parts.Length < 3)
            return false;
        if (!int.TryParse(parts[1], out int row))
            return false;
        if (!int.TryParse(parts[2], out int col))
            return false;

        var gm = GridManager.Instance;
        if (gm == null)
            return false;

        for (int rr = row - radius; rr <= row + radius; rr++)
        {
            for (int cc = col - radius; cc <= col + radius; cc++)
            {
                GameObject cell = gm.GetCell(rr, cc);
                if (cell != null)
                {
                    BuildingSlot slot = cell.GetComponent<BuildingSlot>();
                    if (slot != null && slot.HasBuilding() && slot.GetOwner() == teamOwner)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Resource cost management using BuildingCostManager
    private bool CanAffordBuilding(ColorType who, BuildingType type)
    {
        (int goldCost, int runeCost) = BuildingCostManager.Instance.GetCost(type, who);

        if (who == ColorType.Red)
        {
            var redInv = Object.FindAnyObjectByType<RedInventory>();
            if (redInv == null)
            {
                Debug.LogError("BuildingSlot: RedInventory not found!");
                return false;
            }
            if (!redInv.HasEnoughResources(goldCost, runeCost))
            {
                redInv.FlashInsufficientResources(goldCost, runeCost);
                return false;
            }
        }
        else
        {
            var blueInv = Object.FindAnyObjectByType<Blue_Inventory>();
            if (blueInv == null)
            {
                Debug.LogError("BuildingSlot: Blue_Inventory not found!");
                return false;
            }
            if (!blueInv.HasEnoughResources(goldCost, runeCost))
            {
                blueInv.FlashInsufficientResources(goldCost, runeCost);
                return false;
            }
        }
        return true;
    }

    private void SpendResourcesFor(ColorType who, BuildingType type)
    {
        (int goldCost, int runeCost) = BuildingCostManager.Instance.GetCost(type, who);

        if (who == ColorType.Red)
        {
            var redInv = Object.FindAnyObjectByType<RedInventory>();
            if (redInv != null)
            {
                redInv.RemoveResources(goldCost, runeCost);
            }
        }
        else
        {
            var blueInv = Object.FindAnyObjectByType<Blue_Inventory>();
            if (blueInv != null)
            {
                blueInv.RemoveResources(goldCost, runeCost);
            }
        }
    }

    // New: Clears the building reference stored for this slot.
    public void ClearBuilding()
    {
        Debug.Log($"BuildingSlot ({name}): Clearing building reference. Previous building: {current}");
        current = null;
    }
}
