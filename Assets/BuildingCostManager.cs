using UnityEngine;

[System.Serializable]
public class BuildingCost
{
    public BuildingType type;
    public int gold;
    public int runes;
}

public class BuildingCostManager : MonoBehaviour
{
    public static BuildingCostManager Instance { get; private set; }

    [Header("Set building costs here")]
    public BuildingCost[] costs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public int GetGold(BuildingType type)
    {
        foreach (var c in costs)
            if (c.type == type) return c.gold;
        return 0;
    }

    public int GetRunes(BuildingType type)
    {
        foreach (var c in costs)
            if (c.type == type) return c.runes;
        return 0;
    }

    public (int gold, int runes) GetCost(BuildingType type)
    {
        foreach (var c in costs)
            if (c.type == type) return (c.gold, c.runes);
        return (0, 0);
    }
}
