using UnityEngine;
using UnityEngine.UI;
using static GridCellTint;

public class BuildingSelectionUI : MonoBehaviour
{
    public static BuildingSelectionUI Instance { get; private set; }

    [Header("Button Icons (order: TroopSpawner, Generator, Cannon, Flag)")]
    public Image[] redIcons;     // size 4
    public Image[] blueIcons;    // size 4

    [Header("Red Cost Texts (order: TroopSpawner, Generator, Cannon, Flag)")]
    public Text[] redGoldCostTexts;
    public Text[] redRuneCostTexts;

    [Header("Blue Cost Texts (order: TroopSpawner, Generator, Cannon, Flag)")]
    public Text[] blueGoldCostTexts;
    public Text[] blueRuneCostTexts;



    [Header("Building Types (order: TroopSpawner, Generator, Cannon, Flag)")]
    public BuildingType[] buildingTypes; // Order must match icons & cost texts

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

        // New: Update costs when the selection starts.
        UpdateCostDisplay(owner);
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

    // New: Updates the cost texts for each building type based on the player's current resources.
    public void UpdateCostDisplay(ColorType owner)
    {
        Text[] goldTexts = (owner == ColorType.Red) ? redGoldCostTexts : blueGoldCostTexts;
        Text[] runeTexts = (owner == ColorType.Red) ? redRuneCostTexts : blueRuneCostTexts;

        for (int i = 0; i < buildingTypes.Length; i++)
        {
            (int gold, int runes) = BuildingCostManager.Instance.GetCost(buildingTypes[i], owner);

            if (i < goldTexts.Length && goldTexts[i] != null)
                goldTexts[i].text = gold.ToString();

            if (i < runeTexts.Length && runeTexts[i] != null)
                runeTexts[i].text =  runes.ToString();
        }
    }

}
