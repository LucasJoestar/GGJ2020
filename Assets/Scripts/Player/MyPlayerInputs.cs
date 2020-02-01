using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputs", menuName = "Datas/PlayerInputs")]
public class MyPlayerInputs : ScriptableObject
{
    #region Fields / Properties
    public bool         IsPlayerOne =           true;


    public string       HorizontalAxis =        "Horizontal";

    public string       JumpButton =            "Jump";

    public string       RepairButton =          "Repair";

    public string       QuitButton =            "Repair";
    #endregion
}
