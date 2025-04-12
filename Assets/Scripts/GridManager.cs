using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Use gridCount to set both rows and columns (square grid).
    public int gridCount = 6;
    public GameObject gridCellPrefab;
    public Transform gridParent;

    private float cellSize;

    void Start()
    {
        CreateRightSquareGrid();
    }

    void CreateRightSquareGrid()
    {
        // 1) Get camera boundaries in world space.
        Vector3 cameraTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        Vector3 cameraBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));

        // 2) Calculate total camera width.
        float cameraWidth = cameraTopRight.x - cameraBottomLeft.x;

        // 3) Decide how large each square area is (half of camera width in this example).
        float squareSize = cameraWidth / 2f;

        // 4) For the right square, define boundaries:
        float squareRight = cameraTopRight.x;
        float squareLeft = squareRight - squareSize;
        float squareTop = cameraTopRight.y;
        // The bottom would be squareTop - squareSize (not explicitly used below).

        // 5) The origin is the top-left corner of this right-side square.
        Vector3 origin = new Vector3(squareLeft, squareTop, 0f);

        // 6) Compute each cell’s size.
        cellSize = squareSize / gridCount;

        // 7) Build the grid from top-left to bottom-right, placing each cell
        // so that its *top-left corner* aligns with the correct position.
        for (int row = 0; row < gridCount; row++)
        {
            for (int col = 0; col < gridCount; col++)
            {
                // This is where the top-left corner should be.
                Vector3 topLeftPos = new Vector3(
                    origin.x + col * cellSize,  // left edge
                    origin.y - row * cellSize,  // top edge
                    0f
                );

                // Because the pivot is in the center of the sprite,
                // we shift the cell's transform by half a cell to align the top-left corner.
                Vector3 pivotOffset = new Vector3(cellSize * 0.5f, -cellSize * 0.5f, 0f);
                Vector3 finalPos = topLeftPos + pivotOffset;

                GameObject cell = Instantiate(gridCellPrefab, finalPos, Quaternion.identity, gridParent);
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1f);

                cell.name = $"GridCell_{row}_{col}";
            }
        }
        GameObject selector = GameObject.Find("GridSelector");
        if (selector != null)
            selector.SetActive(true);
    }
}
