using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public GameObject Main_Menu;
    public GameObject Options_Panel;

    void Start()
    {
        Main_Menu.SetActive(true);
        Options_Panel.SetActive(false);
    }
    public void play()
    {
        Main_Menu.SetActive(false);
        Options_Panel.SetActive(true);
    }

    public void single_player()
    {
        if(Game_Manager.Instance != null)
        {
            Game_Manager.Instance.isMultiplayer = false;
        }
        SceneManager.LoadScene("Story");
    }

    public  void multliplayer()
    {
        if(Game_Manager.Instance != null)
        {
            Game_Manager.Instance.isMultiplayer = true;
        }
        SceneManager.LoadScene("Story");
    }
    public void back_to_main_menu()
    {
        Main_Menu.SetActive(true);
        Options_Panel.SetActive(false);
    }
}
