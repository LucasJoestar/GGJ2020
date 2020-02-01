using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerSettings", menuName = "Datas/PlayerControllerSettings")]
public class MyPlayerControllerSettings : ScriptableObject
{
    #region Fields / Properties
    public AnimationCurve       SpeedCurve =            new AnimationCurve();

    public float                JumpInitialForce =      17;

    public float                JumpContinousForce =    .15f;

    public float                JumpMaxTimeLength =     .5f;
    #endregion
}
