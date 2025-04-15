using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this script to a GameObject in your scene.
/// Make sure to set "resultText" in the Inspector to a UI Text.
/// Call OnGameEnd() once your timer runs out.
/// </summary>
public class GameEndManager : MonoBehaviour
{
    [Header("UI Text for Displaying End Game Results")]
    [SerializeField] private Text resultText;

    /// <summary>
    /// Call this method when time is up or the game ends.
    /// It counts the territory for Red vs. Blue and displays a result string.
    /// </summary>
    public void OnGameEnd()
    {
        Debug.Log("GameEndManager: OnGameEnd() called -- starting territory count...");

        // 1) Find how many rows/cols we have (via GridCursor or directly from your GridManager).
        GridCursor[] cursors = FindObjectsByType<GridCursor>(FindObjectsSortMode.None);
        if (cursors.Length == 0)
        {
            Debug.LogError("GameEndManager: No GridCursor found—cannot count territory!");
            return;
        }

        int rowCount = cursors[0].TotalRows;
        int colCount = cursors[0].TotalCols;
        Debug.Log($"GameEndManager: Found {rowCount} rows and {colCount} cols from GridCursor.");

        // 2) Loop over all cells and check territory
        int redCount = 0;
        int blueCount = 0;

        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < colCount; c++)
            {
                GameObject cell = GridManager.Instance.GetCell(r, c);
                if (!cell)
                {
                    Debug.Log($"GameEndManager: Cell at ({r},{c}) is null, skipping.");
                    continue;
                }

                // This script holds the references to redTint, blueTint, etc.
                TerritoryTint tint = cell.GetComponent<TerritoryTint>();
                if (tint == null)
                {
                    Debug.Log($"GameEndManager: Cell at ({r},{c}) has no TerritoryTint, skipping.");
                    continue;
                }

                // But the cell's *actual* color is stored in the GridCellTint component:
                var cellTintComp = cell.GetComponent<GridCellTint>();
                if (cellTintComp == null)
                {
                    Debug.Log($"GameEndManager: Cell({r},{c}) has no GridCellTint, skipping.");
                    continue;
                }

                // The final color is cellTintComp.baseColor (set by TerritoryTint.RefreshColor).
                Color finalColor = cellTintComp.baseColor;

                Debug.Log(
                    $"GameEndManager: Cell({r},{c}) => finalColor={finalColor}"
                );

                // Compare finalColor with red/blue/purple from TerritoryTint
                if (CompareColors(finalColor, tint.redTint))
                {
                    redCount++;
                    Debug.Log($"GameEndManager: Cell({r},{c}) matched redTint -> redCount={redCount}");
                }
                else if (CompareColors(finalColor, tint.blueTint))
                {
                    blueCount++;
                    Debug.Log($"GameEndManager: Cell({r},{c}) matched blueTint -> blueCount={blueCount}");
                }
                else if (CompareColors(finalColor, tint.purpleTint))
                {
                    // If you want to handle purple coverage, do it here:
                    // e.g. count for both sides or skip, etc.
                    Debug.Log($"GameEndManager: Cell({r},{c}) is purple-tinted.");
                }
                else
                {
                    Debug.Log($"GameEndManager: Cell({r},{c}) did not match red/blue/purple. Possibly default/white.");
                }
            }
        }

        // 3) Decide who has more territory
        string winner = "Tie";
        if (redCount > blueCount) winner = "Red";
        else if (blueCount > redCount) winner = "Blue";

        Debug.Log($"GameEndManager: Done counting. Red={redCount}, Blue={blueCount}, Winner={winner}");

        // 4) Display results in the UI
        if (resultText != null)
        {
            resultText.text = $"Red:{redCount}    Blue:{blueCount}\nWinner: {winner}";
            Debug.Log($"GameEndManager: Updated 'resultText' to '{resultText.text}'.");
        }
        else
        {
            Debug.LogWarning("GameEndManager: 'resultText' is not assigned in Inspector. Cannot display results!");
        }
    }

    /// <summary>
    /// Checks if two Colors are (close to) identical, ignoring small floating?point differences.
    /// </summary>
    private bool CompareColors(Color a, Color b, float tolerance = 0.001f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance &&
               Mathf.Abs(a.a - b.a) < tolerance;
    }
}
