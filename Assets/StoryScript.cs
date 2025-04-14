using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogicScript : MonoBehaviour
{
    public float typing_delay = 0.05f;
    public Text Red_text;
    public Text Blue_text;
    public Text Story_text;
    public GameObject Main_dialogue;
    public GameObject Story;
    public string[] storyLines = {
        "Centuries ago, the world of Elarion was vibrant with magic and life. But greed fractured the balance—two mighty sorcerers, once allies, sought to bend the Primordial Flame to their will. The flame, source of all creation and destruction, shattered under the strain.",
        "What followed was The Sundering—a magical cataclysm that reduced the realm to ash and void. Civilizations crumbled. The skies split. Oceans boiled. Yet from the wreckage, two figures emerged—the last surviving sorcerers: Vaelgor the Architect, and Nyssara the Shardbinder.",
        "Now, as the smoke clears and the embers dim, they stand atop the ruin of a dead world, both intent on rebuilding it anew—each with their own vision of what it should become.",
        "But the world cannot be born twice. Only one vision can take root.",
        "Will the world be forged in order and light? Or reborn through chaos and shadow?",
        "You are a fragment of this power—a soul reawakened, drawn to the heart of the conflict. Choose your side, shape your destiny, and determine the fate of this new beginning."
    };
    public string[] dialogueLines = {
        "It didn't have to be this way, Nyssara. We could have saved it—together.",
        "Save it? We were its gods, Vaelgor! And it spat in our faces. Let it burn. I’ll rebuild it in my image—without weakness.",
        "Then we are no longer allies. This world deserves a future forged in wisdom, not vengeance.",
        "Let the ashes decide."
    };

    private int story_lines_typed = 0;
    private int dialogueIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine = null;

    void Start()
    {
        Story.SetActive(true);
        Main_dialogue.SetActive(false);
        Story_text.text = "";
        Red_text.text = "";
        Blue_text.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                SkipCurrentLine();
                return;
            }

            if (story_lines_typed < storyLines.Length)
            {
                typingCoroutine = StartCoroutine(TypeLine(storyLines[story_lines_typed], Story_text));
                story_lines_typed++;
            }
            else if (dialogueIndex < dialogueLines.Length)
            {
                Main_dialogue.SetActive(true);
                Story.SetActive(false);
                if (dialogueIndex % 2 == 0)
                {
                    typingCoroutine = StartCoroutine(TypeLine(dialogueLines[dialogueIndex], Red_text));
                    Blue_text.text = "";
                }
                else
                {
                    typingCoroutine = StartCoroutine(TypeLine(dialogueLines[dialogueIndex], Blue_text));
                    Red_text.text = "";
                }
                dialogueIndex++;
            }
            else
            {
                Debug.Log("Dialogue complete");
                next_scene();
            }
        }
    }

    void SkipCurrentLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        if (story_lines_typed <= storyLines.Length && Story_text.text != storyLines[story_lines_typed - 1])
        {
            Story_text.text = storyLines[story_lines_typed - 1];
        }
        else if (dialogueIndex <= dialogueLines.Length)
        {
            if ((dialogueIndex - 1) % 2 == 0)
            {
                Red_text.text = dialogueLines[dialogueIndex - 1];
            }
            else
            {
                Blue_text.text = dialogueLines[dialogueIndex - 1];
            }
        }
        isTyping = false;
    }

    public void previous_scene()
    {
        SceneManager.LoadScene("Home Tab");
    }

    public void next_scene()
    {
        if (GameManager.Instance != null)
        {
            if(GameManager.Instance.isMultiplayer)
            {
                SceneManager.LoadScene("Multiplayer");
            }
            else if (!GameManager.Instance.isMultiplayer)
            {
                SceneManager.LoadScene("Singleplayer1");
            }
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }
    }

    IEnumerator TypeLine(string line, Text target)
    {
        isTyping = true;
        target.text = "";
        for (int i = 0; i < line.Length; i++)
        {
            target.text += line[i];
            yield return new WaitForSeconds(typing_delay);
        }
        isTyping = false;
    }
}