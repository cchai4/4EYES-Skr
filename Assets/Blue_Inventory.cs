using UnityEngine;

public class Blue_Inventory : MonoBehaviour
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
        gold_count ++;
    }

    public void add_silver()
    {
        silver_count++;
    }
}
