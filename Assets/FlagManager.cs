using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance;

    private List<FlagBuilding> redFlags = new List<FlagBuilding>();
    private List<FlagBuilding> blueFlags = new List<FlagBuilding>();

    public int winFlagCount = 3;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void RegisterFlag(FlagBuilding flag)
    {
        if (flag.team == Team.Red)
            redFlags.Add(flag);
        else if (flag.team == Team.Blue)
            blueFlags.Add(flag);

        CheckWinCondition();
    }

    public void UnregisterFlag(FlagBuilding flag)
    {
        if (flag.team == Team.Red)
            redFlags.Remove(flag);
        else if (flag.team == Team.Blue)
            blueFlags.Remove(flag);
    }

    void CheckWinCondition()
    {
        if (redFlags.Count >= winFlagCount)
            Debug.Log("?? Red wins!");
        if (blueFlags.Count >= winFlagCount)
            Debug.Log("?? Blue wins!");
    }
}
