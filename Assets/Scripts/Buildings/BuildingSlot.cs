using UnityEngine;
using static GridCellTint;

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

    public void PlaceBuilding(ColorType who, BuildingType type)
    {
        if (type == BuildingType.None)
            return;

        if (current != null)
        {
            Debug.Log("Slot already occupied");
            return;
        }

        if (!CanAffordBuilding(who, type))
        {
            BuildingSelectionUI.Instance.EndSelection();
            return;
        }

        SpendResourcesFor(who, type);

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
                    baseScript.team = (who == ColorType.Red) ? Team.Red : Team.Blue;

                return;
            }
        }

        Debug.LogWarning($"BuildingSlot: No mapping for {type}");
    }

    public bool IsBlocked(ColorType asker)
    {
        return (current != null && owner != asker);
    }

    // ? NEW
    public bool HasBuilding()
    {
        return current != null;
    }

    private void GetBuildingCost(BuildingType type, out int gold, out int runes)
    {
        gold = 0;
        runes = 0;

        switch (type)
        {
            case BuildingType.Generator:
                gold = 20;
                runes = 0;
                break;
            case BuildingType.TroopSpawner:
                gold = 30;
                runes = 0;
                break;
            case BuildingType.Cannon:
                gold = 30;
                runes = 1;
                break;
            case BuildingType.Flag:
                gold = 50;
                runes = 3;
                break;
        }
    }

    private bool CanAffordBuilding(ColorType who, BuildingType type)
    {
        GetBuildingCost(type, out int goldCost, out int runeCost);

        if (who == ColorType.Red)
        {
            RedInventory redInv = Object.FindAnyObjectByType<RedInventory>();
            if (redInv == null)
            {
                Debug.LogError("RedInventory not found in scene!");
                return false;
            }

            if (!redInv.HasEnoughResources(goldCost, runeCost))
            {
                redInv.FlashInsufficientResources(goldCost, runeCost);
                return false;
            }

            return true;
        }
        else
        {
            Blue_Inventory blueInv = Object.FindAnyObjectByType<Blue_Inventory>();
            if (blueInv == null)
            {
                Debug.LogError("Blue_Inventory not found in scene!");
                return false;
            }

            if (!blueInv.HasEnoughResources(goldCost, runeCost))
            {
                blueInv.FlashInsufficientResources(goldCost, runeCost);
                return false;
            }

            return true;
        }
    }

    private void SpendResourcesFor(ColorType who, BuildingType type)
    {
        GetBuildingCost(type, out int goldCost, out int runeCost);

        if (who == ColorType.Red)
        {
            RedInventory redInv = Object.FindAnyObjectByType<RedInventory>();
            if (redInv != null)
            {
                redInv.RemoveResources(goldCost, runeCost);
            }
        }
        else
        {
            Blue_Inventory blueInv = Object.FindAnyObjectByType<Blue_Inventory>();
            if (blueInv != null)
            {
                blueInv.RemoveResources(goldCost, runeCost);
            }
        }
    }
}
