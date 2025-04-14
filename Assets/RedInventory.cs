using UnityEngine;

public class RedInventory : MonoBehaviour
{
    private int gold_count;
    private int silver_count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gold_count = 0;
        silver_count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void add_gold()
    {
        gold_count++;
    }

    public void add_silver()
    {
        silver_count++;
    }

    public int GetDiamondCount()
    {
        return diamond_count;
    }

<<<<<<< Updated upstream
=======
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

    public int GetDiamondCount()
    {
        return diamond_count;
    }

>>>>>>> Stashed changes
}
