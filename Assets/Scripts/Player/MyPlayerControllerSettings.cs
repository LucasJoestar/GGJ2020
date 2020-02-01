using EnhancedEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerSettings", menuName = "Datas/PlayerControllerSettings")]
public class MyPlayerControllerSettings : ScriptableObject
{
    #region Fields / Properties
    [HorizontalLine(2, SuperColor.Chocolate)]
    public AnimationCurve       SpeedCurve =            new AnimationCurve();

    public float                JumpInitialForce =      17;

    public float                JumpContinousForce =    .15f;

    public float                JumpMaxTimeLength =     .5f;


    [HorizontalLine(2, SuperColor.Indigo)]
    public float test;
    #endregion
}
