using UnityEngine;
using UnityEngine.UI;
using static GridCellTint;

public class BuildingSelectionUI : MonoBehaviour
{
    public static BuildingSelectionUI Instance { get; private set; }

    [Header("Button Icons (order: TroopSpawner, Generator, Cannon, Flag)")]
    public Image[] redIcons;     // Ensure these arrays have valid elements in the Inspector (or adjust sizes)
    public Image[] blueIcons;

    [Header("Red Cost Texts (order: TroopSpawner, Generator, Cannon, Flag)")]
    public Text[] redGoldCostTexts;
    public Text[] redRuneCostTexts;

    [Header("Blue Cost Texts (order: TroopSpawner, Generator, Cannon, Flag)")]
    public Text[] blueGoldCostTexts;
    public Text[] blueRuneCostTexts;

    [Header("Building Types (order: TroopSpawner, Generator, Cannon, Flag)")]
    public BuildingType[] buildingTypes; // Make sure this array matches the expected order

    public Color normalColor = Color.white;
    public Color highlightColor = Color.green;

    private int redHighlightIndex = -1;
    private int blueHighlightIndex = -1;

    void Awake()
    {
        if (Instance && Instance != this)
            Destroy(gameObject);
        Instance = this;
        ClearAllHighlights();
    }

    public void StartSelection(GridCellTint.ColorType owner, int startIndex)
    {
        if (owner == GridCellTint.ColorType.Red)
        {
            redHighlightIndex = startIndex;
            UpdateHighlight(GridCellTint.ColorType.Red);
        }
        else
        {
            blueHighlightIndex = startIndex;
            UpdateHighlight(GridCellTint.ColorType.Blue);
        }
        UpdateCostDisplay(owner);
    }

    public void Highlight(GridCellTint.ColorType owner, int index)
    {
        if (owner == GridCellTint.ColorType.Red)
        {
            redHighlightIndex = index;
            UpdateHighlight(GridCellTint.ColorType.Red);
        }
        else
        {
            blueHighlightIndex = index;
            UpdateHighlight(GridCellTint.ColorType.Blue);
        }
    }

    public void EndSelection(GridCellTint.ColorType owner)
    {
        if (owner == GridCellTint.ColorType.Red)
        {
            redHighlightIndex = -1;
            UpdateHighlight(GridCellTint.ColorType.Red);
        }
        else
        {
            blueHighlightIndex = -1;
            UpdateHighlight(GridCellTint.ColorType.Blue);
        }
    }

    private void UpdateHighlight(GridCellTint.ColorType owner)
    {
        Image[] arr = (owner == GridCellTint.ColorType.Red) ? redIcons : blueIcons;
        int highlightIndex = (owner == GridCellTint.ColorType.Red) ? redHighlightIndex : blueHighlightIndex;

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

    public void UpdateCostDisplay(GridCellTint.ColorType owner)
    {
        Text[] goldTexts = (owner == GridCellTint.ColorType.Red) ? redGoldCostTexts : blueGoldCostTexts;
        Text[] runeTexts = (owner == GridCellTint.ColorType.Red) ? redRuneCostTexts : blueRuneCostTexts;

        for (int i = 0; i < buildingTypes.Length; i++)
        {
            (int gold, int runes) = BuildingCostManager.Instance.GetCost(buildingTypes[i], owner);
            if (i < goldTexts.Length && goldTexts[i] != null)
                goldTexts[i].text = gold.ToString();
            if (i < runeTexts.Length && runeTexts[i] != null)
                runeTexts[i].text = runes.ToString();
        }
    }
}
