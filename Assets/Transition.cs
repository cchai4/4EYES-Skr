using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public GameObject Main_Menu;
    public GameObject Options_Panel;
    public GameObject Tutorial;

    void Start()
    {
        Main_Menu.SetActive(true);
        Options_Panel.SetActive(false);
        Tutorial.SetActive(false);
    }
    public void play()
    {
        Main_Menu.SetActive(false);
        Options_Panel.SetActive(true);
    }

    public void single_player()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isMultiplayer = false;
        }
        SceneManager.LoadScene("Story");
    }

    public void multliplayer()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isMultiplayer = true;
        }
        SceneManager.LoadScene("Story");
    }
    public void back_to_main_menu()
    {
        Main_Menu.SetActive(true);
        Options_Panel.SetActive(false);
        Tutorial.SetActive(false);
    }
    public void tutorial()
    {
        Main_Menu.SetActive(false);
        Tutorial.SetActive(true);
    }
}