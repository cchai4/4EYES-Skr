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

    /* place a building owned by 'who' */
    public void PlaceBuilding(ColorType who, BuildingType type)
    {
        if (type == BuildingType.None) return;
        if (current != null) { Debug.Log("Slot already occupied"); return; }

        foreach (var p in prefabs)
            if (p.type == type)
            {
                GameObject prefab = (who == ColorType.Red) ? p.redPrefab : p.bluePrefab;
                if (prefab == null) { Debug.LogWarning($"No prefab for {who} {type}"); return; }

                Vector3 pos = transform.position + new Vector3(0, 0, -0.1f); // render on top
                current = Instantiate(prefab, pos, Quaternion.identity, transform);
                owner = who;
                var baseScript = current.GetComponent<BuildingBase>();
                if (baseScript) baseScript.team = (who == ColorType.Red) ? Team.Red : Team.Blue;
                return;
            }

        Debug.LogWarning($"BuildingSlot: No mapping for {type}");
    }

    /* true if this cell has an enemy building */
    public bool IsBlocked(ColorType asker)
    {
        return current != null && owner != asker;
    }
}
