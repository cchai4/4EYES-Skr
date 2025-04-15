using UnityEngine;
using System.Collections;
using static GridCellTint;

public class FlagInitializer : MonoBehaviour
{
    public GameObject redFlagPrefab;
    public GameObject blueFlagPrefab;

    void Start()
    {
        StartCoroutine(WaitForGridAndPlaceFlags());
    }

    IEnumerator WaitForGridAndPlaceFlags()
    {
        // Find the GridManager.
        GridManager gm = Object.FindAnyObjectByType<GridManager>();
        if (gm == null)
        {
            Debug.LogError("FlagInitializer: GridManager not found!");
            yield break;
        }

        // Wait until grid is generated.
        while (gm.gridParent.childCount == 0)
        {
            Debug.Log("FlagInitializer: Waiting for grid to generate...");
            yield return null;
        }

        int rows = gm.GetRowCount();
        int cols = gm.GetColCount();
        Debug.Log($"FlagInitializer: Grid dimensions: {rows} rows, {cols} columns.");

        int midCol = cols / 2;
        Debug.Log($"FlagInitializer: Calculated middle column: {midCol}");

        // Define flag placement: Red flag at row 1, Blue flag at row (rows - 2)
        int redRow = 1;
        int blueRow = rows - 2;
        Debug.Log($"FlagInitializer: Red flag should be placed at row {redRow}, column {midCol}.");
        Debug.Log($"FlagInitializer: Blue flag should be placed at row {blueRow}, column {midCol}.");

        GameObject redCell = gm.GetCell(redRow, midCol);
        if (redCell != null)
        {
            Debug.Log($"FlagInitializer: Found red cell at ({redRow}, {midCol}) - {redCell.name}");
            BuildingSlot redSlot = redCell.GetComponent<BuildingSlot>();
            if (redSlot != null)
            {
                redSlot.PlaceBuilding(ColorType.Red, BuildingType.FlagBuilding, true); // bypass resource check
                Debug.Log("FlagInitializer: Placed red flag (ignoring resources).");
            }
            else
            {
                Debug.LogWarning("FlagInitializer: BuildingSlot component not found on red cell.");
            }
        }
        else
        {
            Debug.LogWarning("FlagInitializer: Red cell for flag placement not found.");
        }

        GameObject blueCell = gm.GetCell(blueRow, midCol);
        if (blueCell != null)
        {
            Debug.Log($"FlagInitializer: Found blue cell at ({blueRow}, {midCol}) - {blueCell.name}");
            BuildingSlot blueSlot = blueCell.GetComponent<BuildingSlot>();
            if (blueSlot != null)
            {
                blueSlot.PlaceBuilding(ColorType.Blue, BuildingType.FlagBuilding, true); // bypass resource check
                // Debug.Log("FlagInitializer: Placed blue flag (ignoring resources).");
            }
            else
            {
                Debug.LogWarning("FlagInitializer: BuildingSlot component not found on blue cell.");
            }
        }
        else
        {
            Debug.LogWarning("FlagInitializer: Blue cell for flag placement not found.");
        }
    }
}
