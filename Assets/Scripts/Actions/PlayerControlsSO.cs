using UnityEngine;

[CreateAssetMenu(menuName = "Controls/Player Controls")]
public class PlayerControlsSO : ScriptableObject
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode dashDoubleTapKey;
    public KeyCode cancelKey;
}
