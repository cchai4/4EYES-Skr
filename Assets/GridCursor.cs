using UnityEngine;
using static GridCellTint; // For ColorType enum
using System.Collections;

public class GridCursor : MonoBehaviour
{
    public void ForcePlaceAt(int row, int col)
    {
        // Example: Calculate the world position from grid coordinates.
        // You may need to adjust these calculations based on your grid design.
        Vector2 newPosition = new Vector2(col, row);
        transform.position = newPosition;
    }

    [Header("Controls")]
    public KeyCode upKey;    // For red: W, for blue: UpArrow
    public KeyCode downKey;  // For red: S, for blue: DownArrow
    public KeyCode leftKey;  // For both, reactivation triggered when pressed in leftmost cell
    public KeyCode rightKey; // For red: D, for blue: RightArrow

    [Header("Cursor Settings")]
    public ColorType tintOwner;   // For Player1Cursor: Red; for Player2Cursor: Blue.
    public int startRow = 0, startCol = 0;

    // Indicates whether the entity has joined the grid (and so the cursor tints cells)
    [HideInInspector] public bool hasJoined = false;

    // Shared grid arrays (initialized only once)
    private static GameObject[,] cells;
    private static bool[,] occupied;
    private static int rows, cols;

    // The current cell indices for this cursor
    private int curRow, curCol;

    void Start() => StartCoroutine(InitWhenGridReady());

    IEnumerator InitWhenGridReady()
    {
        // Wait until GridManager has finished building the grid.
        GridManager gm = null;
        Transform gp = null;
        while (gp == null || gp.childCount == 0)
        {
            gm = Object.FindFirstObjectByType<GridManager>();
            gp = gm ? gm.gridParent : null;
            yield return null;
        }

        // Initialize shared arrays only once.
        if (cells == null)
        {
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
                    if (r < rows && c < cols)
                        cells[r, c] = t.gameObject;
                }
            }
        }

        // Find the nearest free cell starting at (startRow, startCol).
        bool placed = false;
        for (int r = 0; r < rows && !placed; r++)
        {
            for (int c = 0; c < cols && !placed; c++)
            {
                int tryRow = (startRow + r) % rows;
                int tryCol = (startCol + c) % cols;
                if (!occupied[tryRow, tryCol])
                {
                    curRow = tryRow;
                    curCol = tryCol;
                    EnterCell(curRow, curCol);
                    placed = true;
                }
            }
        }
        if (!placed)
            Debug.LogError("GridCursor: No available cell found!");
    }

    void Update()
    {
        if (cells == null) return;

        // Reactivation logic: if leftKey is pressed while at the leftmost column, reactivate the underlying entity.
        if (Input.GetKeyDown(leftKey) && curCol == 0)
        {
            ReactivatePlayer();
            return; // Skip normal movement.
        }

        int newRow = curRow;
        int newCol = curCol;
        if (Input.GetKeyDown(upKey)) newRow--;
        if (Input.GetKeyDown(downKey)) newRow++;
        if (Input.GetKeyDown(leftKey)) newCol--; // normal left movement (if not reactivating)
        if (Input.GetKeyDown(rightKey)) newCol++;

        newRow = Mathf.Clamp(newRow, 0, rows - 1);
        newCol = Mathf.Clamp(newCol, 0, cols - 1);

        // Only move if the destination cell is not occupied.
        if ((newRow != curRow || newCol != curCol) && !occupied[newRow, newCol])
        {
            ExitCell(curRow, curCol);
            EnterCell(newRow, newCol);
            curRow = newRow;
            curCol = newCol;
        }
    }

    void EnterCell(int r, int c)
    {
        occupied[r, c] = true;
        if (hasJoined)
        {
            var tint = cells[r, c].GetComponent<GridCellTint>();
            if (tint != null)
                tint.Enter(tintOwner);
        }
    }

    void ExitCell(int r, int c)
    {
        occupied[r, c] = false;
        if (hasJoined)
        {
            var tint = cells[r, c].GetComponent<GridCellTint>();
            if (tint != null)
                tint.Exit(tintOwner);
        }
    }

    // Called externally (for example, from GridCellActivation) to force the cursor to a specific cell.
    public void ForcePlaceAt(int r, int c)
    {
        if (cells == null || occupied == null) return;
        ExitCell(curRow, curCol);
        curRow = Mathf.Clamp(r, 0, rows - 1);
        curCol = Mathf.Clamp(c, 0, cols - 1);
        if (!occupied[curRow, curCol])
        {
            hasJoined = true;
            EnterCell(curRow, curCol);
        }
        else
        {
            // If the cell is already occupied, reset the state.
            ExitCell(curRow, curCol);
            hasJoined = true;
            EnterCell(curRow, curCol);
            Debug.LogWarning("ForcePlaceAt: Target cell already occupied; state reset.");
        }
        // Release the occupancy flag so the same cell can be re-converted later.
        occupied[curRow, curCol] = false;
    }

    // Reactivates the underlying entity (Red or Blue) when the cursor is at the leftmost cell 
    // and the left key is pressed.
    void ReactivatePlayer()
    {
        ExitCell(curRow, curCol);
        hasJoined = false;

        // Determine the entity name based on tintOwner.
        string entityName = tintOwner == ColorType.Red ? "Red" : "Blue";
        GameObject entity = null;
        GameObject[] allObjs = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjs)
        {
            if (obj.name == entityName)
            {
                entity = obj;
                break;
            }
        }
        if (entity == null)
        {
            Debug.LogWarning($"ReactivatePlayer: Could not find the {entityName} entity!");
            return;
        }
        if (entity.activeInHierarchy)
            return;  // Already active.

        entity.SetActive(true);
        float cellWidth = cells[curRow, curCol].transform.localScale.x;
        Vector3 cellPos = cells[curRow, curCol].transform.position;
        // For both Red and Blue, we now use left offset.
        float offsetMultiplier = 1.5f;  // Change as needed.
        entity.transform.position = cellPos + Vector3.left * (cellWidth * offsetMultiplier);

        Debug.Log($"Reactivated {entityName} at cell [{curRow},{curCol}] with offset {cellWidth * offsetMultiplier}");
    }
}
