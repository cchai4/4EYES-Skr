using UnityEngine;

public class AgentStatusGUI : MonoBehaviour
{
    // Reference to your RedAgent script
    public RedAgent redAgent;

    void OnGUI()
    {
        if (redAgent != null)
        {
            // Define a rectangular area for our GUI elements.
            GUILayout.BeginArea(new Rect(10, 10, 200, 100));

            // Display the current episode.
            GUILayout.Label("Current Episode: " + redAgent.CurrentEpisode);

            // Display the cumulative reward with two decimal places.
            GUILayout.Label("Current Reward: " + redAgent.CumulativeReward.ToString("F2"));

            GUILayout.EndArea();
        }
        else
        {
            GUILayout.Label("RedAgent not assigned in GUI");
        }
    }
}
