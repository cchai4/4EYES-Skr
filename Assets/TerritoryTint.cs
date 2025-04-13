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

    private GridCellTint cellTint; // handshake with the highlight script
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
    /// Called by GridManager.RefreshAllTerritories to re-check coverage 
    /// from all buildings in a 3x3 area. Then sets baseColor in the highlight script.
    /// </summary>
    public void RefreshColor()
    {
        if (!gm || cellTint == null)
        {
            // fallback: no coverage
            cellTint?.SetBaseColor(defaultColor);
            return;
        }

        // 3x3 territory => radius=1
        bool inRed = IsInTerritory(Red, 1);
        bool inBlue = IsInTerritory(Blue, 1);

        if (inRed && inBlue)
        {
            cellTint.SetBaseColor(purpleTint);
        }
        else if (inRed)
        {
            cellTint.SetBaseColor(redTint);
        }
        else if (inBlue)
        {
            cellTint.SetBaseColor(blueTint);
        }
        else
        {
            cellTint.SetBaseColor(defaultColor);
        }
    }

    /// <summary>
    /// True if there's a building from 'teamOwner' in the 3x3 block around this cell.
    /// </summary>
    bool IsInTerritory(ColorType teamOwner, int radius)
    {
        for (int rr = row - radius; rr <= row + radius; rr++)
        {
            for (int cc = col - radius; cc <= col + radius; cc++)
            {
                GameObject cell = gm.GetCell(rr, cc);
                if (cell)
                {
                    var slot = cell.GetComponent<BuildingSlot>();
                    if (slot && slot.HasBuilding())
                    {
                        // If the building belongs to 'teamOwner'
                        if (slot.GetOwner() == teamOwner)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}
