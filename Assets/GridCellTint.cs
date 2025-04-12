// GridCellTint.cs   (attach to the prefab)
using UnityEngine;

public class GridCellTint : MonoBehaviour
{
    private SpriteRenderer sr;
    private int redCount = 0;
    private int blueCount = 0;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    /* --- called by selectors --- */
    public void Enter(ColorType who)
    {
        if (who == ColorType.Red) redCount++;
        if (who == ColorType.Blue) blueCount++;
        UpdateTint();
    }

    public void Exit(ColorType who)
    {
        if (who == ColorType.Red) redCount = Mathf.Max(0, redCount - 1);
        if (who == ColorType.Blue) blueCount = Mathf.Max(0, blueCount - 1);
        UpdateTint();
    }

    void UpdateTint()
    {
        if (redCount > 0 && blueCount > 0) sr.color = Color.magenta;   // purple
        else if (redCount > 0) sr.color = Color.red;
        else if (blueCount > 0) sr.color = Color.blue;
        else sr.color = Color.white;     // default
    }

    public enum ColorType { Red, Blue }
}
