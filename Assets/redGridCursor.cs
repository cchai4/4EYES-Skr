using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;  // For LINQ methods (e.g., Any)
using static GridCellTint;          // For ColorType
using static GridCellTint.ColorType; // For Red

public class RedGridCursor : MonoBehaviour
{
    // Grid variables (initialized using the GridManager's gridParent)
    private static GameObject[,] cells;
    private static bool[,] occupied;
    private static int rows, cols;

    // Current grid coordinates and state.
    private int curRow, curCol;
    private bool hasJoined = false;
    // Flags to manage the auto process.
    private bool buildingProcessTriggered = false;
    private bool autoProcessStarted = false;
    private bool generatorBuilt = false;

    // Since this is RedGridCursor, tintOwner is fixed to Red.
    private ColorType tintOwner = Red;

    // Delay (in seconds) after joining a cell.
    public float joinDelay = 0.5f;

    void Start() => StartCoroutine(InitGrid());

    IEnumerator InitGrid()
    {
        // Wait until GridManager's gridParent becomes available.
        Transform gp = null;
        while (gp == null || gp.childCount == 0)
        {
            gp = FindFirstObjectByType<GridManager>()?.gridParent;
            yield return null;
        }

        // Initialize our grid arrays based on the number of children.
        rows = cols = Mathf.RoundToInt(Mathf.Sqrt(gp.childCount));
        cells = new GameObject[rows, cols];
        occupied = new bool[rows, cols];

        foreach (Transform t in gp)
        {
            string[] parts = t.name.Split('_');
            if (parts.Length == 3 &&
                int.TryParse(parts[1], out int r) &&
                int.TryParse(parts[2], out int c))
            {
                cells[r, c] = t.gameObject;
            }
        }

        // Place the cursor in the first free cell.
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (!occupied[r, c])
                {
                    curRow = r;
                    curCol = c;
                    yield return StartCoroutine(AnimateEnterCell(r, c));
                    hasJoined = true;
                    yield return new WaitForSeconds(joinDelay);
                    // At game start, nothing is automatically triggered until a collision sets redInGrid.
                    yield break;
                }
            }
        }
    }

    void Update()
    {
        // Only start the auto process if the red bot is on the grid and active.
        if (!hasJoined)
        {
            if (GridManager.Instance.RedInGrid)
            {
                hasJoined = true;
            }
            return;
        }

        if (GridManager.Instance != null && GridManager.Instance.RedInGrid && !autoProcessStarted)
        {
            autoProcessStarted = true;
            StartCoroutine(AutoProcess());
        }
        Debug.Log($"The current state of RedINGRID is [{GridManager.Instance.RedInGrid}].");
    }

    /// <summary>
    /// Determines the next building type to place by checking the list of placed buildings.
    /// Order is: if no Generator exists -> Generator; else if no Cannon exists -> Cannon; else if no Barracks exists -> Barrack;
    /// else return Generator.
    /// </summary>
    BuildingType DetermineNextBuilding()
    {
        List<BuildingSlot.BuildingRecord> records = BuildingSlot.GetAllBuildingsOnBoard();
        bool hasGenerator = records.Any(r => r.buildingType == BuildingType.Generator);
        bool hasCannon = records.Any(r => r.buildingType == BuildingType.Cannon);
        bool hasBarracks = records.Any(r => r.buildingType == BuildingType.Barrack);

        if (!hasGenerator)
            return BuildingType.Generator;
        else if (!hasCannon)
            return BuildingType.Cannon;
        else if (!hasBarracks)
            return BuildingType.Barrack;
        else
            return BuildingType.Generator;
    }

    /// <summary>
    /// This coroutine continually recalculates the destination cell by rechecking the list of buildings.
    /// It moves the cursor step-by-step (with animation) toward that destination.
    /// Once reached, it places the appropriate building, then exits the grid and calls ReactivatePlayer.
    /// </summary>
    IEnumerator AutoProcess()
    {
        buildingProcessTriggered = true;

        // PART 1: Recalculate destination until current cell equals destination.
        Vector2Int currentCoords = new Vector2Int(curRow, curCol);
        Vector2Int destCoord = CalculateDestination();
        while (!(curRow == destCoord.x && curCol == destCoord.y))
        {
            List<Vector2Int> pathToDestination = CalculatePath(new Vector2Int(curRow, curCol), destCoord);
            yield return StartCoroutine(MovePath(pathToDestination));
            currentCoords = new Vector2Int(curRow, curCol);
            destCoord = CalculateDestination();
        }
        Debug.Log($"Reached destination cell [{destCoord.x}, {destCoord.y}] for building placement.");

        // PART 2: Place the building using DetermineNextBuilding().
        GridManager gm = GridManager.Instance;
        GameObject targetCell = gm.GetCell(destCoord.x, destCoord.y);
        if (targetCell == null)
        {
            Debug.Log("Target cell not found at destination.");
            yield break;
        }
        BuildingSlot targetSlot = targetCell.GetComponent<BuildingSlot>();
        if (targetSlot == null)
        {
            Debug.Log("No BuildingSlot component found at target cell.");
            yield break;
        }
        if (targetSlot.HasBuilding())
        {
            Debug.Log("Target cell already contains a building.");
            yield break;
        }
        BuildingType buildType = DetermineNextBuilding();
        targetSlot.PlaceBuilding(Red, buildType);
        Debug.Log($"Placed {buildType} at cell [{destCoord.x}, {destCoord.y}].");

        // PART 3: Exit the grid step-by-step (move left until column 0)
        List<Vector2Int> exitPath = CalculatePath(new Vector2Int(curRow, curCol), new Vector2Int(curRow, 0));
        yield return StartCoroutine(MovePath(exitPath));
        Debug.Log($"RedGridCursor exited the grid at cell [{curRow}, {curCol}].");

        // Reactivate the main player and reset auto process flags.
        ReactivatePlayer();
    }

    /// <summary>
    /// Calculates a destination cell around the nearest flag by checking neighboring candidates.
    /// It now goes through all flags sorted by distance and picks the first free neighbor.
    /// If none are found, it falls back to searching the entire grid for a free cell.
    /// </summary>
    Vector2Int CalculateDestination()
    {
        // Get all building records.
        List<BuildingSlot.BuildingRecord> allRecords = BuildingSlot.GetAllBuildingsOnBoard();
        // Get flag records.
        List<BuildingSlot.BuildingRecord> flagRecords = new List<BuildingSlot.BuildingRecord>();
        foreach (var rec in allRecords)
        {
            if (rec.buildingType == BuildingType.FlagBuilding)
                flagRecords.Add(rec);
        }
        // If there are no flags, return a fallback (current cell).
        if (flagRecords.Count == 0)
        {
            Debug.Log("No flag found on board; defaulting destination to current cell.");
            return new Vector2Int(curRow, curCol);
        }

        // Order flags by Manhattan distance from the current cell.
        Vector2Int currentCoords = new Vector2Int(curRow, curCol);
        flagRecords = flagRecords.OrderBy(rec => Mathf.Abs(currentCoords.x - rec.row) + Mathf.Abs(currentCoords.y - rec.col)).ToList();

        // Candidate offsets (neighbors: left, above, right, below, then diagonals).
        Vector2Int[] candidateOffsets = new Vector2Int[]
        {
        // Immediate neighbors (distance = 1)
        new Vector2Int(0, -1),   // left
        new Vector2Int(-1, 0),   // above
        new Vector2Int(0, 1),    // right
        new Vector2Int(1, 0),    // below
        new Vector2Int(-1, -1),  // top-left
        new Vector2Int(-1, 1),   // top-right
        new Vector2Int(1, -1),   // bottom-left
        new Vector2Int(1, 1),    // bottom-right

        // Additional offsets (distance = 2 and mixed distances)
        new Vector2Int(0, -2),   // two cells to the left
        new Vector2Int(-2, 0),   // two cells above
        new Vector2Int(0, 2),    // two cells to the right
        new Vector2Int(2, 0),    // two cells below
        new Vector2Int(-2, -1),  // two cells above and one to the left
        new Vector2Int(-1, -2),  // one cell above and two to the left
        new Vector2Int(-2, 1),   // two cells above and one to the right
        new Vector2Int(-1, 2),   // one cell above and two to the right
        new Vector2Int(2, -1),   // two cells below and one to the left
        new Vector2Int(1, -2),   // one cell below and two to the left
        new Vector2Int(2, 1),    // two cells below and one to the right
        new Vector2Int(1, 2)     // one cell below and two to the right
        };


        // For each flag (starting with the nearest), try to find a valid candidate neighbor.
        foreach (var flag in flagRecords)
        {
            foreach (Vector2Int offset in candidateOffsets)
            {
                Vector2Int candidate = new Vector2Int(flag.row + offset.x, flag.col + offset.y);
                if (candidate.x >= 0 && candidate.x < rows && candidate.y >= 0 && candidate.y < cols)
                {
                    if (cells[candidate.x, candidate.y] != null)
                    {
                        BuildingSlot slot = cells[candidate.x, candidate.y].GetComponent<BuildingSlot>();
                        if (slot != null && !slot.HasBuilding())
                        {
                            return candidate;
                        }
                    }
                }
            }
        }

        // Fallback: if no candidate was found around any flag, search the entire grid for a free cell.
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (cells[r, c] != null)
                {
                    BuildingSlot slot = cells[r, c].GetComponent<BuildingSlot>();
                    if (slot != null && !slot.HasBuilding())
                    {
                        return new Vector2Int(r, c);
                    }
                }
            }
        }

        // Fallback: return current cell if nothing else is free.
        Debug.Log("No valid free cell found on board; defaulting destination to current cell.");
        return currentCoords;
    }

    /// <summary>
    /// Calculates a simple path from start to end (first horizontal then vertical movement).
    /// </summary>
    List<Vector2Int> CalculatePath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;
        while (current.y != end.y)
        {
            current = new Vector2Int(current.x, current.y + (end.y > current.y ? 1 : -1));
            path.Add(current);
        }
        while (current.x != end.x)
        {
            current = new Vector2Int(current.x + (end.x > current.x ? 1 : -1), current.y);
            path.Add(current);
        }
        return path;
    }

    /// <summary>
    /// Moves the cursor along a list of grid coordinates sequentially.
    /// </summary>
    IEnumerator MovePath(List<Vector2Int> path)
    {
        foreach (Vector2Int coord in path)
        {
            yield return StartCoroutine(MoveToCellSequentially(coord.x, coord.y));
        }
    }

    /// <summary>
    /// Coroutine to move from the current cell to a target cell (one step) with animation.
    /// </summary>
    IEnumerator MoveToCellSequentially(int targetRow, int targetCol)
    {
        yield return StartCoroutine(AnimateExitCell(curRow, curCol));
        targetRow = Mathf.Clamp(targetRow, 0, rows - 1);
        targetCol = Mathf.Clamp(targetCol, 0, cols - 1);
        curRow = targetRow;
        curCol = targetCol;
        hasJoined = true;
        yield return new WaitForSeconds(joinDelay);
        yield return StartCoroutine(AnimateEnterCell(curRow, curCol));
        occupied[curRow, curCol] = false;
        Debug.Log($"Moved to cell [{curRow}, {curCol}].");
    }

    /// <summary>
    /// Coroutine to animate entering a cell with a small delay.
    /// </summary>
    IEnumerator AnimateEnterCell(int r, int c)
    {
        yield return new WaitForSeconds(0.1f);
        occupied[r, c] = true;
        GridCellTint tint = cells[r, c].GetComponent<GridCellTint>();
        if (tint != null)
            tint.Enter(tintOwner);
        Debug.Log($"Entered cell [{r}, {c}].");
    }

    /// <summary>
    /// Coroutine to animate exiting a cell with a small delay.
    /// </summary>
    IEnumerator AnimateExitCell(int r, int c)
    {
        yield return new WaitForSeconds(0.1f);
        occupied[r, c] = false;
        GridCellTint tint = cells[r, c].GetComponent<GridCellTint>();
        if (tint != null)
            tint.Exit(tintOwner);
        Debug.Log($"Exited cell [{r}, {c}].");
    }

    /// <summary>
    /// Reactivates the player (for example, if the cursor reaches the grid edge) similar to GridCursor.
    /// </summary>
    void ReactivatePlayer()
    {
        StartCoroutine(AnimateExitCell(curRow, curCol));
        hasJoined = false;
        // Reset auto process flags so that next time the red bot enters, it can trigger auto process again.
        autoProcessStarted = false;
        buildingProcessTriggered = false;
        generatorBuilt = false;

        // Reset redInGrid flags on all grid cells.
        if (GridManager.Instance != null && GridManager.Instance.gridParent != null)
        {
            GridCellActivation[] activations = GridManager.Instance.gridParent.GetComponentsInChildren<GridCellActivation>();
            foreach (GridCellActivation act in activations)
            {
                act.redInGrid = false;
            }
        }

        // Optionally: reset all cell colors.
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (cells[r, c] != null)
                {
                    GridCellTint tint = cells[r, c].GetComponent<GridCellTint>();
                    if (tint != null)
                    {
                        tint.Exit(tintOwner);
                    }
                }
            }
        }

        string name = tintOwner == Red ? "Red" : "Blue";
        GameObject ent = null;
        foreach (var o in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (o.name == name)
            {
                ent = o;
                break;
            }
        }
        if (ent == null || ent.activeInHierarchy)
            return;
        ent.SetActive(true);
        float w = cells[curRow, curCol].transform.localScale.x;
        ent.transform.position = cells[curRow, curCol].transform.position + Vector3.left * w * 1.5f;
    }
}
