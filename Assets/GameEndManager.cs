using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class GameEndManager : MonoBehaviour
{
    [Header("UI Text for Displaying End Game Results")]
    [SerializeField] private Text resultText;

   
    public void OnGameEnd()
    {
        Debug.Log("GameEndManager: OnGameEnd() called -- starting territory count...");

        
        GridCursor[] cursors = FindObjectsByType<GridCursor>(FindObjectsSortMode.None);
        if (cursors.Length == 0)
        {
            Debug.LogError("GameEndManager: No GridCursor found—cannot count territory!");
            return;
        }

        int rowCount = cursors[0].TotalRows;
        int colCount = cursors[0].TotalCols;
        Debug.Log($"GameEndManager: Found {rowCount} rows and {colCount} cols from GridCursor.");

        
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

                
                TerritoryTint tint = cell.GetComponent<TerritoryTint>();
                if (tint == null)
                {
                    Debug.Log($"GameEndManager: Cell at ({r},{c}) has no TerritoryTint, skipping.");
                    continue;
                }

                
                var cellTintComp = cell.GetComponent<GridCellTint>();
                if (cellTintComp == null)
                {
                    Debug.Log($"GameEndManager: Cell({r},{c}) has no GridCellTint, skipping.");
                    continue;
                }

                
                Color finalColor = cellTintComp.baseColor;

                Debug.Log(
                    $"GameEndManager: Cell({r},{c}) => finalColor={finalColor}"
                );

                
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
                 
                    Debug.Log($"GameEndManager: Cell({r},{c}) is purple-tinted.");
                }
                else
                {
                    Debug.Log($"GameEndManager: Cell({r},{c}) did not match red/blue/purple. Possibly default/white.");
                }
            }
        }

        
        string winner = "Tie";
        if (redCount > blueCount) winner = "Red";
        else if (blueCount > redCount) winner = "Blue";

        Debug.Log($"GameEndManager: Done counting. Red={redCount}, Blue={blueCount}, Winner={winner}");

        
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

   
    private bool CompareColors(Color a, Color b, float tolerance = 0.001f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance &&
               Mathf.Abs(a.a - b.a) < tolerance;
    }
}
