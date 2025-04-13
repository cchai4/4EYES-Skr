using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeScript : MonoBehaviour
{
    public float remaining_time = 180;
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
        if (remaining_time > 0)
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
            remaining_time = remaining_time - Time.deltaTime;
            time_text.text = remaining_time.ToString();
        }
        else
        {
            time_text.text = "Game Over";
            gameOverScreen.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
