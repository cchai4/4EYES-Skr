using UnityEngine;

public class newGridCellActivation : MonoBehaviour
{
    private int row, col;

    /// <summary>
    /// Called by GridManager to assign the correct (row, col) at creation.
    /// </summary>
    public void SetCoords(int r, int c)
    {
        row = r;
        col = c;
    }

    /// <summary>
    /// Returns the grid cell coordinates as a Vector2Int.
    /// </summary>
    public Vector2Int GetCellCoords()
    {
        return new Vector2Int(row, col);
    }
    public bool redInGrid = false;
    public bool blueInGrid = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Example tags: "Red" or "Blue"
        if (other.CompareTag("Red"))
        {
            other.gameObject.SetActive(false); // or however you handle it
            GameObject player1Cursor = GameObject.Find("Player1Cursor");
            if (player1Cursor != null)
            {
                var cursor = player1Cursor.GetComponent<GridCursor>();
                if (cursor != null)
                {
                    cursor.ForcePlaceAt(row, col);
                    Debug.Log($"Red touched cell [{row},{col}]. Player1Cursor updated.");
                    redInGrid = true;
                    Debug.Log(redInGrid);
                }
            }
        }
        else if (other.CompareTag("Blue"))
        {
            other.gameObject.SetActive(false);
            GameObject player2Cursor = GameObject.Find("Player2Cursor");
            if (player2Cursor != null)
            {
                var cursor = player2Cursor.GetComponent<GridCursor>();
                if (cursor != null)
                {
                    cursor.ForcePlaceAt(row, col);
                    Debug.Log($"Blue touched cell [{row},{col}]. Player2Cursor updated.");
                    blueInGrid = true;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Red"))
        {
            redInGrid = false;
            Debug.Log($"Red left cell [{row},{col}]. redInGrid set to false.");
        }
        else if (other.CompareTag("Blue"))
        {
            blueInGrid = false;
            Debug.Log($"Blue left cell [{row},{col}]. blueInGrid set to false.");
        }
    }
}
