using UnityEngine;
using static GridCellTint;      // for ColorType enum
using System.Collections;

public class GridCursor : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    [Header("Cursor Settings")]
    public ColorType tintOwner = ColorType.Red;
    public int startRow = 0, startCol = 0;

    /* internal */
    private GameObject[,] cells;
    private int rows, cols;
    private int curRow, curCol;

    void Start() => StartCoroutine(InitWhenGridReady());

    IEnumerator InitWhenGridReady()
    {
        // wait until GridManager exists AND has spawned children
        GridManager gm = null;
        Transform gp = null;
        while (gp == null || gp.childCount == 0)
        {
            gm = FindObjectOfType<GridManager>();
            gp = gm ? gm.gridParent : null;
            yield return null;          // wait one frame
        }

        rows = cols = Mathf.RoundToInt(Mathf.Sqrt(gp.childCount));
        cells = new GameObject[rows, cols];

        foreach (Transform t in gp)
        {
            var p = t.name.Split('_');
            if (p.Length == 3 &&
                int.TryParse(p[1], out int r) &&
                int.TryParse(p[2], out int c) &&
                r < rows && c < cols)
            {
                cells[r, c] = t.gameObject;
            }
        }

        curRow = Mathf.Clamp(startRow, 0, rows - 1);
        curCol = Mathf.Clamp(startCol, 0, cols - 1);
        EnterCell(curRow, curCol);
    }

    void Update()
    {
        // grid not ready yet?
        if (cells == null) return;

        int newRow = curRow;
        int newCol = curCol;

        if (Input.GetKeyDown(upKey)) newRow--;
        if (Input.GetKeyDown(downKey)) newRow++;
        if (Input.GetKeyDown(leftKey)) newCol--;
        if (Input.GetKeyDown(rightKey)) newCol++;

        newRow = Mathf.Clamp(newRow, 0, rows - 1);
        newCol = Mathf.Clamp(newCol, 0, cols - 1);

        if (newRow != curRow || newCol != curCol)
        {
            ExitCell(curRow, curCol);
            EnterCell(newRow, newCol);
            curRow = newRow; curCol = newCol;
        }
    }

    /* helpers -------------------------------------------------- */
    void EnterCell(int r, int c)
    {
        if (cells[r, c])
            cells[r, c].GetComponent<GridCellTint>().Enter(tintOwner);
        else
            Debug.LogWarning($"GridCursor: cells[{r},{c}] is null");
    }

    void ExitCell(int r, int c)
    {
        if (cells[r, c])
            cells[r, c].GetComponent<GridCellTint>().Exit(tintOwner);
    }
}
