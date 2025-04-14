using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // The singleton instance

    // Example variable to track game mode. You can add more variables as needed.
    public bool isMultiplayer = false;

    void Awake()
    {
        // Check if the Instance already exists
        if (Instance == null)
        {
            // Assign this instance if it doesn't exist yet
            Instance = this;
            // Prevent this GameObject from being destroyed on scene load
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists and it's not this, destroy the duplicate
            Destroy(gameObject);
        }
    }
}