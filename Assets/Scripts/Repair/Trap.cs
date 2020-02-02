﻿using EnhancedEditor;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Repairable
{
    #region Fields / Properties
    [SerializeField, HorizontalLine(2, SuperColor.Green, order = 0), Section("REFERENCES", order = 1), Space(order = 2)]
    private Animator                animator =              null;

    [SerializeField]
    private new Collider2D          collider =              null;

    [SerializeField]
    private SpriteRenderer          repairSprite = null;


    [SerializeField, HorizontalLine(2, SuperColor.Sapphire, order = 0), Section("SETTINGS", order = 1), Space(order = 2)]
    private float                   deactivateTime =        10;
    #endregion

    #region Methods

    #region Original Methods
    protected override void Activate(bool _doDestroy)
    {
        base.Activate(_doDestroy);
        repairSprite.enabled = false;

        ActivateAnimator();
        if (repairType == RepairTpe.Spikes)
        {
            GameManager.PlayClipAtPoint(GameManager.I?.TrapSound, transform.position);
            Invoke("ActivateAnimator", deactivateTime);
        }
    }

    public void ActivateAnimator()
    {
        animator.SetTrigger("Activate");
    }

    public override void Deactivate()
    {
        base.Deactivate();
        LevelManager.I?.CallNewRepairable();
    }

    public override void Spawn()
    {
        base.Spawn();
        repairSprite.enabled = true;
    }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        if (collider) collider.isTrigger = true;
        if (!repairSprite) repairSprite = GetComponentInChildren<SpriteRenderer>();
    }
    #endregion

    #endregion
}
