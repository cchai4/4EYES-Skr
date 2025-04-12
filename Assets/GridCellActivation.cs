using UnityEngine;

public class GridCellActivation : MonoBehaviour
{
    private int row, col;
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
        if (activated)
            return;

        // Check if a Red or Blue entity hit this cell.
        if (other.CompareTag("Red"))
        {
            activated = true;
            other.gameObject.SetActive(false);

            var cursor = GameObject.Find("Player1Cursor")?.GetComponent<GridCursor>();
            if (cursor != null)
            {
                cursor.ForcePlaceAt(row, col);
                Debug.Log($"Red touched cell [{row},{col}]. Player1Cursor joined.");
            }
        }

        else if (other.CompareTag("Blue"))
        {
            activated = true;
            other.gameObject.SetActive(false);

            var cursor = GameObject.Find("Player2Cursor")?.GetComponent<GridCursor>();
            if (cursor != null)
            {
                cursor.ForcePlaceAt(row, col);
                Debug.Log($"Blue touched cell [{row},{col}]. Player2Cursor joined.");
            }
        }
    }
}
