using EnhancedEditor;
using System;
using System.Collections;
using UnityEngine;

public class MyPlayercontroller : Movable
{
    #region Events

    #endregion

    #region Fields / Properties
    /*********************
     ***   CONSTANTS   ***
     ********************/

    public const float                      ResetSpeedTime =        .2f;


    /**********************
     *****   FIELDS   *****
     *********************/

    [SerializeField, PropertyField]
    private bool                            isPlayable =               true;


    [SerializeField, HorizontalLine(2, SuperColor.Green, order = 0), Section("References", order = 1), Space(order = 2)]
    private MyPlayerControllerSettings      playerSettings =        null;

    [SerializeField]
    private MyPlayerInputs                  playerInputs =          null;


    /**********************
     ***   PROPERTIES   ***
     *********************/

    public bool IsPlayable
    {
        get { return isPlayable; }
        private set
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                isPlayable = true;
                return;
            }
            #endif

            isPlayable = value;

            if (value)
            {
                if (checkInputCoroutine == null)
                {
                    checkInputCoroutine = StartCoroutine(CheckInput());
                }
            }
            else if (checkInputCoroutine != null)
            {
                StopCoroutine(checkInputCoroutine);
                checkInputCoroutine = null;
            }
        }
    }
    #endregion

    #region Memory & Coroutines
    /**********************
     ***   COROUTINES   ***
     *********************/

    private Coroutine   checkInputCoroutine =       null;

    private Coroutine   jumpCoroutine =             null;

    private Coroutine   repairCoroutine =           null;


    /**********************
     *****   MEMORY   *****
     *********************/

    private float       speedIncreaseTimer =                0;

    private float       speedResetTimer =                   0;
    #endregion

    #region Methods

    #region Original Methods
    /**************************
     *******   INPUTS   *******
     *************************/

    private IEnumerator CheckInput()
    {
        while (true)
        {
            yield return null;

            // Repair input
            if (Input.GetButtonDown(playerInputs.RepairButton))
            {
                // Do it
                continue;
            }

            // Jump input
            if (Input.GetButtonDown(playerInputs.JumpButton))
            {
                Jump();
            }

            // X movement input
            float _movement = Input.GetAxis(playerInputs.HorizontalAxis) * speed * Time.deltaTime;
            if (_movement == 0)
            {
                if (speedResetTimer < ResetSpeedTime)
                {
                    speedResetTimer += Time.deltaTime;
                    if (speedResetTimer >= ResetSpeedTime) ResetSpeed();
                }

                continue;
            }

            if (speedResetTimer > 0) speedResetTimer = 0;

            // Increase speed if needed
            if (speedIncreaseTimer < playerSettings.SpeedCurve[playerSettings.SpeedCurve.length - 1].time)
            {
                speedIncreaseTimer = Mathf.Min(speedIncreaseTimer + Time.deltaTime, playerSettings.SpeedCurve[playerSettings.SpeedCurve.length - 1].time);
                speed = playerSettings.SpeedCurve.Evaluate(speedIncreaseTimer);
            }

            // Perform movement
            PerformMovement(new Vector2(_movement, 0));
        }
    }

    /************************
     *******   JUMP   *******
     ***********************/

    private IEnumerator DoJump()
    {
        float _timer = playerSettings.JumpMaxTimeLength;
        velocity.y += playerSettings.JumpInitialForce;

        // Increase jump force while holding button
        while (Input.GetButton(playerInputs.JumpButton) && (_timer > 0))
        {
            yield return null;
            _timer += Time.deltaTime;

            velocity.y += playerSettings.JumpContinousForce * Time.deltaTime;
        }

        jumpCoroutine = null;
    }

    private void Jump()
    {
        // Return if cannot jump
        if (!isGrounded) return;

        if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
        jumpCoroutine = StartCoroutine(DoJump());
    }

    public void StopJump()
    {
        if (jumpCoroutine == null) return;

        StopCoroutine(jumpCoroutine);
        jumpCoroutine = null;
    }


    /**************************
     *******   REPAIR   *******
     *************************/

    private IEnumerator DoRepair()
    {
        while (true)
        {
            yield return null;
        }

        repairCoroutine = null;
    }

    public void Repair()
    {
        if (repairCoroutine != null) StopCoroutine(repairCoroutine);
        repairCoroutine = StartCoroutine(DoRepair());
    }

    public void StopRepair()
    {
        if (repairCoroutine == null) return;

        StopCoroutine(repairCoroutine);
        repairCoroutine = null;
    }


    /***************************
     ******   MOVEMENTS   ******
     **************************/

    private void ResetSpeed()
    {
        speedIncreaseTimer = 0;
        speed = playerSettings.SpeedCurve[0].value;
    }
    #endregion

    #region Unity Methods
    /*****************************
     *****   UNITY METHODS   *****
     ****************************/

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Start checking inputs
        IsPlayable = true;
    }
    #endregion

    #endregion
}
