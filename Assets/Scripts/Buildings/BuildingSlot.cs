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

        // Resource + territory checks if not ignoring
        if (!ignoreResources)
        {
            if (!CanAffordBuilding(who, type))
            {
                Debug.Log("Not enough resources");
                return;
            }

            // For non-flag buildings, must be in your 3x3 territory
            if (type != BuildingType.Flag && !IsInRangeOfTeamTerritory(who, 1))
            {
                Debug.Log("Cannot place building here — not in your 3x3 territory.");
                return;
            }

            // Also ensure it's not in the enemy’s 3x3 territory (exclusive rule).
            ColorType enemy = (who == ColorType.Red ? ColorType.Blue : ColorType.Red);
            if (type != BuildingType.Flag && IsInRangeOfTeamTerritory(enemy, 1))
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

        // Instantiate
        foreach (var p in prefabs)
        {
            if (p.type == type)
            {
                GameObject prefab = (who == ColorType.Red) ? p.redPrefab : p.bluePrefab;
                if (prefab == null)
                {
                    Debug.LogWarning($"No prefab for {who} {type}");
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

                // After building is placed, refresh territory so coverage is updated
                if (GridManager.Instance)
                {
                    GridManager.Instance.RefreshAllTerritories();
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

    // Check if there's a same-team building within 'radius' cells
    // e.g. radius=1 => 3x3 territory
    private bool IsInRangeOfTeamTerritory(ColorType teamOwner, int radius)
    {
        string[] parts = gameObject.name.Split('_');
        if (parts.Length < 3) return false;
        if (!int.TryParse(parts[1], out int row)) return false;
        if (!int.TryParse(parts[2], out int col)) return false;

        var gm = GridManager.Instance;
        if (!gm) return false;

        for (int rr = row - radius; rr <= row + radius; rr++)
        {
            for (int cc = col - radius; cc <= col + radius; cc++)
            {
                GameObject cell = gm.GetCell(rr, cc);
                if (cell != null)
                {
                    BuildingSlot slot = cell.GetComponent<BuildingSlot>();
                    if (slot && slot.HasBuilding() && slot.GetOwner() == teamOwner)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Resource Logic
    private bool CanAffordBuilding(ColorType who, BuildingType type)
    {
        // If you have a separate BuildingCostManager, use it. Example:
        (int goldCost, int runeCost) = BuildingCostManager.Instance.GetCost(type);

        if (who == ColorType.Red)
        {
            var redInv = Object.FindAnyObjectByType<RedInventory>();
            if (redInv == null) return false;
            if (!redInv.HasEnoughResources(goldCost, runeCost))
            {
                redInv.FlashInsufficientResources(goldCost, runeCost);
                return false;
            }
        }
        else
        {
            var blueInv = Object.FindAnyObjectByType<Blue_Inventory>();
            if (blueInv == null) return false;
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
        (int goldCost, int runeCost) = BuildingCostManager.Instance.GetCost(type);

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
}
