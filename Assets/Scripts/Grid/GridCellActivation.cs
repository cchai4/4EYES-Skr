using UnityEngine;

public class GridCellActivation : MonoBehaviour
{
    [Header("Sound Effects")]
    public AudioClip enterSound;    // Sound played when Red/Blue enters
    [Range(0f, 1f)]
    public float enterVolume = 1f;

    public AudioClip exitSound;     // Sound played when Red/Blue exits
    [Range(0f, 1f)]
    public float exitVolume = 1f;

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
            // Play "enter" sound if assigned
            if (enterSound != null)
                AudioSource.PlayClipAtPoint(enterSound, transform.position, enterVolume);

            // Original logic
            other.gameObject.SetActive(false);
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
            // Play "enter" sound if assigned
            if (enterSound != null)
                AudioSource.PlayClipAtPoint(enterSound, transform.position, enterVolume);

            // Original logic
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

    void OnTriggerExit2D(Collider2D other)
    {
        // Example tags: "Red" or "Blue"
        if (other.CompareTag("Red") || other.CompareTag("Blue"))
        {
            // Play "exit" sound if assigned
            if (exitSound != null)
                AudioSource.PlayClipAtPoint(exitSound, transform.position, exitVolume);

            Debug.Log($"{other.tag} exited cell [{row},{col}].");
        }
    }
}
