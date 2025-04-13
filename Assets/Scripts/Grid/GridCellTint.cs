using UnityEngine;

/// <summary>
/// Handles cursor highlights on a single cell. If no cursor is present, 
/// the cell reverts to the "baseColor" set by TerritoryTint.
/// </summary>
public class GridCellTint : MonoBehaviour
{
    // The territory or “base” color. TerritoryTint sets this, 
    // and we only apply it if no cursor is on the cell.
    [HideInInspector] public Color baseColor = Color.white;

    private SpriteRenderer sr;
    private int redCount = 0;
    private int blueCount = 0;

    [Header("Cursor Highlight Colors")]
    public Color redHighlight = new Color(1f, 0.6f, 0.6f);
    public Color blueHighlight = new Color(0.6f, 0.6f, 1f);
    public Color purpleHighlight = new Color(1f, 0.5f, 1f);

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called when a player's cursor enters this cell.
    /// </summary>
    public void Enter(ColorType who)
    {
        if (who == ColorType.Red) redCount++;
        else if (who == ColorType.Blue) blueCount++;
        UpdateHighlight();
    }

    /// <summary>
    /// Called when that player's cursor leaves this cell.
    /// </summary>
    public void Exit(ColorType who)
    {
        if (who == ColorType.Red) redCount = Mathf.Max(0, redCount - 1);
        else if (who == ColorType.Blue) blueCount = Mathf.Max(0, blueCount - 1);
        UpdateHighlight();
    }

    /// <summary>
    /// Set the cell's territory/base color. If no cursor is present, we apply it immediately.
    /// Otherwise, we keep showing the highlight color.
    /// </summary>
    public void SetBaseColor(Color c)
    {
        baseColor = c;
        if (redCount == 0 && blueCount == 0)
        {
            sr.color = baseColor;
        }
    }

    /// <summary>
    /// Re-check the highlight color or revert to base color if no cursor.
    /// </summary>
    void UpdateHighlight()
    {
        if (redCount > 0 && blueCount > 0)
        {
            sr.color = purpleHighlight;
        }
        else if (redCount > 0)
        {
            sr.color = redHighlight;
        }
        else if (blueCount > 0)
        {
            sr.color = blueHighlight;
        }
        else
        {
            sr.color = baseColor;
        }
    }

    // For reference: same enum used in BuildingSlot or other scripts
    public enum ColorType { Red, Blue }
}
