using UnityEngine;

/// <summary>
/// Holds the key assignments for movement and dash/cancel actions.
/// You can set these in the Inspector, or you can create separate ones for Red vs. Blue.
/// </summary>
[System.Serializable]
public class PlayerControls
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    // For dash double-tap logic
    public KeyCode dashDoubleTapKey;

    // For "cancel" or any other special action
    public KeyCode cancelKey;
}
