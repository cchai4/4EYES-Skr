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
     private string[] resource_options = {"gold", "silver"};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner = resource_spawner.GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        if (remaining_time > 0)
=======
        // Subtract the delta time from your total time
        remaining_time -= Time.deltaTime;

        // Use Mathf.FloorToInt to display the integer portion
        int displayTime = Mathf.FloorToInt(remaining_time);
        time_text.text = displayTime.ToString();

        // Check if there's time left
        if (true)
        // if (remaining_time > 0)
>>>>>>> Stashed changes
        {
            if (timer < spawnRate)
            {
                timer = timer + Time.deltaTime;
            }
            else
            {
                int randInt = Random.Range(0, resource_options.Length);
                if (randInt == 0)
                {
                    spawner.spawnResources(gold);
                    spawner.spawnResources(gold);
                    spawner.spawnResources(gold);
                }
                else if (randInt == 1)
                {
                    spawner.spawnResources(silver);
                }
                timer = 0;
            }
<<<<<<< Updated upstream
            remaining_time = remaining_time - Time.deltaTime;
            time_text.text = remaining_time.ToString();
=======
>>>>>>> Stashed changes
        }
        else
        {
            time_text.text = "Game Over";
            gameOverScreen.SetActive(true);
        }
    }

    // New method to reset the timer values
    public void ResetTimer()
    {
        remaining_time = 5;  // Reset to starting time (adjust as needed)
        timer = 0;
        if (time_text != null)
        {
            time_text.text = Mathf.FloorToInt(remaining_time).ToString();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
