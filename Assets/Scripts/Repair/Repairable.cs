using EnhancedEditor;
using UnityEngine;

public class Repairable : MonoBehaviour
{
    #region Fields / Properties
    /**********************
     *****   FIELDS   *****
     *********************/

    [SerializeField, HorizontalLine(order = 0), Section("REPAIRABLE", order = 1), Space(order = 2)]
    protected int                       repairAmount =          10;

    protected int                       repairCount =           0;


    [SerializeField]
    protected CircleCollider2D          trigger =              null;


    [SerializeField]
    protected RepairTpe                 repairType =            RepairTpe.Plant;
    #endregion

    #region Methods
    /***********************
     *****   METHODS   *****
     **********************/

    protected virtual void Activate(bool _doDestroy)
    {
        // Feedback

        if (_doDestroy) Destroy(gameObject);
    }

    public bool Repair(MyPlayercontroller _player)
    {
        trigger.enabled = false;
        repairCount++;

        UIManager.I?.SetReppairPercent((float)repairCount / repairAmount);

        if (repairCount== repairAmount)
        {
            // Activate player related abilities
            switch (repairType)
            {
                case RepairTpe.Balls:
                    _player.RepairBalls();
                    break;

                case RepairTpe.Plant:
                    _player.RepairPlant();
                    break;

                case RepairTpe.Shield:
                    _player.RepairShield();
                    break;

                default:
                    break;
            }

            Activate(repairType < 0);
            return true;
        }

        return false;
    }


    protected virtual void Awake()
    {
        if (!trigger) trigger = GetComponentInChildren<CircleCollider2D>();
        trigger.isTrigger = true;
    }
    #endregion
}

public enum RepairTpe
{
    Balls = -3,
    Plant,
    Shield,
    Saw,
    Spikes
}
