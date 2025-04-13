using UnityEngine;

public class Generator : BuildingBase
{
    protected override void Awake()
    {
        base.Awake();
        GeneratorManager.Instance.RegisterGenerator(team);
    }

    // This is called by the base class if HP goes to 0
    protected override void DestroyBuilding()
    {
        GeneratorManager.Instance.UnregisterGenerator(team);
        base.DestroyBuilding();
    }

    private void OnDestroy()
    {
        // If this object is destroyed in another way (e.g. scene change),
        // make sure we also unregister so the manager isn't left with an invalid count.
        GeneratorManager.Instance.UnregisterGenerator(team);
    }
}
