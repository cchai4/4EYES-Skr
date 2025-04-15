using UnityEngine;
using static GridCellTint;

public class TerritoryManager : MonoBehaviour
{
    public GridManager gridManager;

    public int GetTerritoryCount(Team team)
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager not assigned in TerritoryManager.");
            return 0;
        }

        int count = 0;
        int rows = gridManager.GetRowCount();
        int cols = gridManager.GetColCount();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cell = gridManager.GetCell(r, c);
                if (cell != null)
                {
                    BuildingSlot slot = cell.GetComponent<BuildingSlot>();
                    if (slot != null && slot.HasBuilding())
                    {
                        if (MapColorToTeam(slot.GetOwner()) == team)
                            count++;
                    }
                }
            }
        }
        return count;
    }

    private Team MapColorToTeam(ColorType ct)
    {
        return ct == ColorType.Red ? Team.Red : Team.Blue;
    }
}
