using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public Text Red_text;
    public Text Blue_text;
    public string[] dialogueLines = {
        "The disaster spared only us. We could rebuild together—unite our powers for a better future.",
        "Better? I seek dominion, not compromise. I will seize control and reshape this world in my image.",
        "Then our magic must decide—no more words, only battle.",
        "So be it. Let destiny witness the clash of our wills."
    };
    public int Index_line = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (dialogueLines.Length < 0)
            Debug.Log("No dialogue to display");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (Index_line < dialogueLines.Length)
            {
                if(Index_line % 2 == 0)
                    Red_text.text = dialogueLines[Index_line];
                if(Index_line % 2 == 1)
                    Blue_text.text = dialogueLines[Index_line];
                Index_line++;
            }
            else
            {
                Debug.Log("Dialogue complete");
                SceneManager.LoadScene("Game");
            }
        }
    }
}
