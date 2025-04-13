using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeScript : MonoBehaviour
{
    public float remaining_time = 5;
    public float timer = 0;
    public Text time_text;
    public GameObject gold;
    public GameObject silver;
    public GameObject gameOverScreen;
    public GameObject resource_spawner;
    public Spawner spawner;
    public float spawnRate = 2;
    public int maxGold = 6;  // New: max gold allowed on screen

    private string[] resource_options = { "gold", "silver" };

    void Start()
    {
        spawner = resource_spawner.GetComponent<Spawner>();
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
                GameObject[] goldObjects = GameObject.FindGameObjectsWithTag("Gold"); // New
                GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Diamond");
                int num_objects = taggedObjects.Length;
                int gold_count = goldObjects.Length; // New
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
            gameOverScreen.SetActive(true);
        }
    }

    public void Restart()
    {
        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
