using UnityEngine;

public class GridSelector : MonoBehaviour
{
    public Color highlightColor = Color.yellow;

    private GameObject[,] cells;
    private int rows = 0, cols = 0;
    private int curRow = 0, curCol = 0;

    private SpriteRenderer lastRenderer;
    private Color lastOriginalColor;

    void Start() => StartCoroutine(InitWhenGridReady());

    System.Collections.IEnumerator InitWhenGridReady()
    {
        // Wait until GridManager has spawned its children
        Transform gridParent = null;
        do
        {
            gridParent = GameObject.FindFirstObjectByType<GridManager>()?.gridParent;
            yield return null; // wait one frame
        } while (gridParent == null || gridParent.childCount == 0);

        // ---- normal initialization ----
        int count = gridParent.childCount;
        rows = cols = Mathf.RoundToInt(Mathf.Sqrt(count));
        cells = new GameObject[rows, cols];

        foreach (Transform cell in gridParent)
        {
            string[] parts = cell.name.Split('_');
            if (parts.Length == 3 &&
                int.TryParse(parts[1], out int r) &&
                int.TryParse(parts[2], out int c) &&
                r < rows && c < cols)
            {
                cells[r, c] = cell.gameObject;
            }
        }

        curRow = 0;
        curCol = 0;
        TryHighlight(curRow, curCol);
    }

    void Update()
    {
        if (rows == 0 || cols == 0)   // grid not ready yet
            return;
        int prevRow = curRow;
        int prevCol = curCol;

        if (Input.GetKeyDown(KeyCode.I)) curRow--;
        if (Input.GetKeyDown(KeyCode.K)) curRow++;
        if (Input.GetKeyDown(KeyCode.J)) curCol--;
        if (Input.GetKeyDown(KeyCode.L)) curCol++;

        // Clamp to grid bounds
        curRow = Mathf.Clamp(curRow, 0, rows - 1);
        curCol = Mathf.Clamp(curCol, 0, cols - 1);

        if (curRow != prevRow || curCol != prevCol)
        {
            TryHighlight(curRow, curCol);
        }
    }

    void TryHighlight(int row, int col)
    {
        if (row < 0 || col < 0 || row >= rows || col >= cols)
        {
            Debug.LogWarning($"Attempted to highlight out-of-bounds cell at [{row}, {col}]");
            return;
        }

        GameObject cell = cells[row, col];
        if (cell == null)
        {
            Debug.LogWarning($"Cell at [{row}, {col}] is null.");
            return;
        }

        Highlight(cell);
    }

    void Highlight(GameObject cell)
    {
        if (lastRenderer != null)
            lastRenderer.color = lastOriginalColor;

        lastRenderer = cell.GetComponent<SpriteRenderer>();
        if (lastRenderer != null)
        {
            lastOriginalColor = lastRenderer.color;
            lastRenderer.color = highlightColor;
        }
        else
        {
            Debug.LogWarning($"No SpriteRenderer found on {cell.name}");
        }
    }
}
