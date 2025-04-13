using UnityEngine;

public class GridCellActivation : MonoBehaviour
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
                }
            }
        }
    }
}
