using UnityEngine;

public class newGridManager : MonoBehaviour
{
    public int gridSize = 6;             // Creates a gridSize x gridSize grid.
    public GameObject gridCellPrefab;    // Assign in Inspector
    public Transform gridParent;         // Assign in Inspector

    private GameObject[,] cells;
    private int rows, cols;

    void Start()
    {
        CreateRightSquareGrid();
    }

    public static newGridManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void CreateRightSquareGrid()
    {
        Vector3 camTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
        Vector3 camMid = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        float gridWidth = camTopRight.x - camMid.x;
        float cellSize = gridWidth / gridSize;

        Vector3 origin = new Vector3(camMid.x, camTopRight.y, 0);

        rows = gridSize;
        cols = gridSize;
        cells = new GameObject[rows, cols];

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Vector3 pos = origin + new Vector3(
                    col * cellSize + cellSize / 2f,
                    -row * cellSize - cellSize / 2f,
                    0f);

                // Instantiate each cell
                GameObject cell = Instantiate(gridCellPrefab, pos, Quaternion.identity, gridParent);
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1f);
                cell.name = $"GridCell_{row}_{col}";

                // 1) If there's a GridCellActivation script, set its coords
                var activation = cell.GetComponent<GridCellActivation>();
                if (activation != null)
                {
                    activation.SetCoords(row, col);
                }

                // 2) If there's a TerritoryTint script, set its coords
                var tint = cell.GetComponent<TerritoryTint>();
                if (tint != null)
                {
                    tint.SetCoords(row, col);
                }

                cells[row, col] = cell;
            }
        }
    }

    /// <summary>
    /// Returns the grid cell at (row,col), or null if out of bounds
    /// </summary>
    public GameObject GetCell(int row, int col)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols)
            return null;
        return cells[row, col];
    }

    public int GetRowCount() => rows;
    public int GetColCount() => cols;

    /// <summary>
    /// Refreshes the territory tints for all grid cells.
    /// </summary>
    public void RefreshAllTerritories()
    {
        Debug.Log("GridManager: RefreshAllTerritories called.");

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cell = cells[r, c];
                if (cell != null)
                {
                    Debug.Log($"Refreshing territory on cell at row {r}, col {c} (named {cell.name}).");
                    var tint = cell.GetComponent<TerritoryTint>();
                    if (tint != null)
                    {
                        tint.RefreshColor();
                    }
                    else
                    {
                        Debug.LogWarning($"Cell {cell.name} does not have a TerritoryTint component.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Cell at row {r}, col {c} is null.");
                }
            }
        }
    }

    /// <summary>
    /// Returns the first GridCellActivation that has redInGrid set to true.
    /// </summary>
    public newGridCellActivation GetRedActivatedCell()
    {
        newGridCellActivation[] activations = gridParent.GetComponentsInChildren<newGridCellActivation>();
        foreach (var act in activations)
        {
            if (act.redInGrid)
                return act;
        }
        return null;
    }

    /// <summary>
    /// Returns true if any GridCellActivation under gridParent has redInGrid set to true.
    /// </summary>
    public bool RedInGrid
    {
        get
        {
            newGridCellActivation[] activations = gridParent.GetComponentsInChildren<newGridCellActivation>();
            foreach (var act in activations)
            {
                if (act.redInGrid)
                    return true;
            }
            return false;
        }
    }
}
