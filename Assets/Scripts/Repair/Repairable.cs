using EnhancedEditor;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField]
    private UnityEvent                  @event =                 new UnityEvent();


    /**********************
     ***   PROPERTIES   ***
     *********************/

    public RepairTpe RepairType { get { return repairType; } }
    #endregion

    #region Methods
    /***********************
     *****   METHODS   *****
     **********************/

    protected virtual void Activate(bool _doDeactivate)
    {
        // Feedback

        @event?.Invoke();
        if (_doDeactivate) Deactivate();
    }

    public virtual void Deactivate() => gameObject.SetActive(false);

    public bool Repair(MyPlayercontroller _player)
    {
        trigger.enabled = false;
        repairCount++;

        UIManager.I?.SetReppairPercent(_player.IsPlayerOne, (float)repairCount / repairAmount);

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

    public virtual void Spawn()
    {
        repairCount = 0;
        gameObject.SetActive(true);
        trigger.enabled = true;
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
    Other = -4,
    Balls,
    Plant,
    Shield,
    Saw,
    Spikes
}
