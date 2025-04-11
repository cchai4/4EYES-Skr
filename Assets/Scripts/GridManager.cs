using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows = 6;
    public int columns = 5;
    public GameObject gridCellPrefab;    // Assign your GridCell prefab in the Inspector.
    public Transform gridParent;         // Optional parent object for organizational purposes.

    private float cellSize;

    void Start()
    {
        CreateRightSideGrid();
    }

    void CreateRightSideGrid()
    {
        // Get world positions for the right half of the screen.
        Vector3 midPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
        Vector3 bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0));

        // Define the grid area: from midPoint.x to topRight.x.
        float gridWidth = topRight.x - midPoint.x;
        float gridHeight = topRight.y - bottomRight.y;

        // Calculate cell size based on grid width and number of columns.
        cellSize = gridWidth / columns;

        // Define the origin of the grid (top-left corner of the right half).
        Vector3 origin = new Vector3(midPoint.x, topRight.y, 0);

        // Instantiate grid cells.
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculate cell position. Cells are arranged so that increasing row moves downward.
                Vector3 cellPos = origin + new Vector3(col * cellSize, -row * cellSize, 0);

                GameObject cell = Instantiate(gridCellPrefab, cellPos, Quaternion.identity, gridParent);
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1);
                cell.name = $"GridCell_{row}_{col}";

                // For the leftmost column (col == 0), add a BoxCollider2D if not already present.
                if (col == 0)
                {
                    BoxCollider2D col2d = cell.GetComponent<BoxCollider2D>();
                    if (col2d == null)
                        cell.AddComponent<BoxCollider2D>();

                    // Optionally, you can also add a script for detecting player entry.
                    if (cell.GetComponent<GridCellDetector>() == null)
                        cell.AddComponent<GridCellDetector>();
                }
            }
        }
    }
}
