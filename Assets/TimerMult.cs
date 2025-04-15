using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerMult : MonoBehaviour
{
    public float remaining_time = 5;
    public float timer = 0;
    public Text time_text;
    public GameObject gold;
    public GameObject silver;
    public GameObject gameOverScreen_mult;
    public GameObject resource_spawner;
    public Spawner spawner;
    public float spawnRate = 2;
    public int maxGold = 6;  // New: max gold allowed on screen

    private string[] resource_options = { "gold", "silver" };

    void Start()
    {
        spawner = resource_spawner.GetComponent<Spawner>();
        gameOverScreen_mult.SetActive(false);
    }

    void Update()
    {
        // Subtract the delta time from your total time
        remaining_time -= Time.deltaTime;

        // Use Mathf.FloorToInt to display the integer portion
        int displayTime = Mathf.FloorToInt(remaining_time);
        time_text.text = displayTime.ToString();

        // Check if there's time left
        if (remaining_time > 0)
        {
            // Handle spawning every "spawnRate" seconds
            if (timer < spawnRate)
            {
                timer += Time.deltaTime;
            }
            else
            {
                GameObject[] goldObjects = GameObject.FindGameObjectsWithTag("Gold");
                GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Diamond");
                int num_objects = taggedObjects.Length;
                int gold_count = goldObjects.Length;
                int randInt = Random.Range(0, resource_options.Length);

                if (randInt == 0 && gold_count < maxGold) // Check gold limit
                {
                    spawner.spawnResources(gold);
                    spawner.spawnResources(gold);
                }
                else if (randInt == 1 && num_objects < 1)
                {
                    spawner.spawnResources(silver);
                }
                timer = 0;
            }
        }
        else
        {
            // Time is up
            time_text.text = "Game Over";
            gameOverScreen_mult.SetActive(true);

            // NEW: Call the territory-count code in GameEndManager (if present):
            GameEndManager gem = Object.FindFirstObjectByType<GameEndManager>();
            if (gem != null)
            {
                gem.OnGameEnd();
            }
            else
            {
                Debug.LogWarning("TimerMult: No GameEndManager found in the scene, cannot call OnGameEnd!");
            }
        }
    }

    public void Restart()
    {
        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        SceneManager.LoadScene("Home Tab");
    }
}
