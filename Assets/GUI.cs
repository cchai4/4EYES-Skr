using UnityEngine;

public class GUIRedAgentScript : MonoBehaviour
{
    [SerializeField] private RedAgent _redAgent;

    // Default style (yellow), positive (green), negative (red)
    private GUIStyle _defaultStyle = new GUIStyle();
    private GUIStyle _positiveStyle = new GUIStyle();
    private GUIStyle _negativeStyle = new GUIStyle();

    // Start is called before the first frame update
    void Start()
    {
        // Set up GUIStyles, you can modify these values as required.
        _defaultStyle.fontSize = 20;
        _defaultStyle.normal.textColor = Color.yellow;

        _positiveStyle.fontSize = 20;
        _positiveStyle.normal.textColor = Color.green;

        _negativeStyle.fontSize = 20;
        _negativeStyle.normal.textColor = Color.red;
    }

    void OnGUI()
    {
        // Create debug strings showing current episode, step count, and cumulative reward.
        string debugEpisode = "Episode: " + _redAgent.CurrentEpisode + " - Step: " + _redAgent.StepCount;
        string debugReward = "Reward: " + _redAgent.CumulativeReward.ToString("F2");

        // Choose style based on whether the cumulative reward is negative or positive.
        GUIStyle rewardStyle = _redAgent.CumulativeReward < 0 ? _negativeStyle : _positiveStyle;

        // Display the episode and step info at a position (x=20, y=20).
        GUI.Label(new Rect(20, 20, 500, 30), debugEpisode, _defaultStyle);

        // Display the reward info below the episode info.
        GUI.Label(new Rect(20, 60, 500, 30), debugReward, rewardStyle);
    }
}
