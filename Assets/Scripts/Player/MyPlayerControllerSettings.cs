using EnhancedEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerSettings", menuName = "Datas/PlayerControllerSettings")]
public class MyPlayerControllerSettings : ScriptableObject
{
    #region Fields / Properties
    [HorizontalLine(2, order = 0), Section("CONTROLLER", order = 1)]
    public AnimationCurve       SpeedCurve =                new AnimationCurve();

    public float                JumpInitialForce =          17;

    public float                JumpContinousForce =        .15f;

    public float                JumpMaxTimeLength =         .5f;


    [HorizontalLine(2, SuperColor.Indigo, order = 0), Section("SPECIALS", order = 1)]
    public GameObject           Projectile =                null;

    public GameObject           Balls =                     null;

    public float                PlantProjectileInterval =   1.5f;

    public float                PlantActivationTime =       10f;

    public float                ShieldActivationTime =      5f;

    public float                BallsProjectileInterval =   .2f;

    public float                BallsActivationTime =       5f;
    #endregion
}
