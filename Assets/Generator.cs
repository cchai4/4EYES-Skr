using UnityEngine;

public class Generator : BuildingBase
{
    protected override void Awake()
    {
        base.Awake();
        GeneratorManager.Instance.RegisterGenerator(team);
    }

    
    protected override void DestroyBuilding()
    {
        GeneratorManager.Instance.UnregisterGenerator(team);
        base.DestroyBuilding();
    }

    private void OnDestroy()
    {
        
        GeneratorManager.Instance.UnregisterGenerator(team);
    }
}
