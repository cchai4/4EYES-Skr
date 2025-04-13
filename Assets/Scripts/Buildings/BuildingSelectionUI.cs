using UnityEngine;
using UnityEngine.UI;
using static GridCellTint;

public class BuildingSelectionUI : MonoBehaviour
{
    public static BuildingSelectionUI Instance { get; private set; }

    [Header("Button Icons (order: TroopSpawner, Generator, Cannon, Flag)")]
    public Image[] redIcons;     // size 4
    public Image[] blueIcons;    // size 4

    public Color normalColor = Color.white;
    public Color highlightColor = Color.green;

    // Track active selection per player
    private int redHighlightIndex = -1;
    private int blueHighlightIndex = -1;

    void Awake()
    {
        if (Instance && Instance != this)
            Destroy(gameObject);
        Instance = this;

        ClearAllHighlights();
    }

    public void StartSelection(ColorType owner, int startIndex)
    {
        if (owner == ColorType.Red)
        {
            redHighlightIndex = startIndex;
            UpdateHighlight(ColorType.Red);
        }
        else
        {
            blueHighlightIndex = startIndex;
            UpdateHighlight(ColorType.Blue);
        }
    }

    public void Highlight(ColorType owner, int index)
    {
        if (owner == ColorType.Red)
        {
            redHighlightIndex = index;
            UpdateHighlight(ColorType.Red);
        }
        else
        {
            blueHighlightIndex = index;
            UpdateHighlight(ColorType.Blue);
        }
    }

    public void EndSelection(ColorType owner)
    {
        if (owner == ColorType.Red)
        {
            redHighlightIndex = -1;
            UpdateHighlight(ColorType.Red);
        }
        else
        {
            blueHighlightIndex = -1;
            UpdateHighlight(ColorType.Blue);
        }
    }

    // Updates only the icons for the given player
    private void UpdateHighlight(ColorType owner)
    {
        Image[] arr = (owner == ColorType.Red) ? redIcons : blueIcons;
        int highlightIndex = (owner == ColorType.Red) ? redHighlightIndex : blueHighlightIndex;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i])
                arr[i].color = (i == highlightIndex) ? highlightColor : normalColor;
        }
    }

    private void ClearAllHighlights()
    {
        foreach (var img in redIcons)
            if (img) img.color = normalColor;

        foreach (var img in blueIcons)
            if (img) img.color = normalColor;

        redHighlightIndex = -1;
        blueHighlightIndex = -1;
    }
}
