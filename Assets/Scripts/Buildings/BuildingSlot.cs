using UnityEngine;

public class BuildingSlot : MonoBehaviour
{
    [System.Serializable]
    public struct PrefabMap
    {
        public BuildingType type;
        public GameObject prefab;
    }

    [Header("Prefabs for each type")]
    public PrefabMap[] prefabs;         // fill in via Inspector

    GameObject current;

    public bool IsOccupied => current != null;

    public void PlaceBuilding(BuildingType type)
    {
        if (IsOccupied) { Debug.LogWarning($"Cell {name} already has a building"); return; }

        foreach (var m in prefabs)
            if (m.type == type)
            {
                current = Instantiate(m.prefab, transform.position, Quaternion.identity, transform);
                return;
            }

        Debug.LogWarning($"No prefab mapped for {type}");
    }

    public void Clear()
    {
        if (current) Destroy(current);
        current = null;
    }
}
