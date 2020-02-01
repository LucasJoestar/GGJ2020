using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Repairable : MonoBehaviour
{
    #region Fields / Properties
    /**********************
     *****   FIELDS   *****
     *********************/

    [SerializeField]
    protected int                       repairAmount =          10;

    private int                         repairCount =           0;


    [SerializeField]
    private new CircleCollider2D        collider =              null;


    [SerializeField]
    private RepairTpe                   repairType =            RepairTpe.Plant;
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
        collider.enabled = false;
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
            if ((int)repairType > -1) LevelManager.I?.SpawnNewRepairable();
            return true;
        }

        return false;
    }


    protected virtual void Awake()
    {
        if (!collider) collider = GetComponent<CircleCollider2D>();
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
