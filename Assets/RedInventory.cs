using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RedInventory : MonoBehaviour
{
    public static RedInventory Instance { get; private set; }

    private int gold_count;
    private int diamond_count;
    public Text red_gold;
    public Text red_diamond;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        gold_count = 0;
        diamond_count = 0;
    }

    public void write_gold()
    {
        red_gold.text = "Gold: " + gold_count.ToString();
    }

    public void write_diamond()
    {
        red_diamond.text = "Runes: " + diamond_count.ToString();
    }

    public void add_gold()
    {
        gold_count++;
    }

    public void add_diamond()
    {
        diamond_count++;
    }

    // Check whether we have enough for cost
    public bool HasEnoughResources(int requiredGold, int requiredDiamonds)
    {
        return (gold_count >= requiredGold && diamond_count >= requiredDiamonds);
    }

    // Subtract the cost
    public void RemoveResources(int goldToRemove, int diamondsToRemove)
    {
        gold_count -= goldToRemove;
        diamond_count -= diamondsToRemove;
        write_gold();
        write_diamond();
    }

    // ?????????????????????????????????????????????????????????????????????
    //  NEW: Flash the resource text if we're missing gold or runes.
    // ?????????????????????????????????????????????????????????????????????
    public void FlashInsufficientResources(int requiredGold, int requiredDiamonds)
    {
        if (gold_count < requiredGold)
            StartCoroutine(FlashText(red_gold));

        if (diamond_count < requiredDiamonds)
            StartCoroutine(FlashText(red_diamond));
    }

    // A simple coroutine that blinks the text UI in red 3 times
    private IEnumerator FlashText(Text textUI)
    {
        Color originalColor = textUI.color;

        for (int i = 0; i < 3; i++) // number of flashes
        {
            // Turn it red
            textUI.color = Color.red;
            yield return new WaitForSeconds(0.2f);

            // Back to original
            textUI.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
