using UnityEngine;

public class BuildingCostManager : MonoBehaviour
{
    public static BuildingCostManager Instance { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public (int gold, int runes) GetCost(BuildingType type, GridCellTint.ColorType owner)
    {
        int count = CountOwnedBuildings(type, owner);
        int gold = 0, runes = 0;

        switch (type)
        {
            case BuildingType.Generator:
                gold = Mathf.Min(10 + 10 * count, 40);
                runes = 0;
                break;

            case BuildingType.Barrack: // barracks
                gold = Mathf.Min(20 + 10 * count, 50);
                runes = 1;
                break;

            case BuildingType.Cannon:
                gold = Mathf.Min(30 + 10 * count, 60);
                runes = 2;
                break;

            case BuildingType.FlagBuilding:
                gold = Mathf.Min(40 + 10 * count, 70);
                runes = Mathf.Min(3 + count, 5);
                break;
        }

        return (gold, runes);
    }

    private int CountOwnedBuildings(BuildingType type, GridCellTint.ColorType owner)
    {
        int count = 0;
        foreach (var building in Object.FindObjectsByType<BuildingBase>(FindObjectsSortMode.None))
        {
            bool isOwner = (building.team == (owner == GridCellTint.ColorType.Red ? Team.Red : Team.Blue));
            if (!isOwner) continue;

            var scriptName = type.ToString(); // e.g. "Generator", "Cannon"
            if (building.GetComponent(scriptName) != null)
            {
                count++;
            }
        }
        return count;
    }
}
