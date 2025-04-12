using UnityEngine;
using static GridCellTint; // for ColorType enum
using System.Collections;

public class GridCursor : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    [HideInInspector] public bool hasJoined = false;

    [Header("Cursor Settings")]
    public ColorType tintOwner = ColorType.Red;
    public int startRow = 0, startCol = 0;

    private static GameObject[,] cells;
    private static bool[,] occupied;
    private static int rows, cols;

    private int curRow, curCol;

    void Start() => StartCoroutine(InitWhenGridReady());

    IEnumerator InitWhenGridReady()
    {
        // Wait for grid to be ready
        GridManager gm = null;
        Transform gp = null;
        while (gp == null || gp.childCount == 0)
        {
            gm = Object.FindFirstObjectByType<GridManager>();
            gp = gm ? gm.gridParent : null;
            yield return null;
        }

        // Initialize grid only once
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

        // Search for the nearest unoccupied cell
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

        int newRow = curRow;
        int newCol = curCol;

        if (Input.GetKeyDown(upKey)) newRow--;
        if (Input.GetKeyDown(downKey)) newRow++;
        if (Input.GetKeyDown(leftKey)) newCol--;
        if (Input.GetKeyDown(rightKey)) newCol++;

        newRow = Mathf.Clamp(newRow, 0, rows - 1);
        newCol = Mathf.Clamp(newCol, 0, cols - 1);

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
            if (tint) tint.Enter(tintOwner);
        }
    }

    void ExitCell(int r, int c)
    {
        occupied[r, c] = false;
        if (hasJoined)
        {
            var tint = cells[r, c].GetComponent<GridCellTint>();
            if (tint) tint.Exit(tintOwner);
        }
    }

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
            Debug.LogWarning("Target cell already occupied!");
        }
    }


}
