using UnityEngine;
using UnityEngine.UI;
using static GridCellTint;

/// <summary>
/// Singleton that keeps the left/right building buttons visible
/// and simply changes their highlight color while a player
/// is in Building?Select mode.
/// </summary>
public class BuildingSelectionUI : MonoBehaviour
{
    public static BuildingSelectionUI Instance { get; private set; }

    [Header("Button Icons (order: Troop, Wall, Cannon, Flag)")]
    public Image[] redIcons;          // size 4
    public Image[] blueIcons;         // size 4
    public Color normalColor = Color.white;
    public Color highlightColor = Color.green;

    private ColorType currentOwner = ColorType.Red;

    void Awake()
    {
        if (Instance && Instance != this) Destroy(gameObject);
        Instance = this;
        ClearHighlight();
    }

    /// <summary>Called from GridCursor when the player presses Space on a cell.</summary>
    public void StartSelection(ColorType owner, int startIndex)
    {
        currentOwner = owner;
        ClearHighlight();
        Highlight(startIndex);
    }

    /// <summary>Moves the yellow outline when A / D is pressed.</summary>
    public void Highlight(int index)
    {
        ClearHighlight();
        Image[] arr = currentOwner == ColorType.Red ? redIcons : blueIcons;
        if (index >= 0 && index < arr.Length) arr[index].color = highlightColor;
    }

    /// <summary>Called when the player confirms or cancels.</summary>
    public void EndSelection() => ClearHighlight();

    /* ---------- helpers ---------- */
    void ClearHighlight()
    {
        foreach (var img in redIcons) if (img) img.color = normalColor;
        foreach (var img in blueIcons) if (img) img.color = normalColor;
    }
}
