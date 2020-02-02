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


    [SerializeField, HorizontalLine(2, SuperColor.Sapphire, order = 0), Section("SETTINGS", order = 1), Space(order = 2)]
    private float                   deactivateTime =        10;
    #endregion

    #region Methods

    #region Original Methods
    protected override void Activate(bool _doDestroy)
    {
        base.Activate(_doDestroy);

        ActivateAnimator();
        if (repairType == RepairTpe.Spikes) Invoke("ActivateAnimator", deactivateTime);
    }

    public void ActivateAnimator()
    {
        animator.SetTrigger("Activate");
    }

    public void Destroy() => Destroy(gameObject);
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        if (collider) collider.isTrigger = true;
    }
    #endregion

    #endregion
}