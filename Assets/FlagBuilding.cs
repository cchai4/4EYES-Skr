using UnityEngine;

public class FlagBuilding : BuildingBase
{
    protected override void Awake()
    {
        base.Awake(); // Initializes currentHP from maxHP
        FlagManager.Instance.RegisterFlag(this);
    }

    public override void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            FlagManager.Instance.UnregisterFlag(this);
            DestroyBuilding();
        }
    }

    protected override void DestroyBuilding()
    {
        // Optional: add flag-specific logic here
        base.DestroyBuilding();
    }
}
