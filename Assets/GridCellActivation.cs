using UnityEngine;

public class GridCellActivation : MonoBehaviour
{
    private int row, col;
    // This flag ensures the cell only activates once.
    private bool activated = false;

    /// <summary>
    /// Called from GridManager.cs to assign this cell's coordinates.
    /// </summary>
    public void SetCoords(int r, int c)
    {
        row = r;
        col = c;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If this cell has already been activated, do nothing.
        if (activated)
            return;

        // Process the collision based on the tag of the incoming object.
        if (other.CompareTag("Red"))
        {
            activated = true;
            // Deactivate the incoming entity.
            other.gameObject.SetActive(false);
            // Look for the Player1Cursor and update it.
            GameObject player1Cursor = GameObject.Find("Player1Cursor");
            if (player1Cursor != null)
            {
                var cursor = player1Cursor.GetComponent<GridCursor>();
                if (cursor != null)
                {
                    cursor.ForcePlaceAt(row, col);
                    Debug.Log($"Red touched cell [{row},{col}]. Player1Cursor joined.");
                }
            }
        }
        else if (other.CompareTag("Blue"))
        {
            activated = true;
            // Deactivate the incoming entity.
            other.gameObject.SetActive(false);
            // Look for the Player2Cursor and update it.
            GameObject player2Cursor = GameObject.Find("Player2Cursor");
            if (player2Cursor != null)
            {
                var cursor = player2Cursor.GetComponent<GridCursor>();
                if (cursor != null)
                {
                    cursor.ForcePlaceAt(row, col);
                    Debug.Log($"Blue touched cell [{row},{col}]. Player2Cursor joined.");
                }
            }
        }
    }
}
