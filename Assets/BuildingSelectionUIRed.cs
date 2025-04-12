using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionUI : MonoBehaviour
{
    public Button[] buttons;                 // assign in Inspector (order = Troop, Wall, Cannon, Flag)

    int index;
    public BuildingType Current => (BuildingType)index;

    void Awake() => Highlight(0);

    public void Move(int dir)               // dir = -1 (left) or +1 (right)
    {
        index = (index + dir + buttons.Length) % buttons.Length;
        Highlight(index);
    }

    void Highlight(int i)
    {
        for (int k = 0; k < buttons.Length; k++)
            buttons[k].transform.localScale = (k == i) ? Vector3.one * 1.2f : Vector3.one;
    }

    public void Show(bool yes) => gameObject.SetActive(yes);
}
