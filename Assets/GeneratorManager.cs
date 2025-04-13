using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public static GeneratorManager Instance;

    [SerializeField] private float goldInterval = 1f;  // time between gold increments
    private float timer = 0f;

    private int redGeneratorCount = 0;
    private int blueGeneratorCount = 0;

    private RedInventory redInv;
    private Blue_Inventory blueInv;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Find your inventories once; assume only one of each
        redInv = Object.FindFirstObjectByType<RedInventory>();
        blueInv = Object.FindFirstObjectByType<Blue_Inventory>();

        // Optionally keep this manager alive across scenes
        // DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= goldInterval)
        {
            timer = 0f;

            // Add gold based on how many active generators each side has
            if (redInv != null && redGeneratorCount > 0)
            {
                // e.g. 1 gold per generator
                for (int i = 0; i < redGeneratorCount; i++)
                {
                    redInv.add_gold();
                }
                redInv.write_gold();
            }

            if (blueInv != null && blueGeneratorCount > 0)
            {
                // e.g. 1 gold per generator
                for (int i = 0; i < blueGeneratorCount; i++)
                {
                    blueInv.add_gold();
                }
                blueInv.write_gold();
            }
        }
    }

    // Called by each Generator when it spawns
    public void RegisterGenerator(Team team)
    {
        if (team == Team.Red) redGeneratorCount++;
        else if (team == Team.Blue) blueGeneratorCount++;
    }

    // Called by each Generator when destroyed
    public void UnregisterGenerator(Team team)
    {
        if (team == Team.Red && redGeneratorCount > 0)
            redGeneratorCount--;
        else if (team == Team.Blue && blueGeneratorCount > 0)
            blueGeneratorCount--;
    }
}
