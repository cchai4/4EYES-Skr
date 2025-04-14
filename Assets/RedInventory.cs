using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RedInventory : MonoBehaviour
{
    public static RedInventory Instance { get; private set; }

    [Header("Starting Resources")]
    [SerializeField] private int startingGold = 10;
    [SerializeField] private int startingRunes = 0;

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
        gold_count = startingGold;
        diamond_count = startingRunes;

        write_gold();
        write_diamond();
    }

    public void write_gold()
    {
        red_gold.text = ":" + gold_count.ToString();
    }

    public void write_diamond()
    {
        red_diamond.text = ":"+ diamond_count.ToString();
    }

    public void add_gold()
    {
        gold_count++;
    }

    public void add_diamond()
    {
        diamond_count++;
    }

    public bool HasEnoughResources(int requiredGold, int requiredDiamonds)
    {
        return (gold_count >= requiredGold && diamond_count >= requiredDiamonds);
    }

    public void RemoveResources(int goldToRemove, int diamondsToRemove)
    {
        gold_count -= goldToRemove;
        diamond_count -= diamondsToRemove;
        write_gold();
        write_diamond();
    }

    public void FlashInsufficientResources(int requiredGold, int requiredDiamonds)
    {
        if (gold_count < requiredGold)
            StartCoroutine(FlashText(red_gold));

        if (diamond_count < requiredDiamonds)
            StartCoroutine(FlashText(red_diamond));
    }

    private IEnumerator FlashText(Text textUI)
    {
        Color originalColor = textUI.color;

        for (int i = 0; i < 3; i++)
        {
            textUI.color = Color.red;
            yield return new WaitForSeconds(0.2f);

            textUI.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
