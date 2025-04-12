using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridSize = 6;                          // Creates a gridSize x gridSize grid
    public GameObject gridCellPrefab;                // Assign in Inspector
    public Transform gridParent;                     // Assign in Inspector (for organization)

    void Start()
    {
        CreateRightSquareGrid();
    }

    void CreateRightSquareGrid()
    {
        // Get screen space in world coordinates
        Vector3 camTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
        Vector3 camMid = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

        float gridWidth = camTopRight.x - camMid.x;
        float cellSize = gridWidth / gridSize;

        Vector3 origin = new Vector3(camMid.x, camTopRight.y, 0);

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Vector3 pos = origin + new Vector3(
                    col * cellSize + cellSize / 2f,
                    -row * cellSize - cellSize / 2f,
                    0f);

                GameObject cell = Instantiate(gridCellPrefab, pos, Quaternion.identity, gridParent);
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1f);
                cell.name = $"GridCell_{row}_{col}";

                var activation = cell.GetComponent<GridCellActivation>();
                if (activation != null)
                    activation.SetCoords(row, col);
            }
        }

        // Optional: activate GridSelector (if you're still using one)
        GameObject selector = GameObject.Find("GridSelector");
        if (selector != null)
            selector.SetActive(true);
    }
}
