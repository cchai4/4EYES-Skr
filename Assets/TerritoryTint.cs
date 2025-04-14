using UnityEngine;
using static GridCellTint;
using static GridCellTint.ColorType;

public class TerritoryTint : MonoBehaviour
{
    [Header("Territory Colors")]
    public Color redTint = new Color(1f, 0.8f, 0.8f, 0.5f);
    public Color blueTint = new Color(0.8f, 0.8f, 1f, 0.5f);
    public Color purpleTint = new Color(1f, 0.6f, 1f, 0.5f);
    public Color defaultColor = Color.white;

    private GridCellTint cellTint;
    private GridManager gm;
    private int row, col;

    void Awake()
    {
        cellTint = GetComponent<GridCellTint>();
        gm = Object.FindAnyObjectByType<GridManager>();
    }

    public void SetCoords(int r, int c)
    {
        row = r;
        col = c;
    }

    /// <summary>
    /// Called by GridManager.RefreshAllTerritories to re-check coverage from all buildings
    /// in a 3×3 area (radius=1). Then sets baseColor in the highlight script.
    /// </summary>
    public void RefreshColor()
    {
        // Log that we're refreshing this cell
        Debug.Log($"TerritoryTint [{name}] at row={row},col={col}: RefreshColor() called.");

        if (!gm)
        {
            Debug.LogWarning($"TerritoryTint [{name}]: gm is null, defaulting color.");
            cellTint?.SetBaseColor(defaultColor);
            return;
        }
        if (cellTint == null)
        {
            Debug.LogWarning($"TerritoryTint [{name}]: cellTint is null, can't set color!");
            return;
        }

        bool inRed = IsInTerritory(Red, 1);
        bool inBlue = IsInTerritory(Blue, 1);

        Debug.Log($"TerritoryTint [{name}] row={row},col={col}: inRed={inRed}, inBlue={inBlue}");

        // Decide final color
        if (inRed && inBlue)
        {
            cellTint.SetBaseColor(purpleTint);
            Debug.Log($"TerritoryTint [{name}] -> Purple");
        }
        else if (inRed)
        {
            cellTint.SetBaseColor(redTint);
            Debug.Log($"TerritoryTint [{name}] -> Red");
        }
        else if (inBlue)
        {
            cellTint.SetBaseColor(blueTint);
            Debug.Log($"TerritoryTint [{name}] -> Blue");
        }
        else
        {
            cellTint.SetBaseColor(defaultColor);
            Debug.Log($"TerritoryTint [{name}] -> Default");
        }
    }

    /// <summary>
    /// True if there's a building from 'teamOwner' in the 3x3 block around this cell.
    /// </summary>
    private bool IsInTerritory(ColorType teamOwner, int radius)
    {
        for (int rr = row - radius; rr <= row + radius; rr++)
        {
            for (int cc = col - radius; cc <= col + radius; cc++)
            {
                GameObject cellObj = gm.GetCell(rr, cc);
                if (cellObj != null)
                {
                    BuildingSlot slot = cellObj.GetComponent<BuildingSlot>();
                    if (slot && slot.HasBuilding())
                    {
                        // If the building belongs to 'teamOwner'
                        if (slot.GetOwner() == teamOwner)
                        {
                            // Log for clarity
                            Debug.Log($"TerritoryTint [{name}] row={row},col={col}: Found {teamOwner} building at row={rr},col={cc}");
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}
