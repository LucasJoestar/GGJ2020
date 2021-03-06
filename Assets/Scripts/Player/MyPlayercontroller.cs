﻿using EnhancedEditor;
using System;
using System.Collections;
using UnityEngine;

#pragma warning disable
public class MyPlayercontroller : Movable
{
    #region Events
    public static event Action<Repairable> OnRepairSomething = null;
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
    private bool                            isDead =                false;

    [SerializeField]
    private bool                            isJumping =             false;

    [SerializeField, PropertyField]
    private bool                            isPlayable =            true;

    [SerializeField, PropertyField]
    private bool                            isMoving =              false;


    [SerializeField, HorizontalLine(2, SuperColor.Green, order = 0), Section("SETTINGS", order = 1), Space(order = 2)]
    private MyPlayerControllerSettings      playerSettings =        null;

    [SerializeField]
    private MyPlayerInputs                  playerInputs =          null;


    [SerializeField]
    private AudioSource                     audioSource =              null;

    [SerializeField]
    private Animator                        animator =              null;

    [SerializeField]
    private Transform                       attackTransform =       null;

    [SerializeField, HorizontalLine(2, SuperColor.Raspberry, order = 0), Section("STATES", order = 1), Space(order = 2)]
    private bool                            isPlantActivated =      false;

    [SerializeField]
    private bool                            isBallsTrapActivated =  false;

    [SerializeField]
    private bool                            hasShield =             false;


    [SerializeField]
    private GameObject shieldAnchor = null;

    [SerializeField]
    private GameObject leftShield = null;

    [SerializeField]
    private GameObject rightShield = null;


    /**********************
     ***   PROPERTIES   ***
     *********************/

    public bool IsDead
    {
        get { return isDead; }
        private set
        {
            isDead = value;
            IsPlayable = !value;

            if (value) Die();
        }
    }

    public override bool IsGrounded
    {
        get { return base.IsGrounded; }
        protected set
        {
            isGrounded = true;

            animator?.SetBool("IsGrounded", value);
            if (value)
            {
                if (setGroundedCoroutine != null)
                {
                    StopCoroutine(setGroundedCoroutine);
                    setGroundedCoroutine = null;
                }
            }
            else
            {
                if (isGrounded && (setGroundedCoroutine == null))
                {
                    if (!isJumping) setGroundedCoroutine = StartCoroutine(SetGrounded());
                    else isGrounded = false;
                }
            }
        }
    }

    public bool IsPlayable
    {
        get { return isPlayable; }
        set
        {
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

    public bool IsPlayerOne { get { return playerInputs.IsPlayerOne; } }

    public bool IsMoving
    {
        get { return isMoving; }
        private set
        {
            isMoving = value;
            animator?.SetBool("IsMoving", value);
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

    private Coroutine   setGroundedCoroutine =      null;


    private Coroutine   shieldCoroutine =           null;

    private Coroutine   plantCoroutine =            null;

    private Coroutine   ballsCoroutine =            null;


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

            // Quit input
            if (Input.GetButtonDown(playerInputs.QuitButton))
            {
                Application.Quit();
                yield break;
            }

            // Repair input
            if (Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2) || Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1) || Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3) || Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.E : KeyCode.RightShift) || Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.F : KeyCode.Return))
            {
                Collider2D[] _colliders = new Collider2D[16];
                contactFilter.useTriggers = true;
                
                int _count = collider.OverlapCollider(contactFilter, _colliders);
                contactFilter.useTriggers = false;

                for (int _i = 0; _i < _count; _i++)
                {
                    if (_colliders[_i].gameObject.HasTag("Repairable"))
                    {
                        Debug.Log("Repair");
                        Repair(_colliders[_i].GetComponentInParent<Repairable>());
                        continue;
                    }
                }
            }

            // Jump input
            if (Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0) || Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.Space : KeyCode.RightControl))
            {
                Jump();
            }

            // X movement input
            float _movement = Input.GetAxis(playerInputs.HorizontalAxis) * speed * Time.deltaTime;
            if (_movement == 0)
            {
                if (isMoving) IsMoving = false;

                if (speedResetTimer < ResetSpeedTime)
                {
                    speedResetTimer += Time.deltaTime;
                    if (speedResetTimer >= ResetSpeedTime) ResetSpeed();
                }

                
                continue;
            }

            if (speedResetTimer > 0) speedResetTimer = 0;

            if (!isMoving) IsMoving = true;

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
        isJumping = true;
        animator?.SetTrigger("Jump");

        // Feedback
        GameManager.PlayClipAtPoint(GameManager.I?.JumpSound, transform.position);

        float _timer = playerSettings.JumpMaxTimeLength;
        velocity.y = playerSettings.JumpInitialForce;

        // Increase jump force while holding button
        while (Input.GetButton(playerInputs.JumpButton) && (_timer > 0))
        {
            yield return null;
            _timer -= Time.deltaTime;

            velocity.y += playerSettings.JumpContinousForce * Time.deltaTime;
        }

        isJumping = false;
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
        isJumping = false;
    }


    /**************************
     *******   REPAIR   *******
     *************************/

    private IEnumerator DoRepair(Repairable _repairable)
    {
        UIManager.I?.ActiveRepair(IsPlayerOne, new Vector2(transform.position.x, transform.position.y + 1.5f));
        UIManager.I?.SetReppairPercent(IsPlayerOne, 0);
        IsPlayable = false;

        audioSource.time = 0;
        audioSource.Play();

        bool _doRepair = true;

        OnRepairSomething += (Repairable _r) =>
        {
            if (_r == _repairable)
            {
                UIManager.I?.SetReppairPercent(IsPlayerOne, 1);
                _doRepair = false;
            }
        };

        while (_doRepair)
        {
            yield return null;
            if (Input.GetKeyDown(IsPlayerOne ? KeyCode.Joystick1Button2 : KeyCode.Joystick2Button2) || Input.GetKeyDown(IsPlayerOne ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1) || Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3) || Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.E : KeyCode.RightShift) || Input.GetKeyDown(playerInputs.IsPlayerOne ? KeyCode.F : KeyCode.Return))
            {
                if (_repairable.Repair(this)) break;
            }
        }

        OnRepairSomething?.Invoke(_repairable);
        audioSource.Stop();

        IsPlayable = true;
        repairCoroutine = null;
    }

    public void Repair(Repairable _repairable)
    {
        if (!_repairable) return;

        if (repairCoroutine != null) StopCoroutine(repairCoroutine);
        repairCoroutine = StartCoroutine(DoRepair(_repairable));
    }

    public void StopRepair()
    {
        if (repairCoroutine == null) return;

        audioSource.Stop();
        StopCoroutine(repairCoroutine);
        repairCoroutine = null;
    }

    /************************
     *******   LIFE   *******
     ***********************/

    private void Die()
    {
        // Stop what needs to be
        StopJump();
        StopAllCoroutines();
        audioSource.Stop();
        collider.enabled = false;

        // Play sound & animations
        animator.SetTrigger("Die");

        // Increase score
        GameManager.I.IncreaseScore(!playerInputs.IsPlayerOne);
        MyCamera.I?.StartScreenShake();
    }

    public void Kill(Vector2 _direction)
    {
        if (isDead || hasShield) return;
        IsDead = true;

        UseGravity = true;
        velocity = _direction.normalized * playerSettings.JumpInitialForce * .75f;

        Debug.Log(name + " Player is Dead !!");
    }

    /**************************
     ******   SPECIALS   ******
     *************************/

    private IEnumerator Plant()
    {
        // Feedback
        GameManager.PlayClipAtPoint(GameManager.I?.TurretInstall, transform.position);

        float _timer = playerSettings.PlantActivationTime;
        isPlantActivated = true;

        while (_timer > 0)
        {
            float _wait = Mathf.Min(playerSettings.PlantProjectileInterval, _timer);

            yield return new WaitForSeconds(_wait);
            _timer -= _wait;

            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(.1f);
            _timer -= .1f;
            Instantiate(playerSettings.Projectile, attackTransform.position, Quaternion.identity).GetComponent<Projectile>().Init(new Vector2(isFacingRight ? 1 : -1, -.25f));
            GameManager.PlayClipAtPoint(GameManager.I?.OrganicShot, attackTransform.position);
        }

        isPlantActivated = false;
        LevelManager.I?.CallNewRepairable();

        plantCoroutine = null;
    }

    private IEnumerator BallsTrap()
    {
        // Feedback

        float _timer = playerSettings.BallsActivationTime;
        isBallsTrapActivated = true;

        while (_timer > 0)
        {
            float _wait = Mathf.Min(playerSettings.BallsProjectileInterval, _timer);
            yield return new WaitForSeconds(_wait);
            _timer -= _wait;

            Instantiate(playerSettings.Balls, attackTransform.position, Quaternion.identity).GetComponent<Ball>().Init(isFacingRight);
            GameManager.PlayClipAtPoint(GameManager.I?.OrganicShot, attackTransform.position);
        }

        isBallsTrapActivated = false;
        LevelManager.I?.CallNewRepairable();

        ballsCoroutine = null;
    }

    private IEnumerator Shield()
    {
        // Feedback
        shieldAnchor.SetActive(true);

        float _timer = playerSettings.ShieldActivationTime;
        GameManager.PlayClipAtPoint(GameManager.I?.ShieldAura, transform.position);
        hasShield = true;

        while (_timer > 0)
        {
            yield return null;
            _timer -= Time.deltaTime;
        }

        hasShield = false;
        shieldAnchor.SetActive(false);
        GameManager.PlayClipAtPoint(GameManager.I?.ShieldAura, transform.position);
        LevelManager.I?.CallNewRepairable();

        shieldCoroutine = null;
    }


    public void RepairShield()
    {
        Debug.Log("Shield !!");

        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            LevelManager.I?.CallNewRepairable();
        }
        shieldCoroutine = StartCoroutine(Shield());
    }

    public void RepairPlant()
    {
        Debug.Log("Plant !!");

        if (plantCoroutine != null)
        {
            StopCoroutine(plantCoroutine);
            LevelManager.I?.CallNewRepairable();
        }
        plantCoroutine = StartCoroutine(Plant());
    }

    public void RepairBalls()
    {
        Debug.Log("Balls !!");

        if (ballsCoroutine != null)
        {
            StopCoroutine(ballsCoroutine);
            LevelManager.I?.CallNewRepairable();
        }
        ballsCoroutine = StartCoroutine(BallsTrap());
    }


    /***************************
     ******   MOVEMENTS   ******
     **************************/

    private IEnumerator SetGrounded()
    {
        yield return new WaitForSeconds(.1f);
        isGrounded = false;

        setGroundedCoroutine = null;
    }

    public override void Flip()
    {
        base.Flip();
        if (isFacingRight)
        {
            rightShield.SetActive(true);
            leftShield.SetActive(false);
        }
        else
        {
            rightShield.SetActive(false);
            leftShield.SetActive(true);
        }
    }

    protected override bool CheckColliderTag(RaycastHit2D _hit, Vector2 _movement)
    {
        // Kill player if jumped on his head
        if (_hit.collider.gameObject.HasTag("Player"))
        {
            if (Mathf.Approximately(_hit.normal.y, 1) && (_movement.y < 0) && !LevelManager.I.IsTutorial)
            {
                GameManager.PlayClipAtPoint(GameManager.I?.OrganicImpact, attackTransform.position);

                _hit.transform.GetComponent<MyPlayercontroller>()?.Kill(Vector3.right * (isFacingRight ? 1 : -1));
                velocity.y = playerSettings.JumpInitialForce * .75f;
            }
            return false;
        }
        if (_hit.collider.gameObject.HasTag("Projectile") && _hit.collider.GetComponent<Projectile>().DoHit)
        {
            Kill(new Vector2(isFacingRight ? -1 : 1, 0));
            return false;
        }
        return true;
    }

    private void OnHitSomethingCallback(RaycastHit2D _hit)
    {
        // Hit normal checks
        if (_hit.normal.y == 1)
        {
            StopJump();
        }
        else if (_hit.normal.y == -1)
        {
            StopJump();
        }
        else if(_hit.normal.x == 1)
        {
            // Do nothing
        }
        else if (_hit.normal.x == -1)
        {
            // Do nothing
        }

        // Feedback on hit
    }

    private IEnumerator OverlapCollisions()
    {
        int _count;
        Collider2D[] _colliders = new Collider2D[16];

        while (true)
        {
            yield return null;

            // Set contact filter
            contactFilter.useTriggers = true;

            // Extract player from overlapping colliders
            _count = collider.OverlapCollider(contactFilter, _colliders);
            for (int _i = 0; _i < _count; _i++)
            {
                if (_colliders[_i].isTrigger)
                {
                    if (_colliders[_i].gameObject.HasTag("Starter"))
                    {
                        GameManager.I?.SetGameReady(this);
                        continue;
                    }
                    if (_colliders[_i].gameObject.HasTag("Spikes"))
                    {
                        Kill(Vector3.up);

                        // Feedback
                        GameManager.PlayClipAtPoint(GameManager.I?.SpikeDeath, transform.position);
                        yield break;
                    }
                    Ball _ball = _colliders[_i].gameObject.GetComponent<Ball>();
                    if (_ball && _ball.DoHit)
                    {
                        _ball.DestroyBall();
                        Kill(Vector3.up);

                        // Feedback
                        GameManager.PlayClipAtPoint(GameManager.I?.BallWhoosh, transform.position);
                        yield break;
                    }

                    continue;
                }

                if (_colliders[_i].gameObject.HasTags(new string[] { "Player", "Projectile" })) continue;

                ColliderDistance2D _distance = collider.Distance(_colliders[_i]);
                if (_distance.isOverlapped)
                {
                    Vector2 _movement = _distance.pointA - _distance.pointB;
                    _movement = _movement.normalized * (_movement.magnitude - Physics2D.defaultContactOffset);
                    transform.position = (Vector2)transform.position - _movement;
                    rigidbody.MovePosition(rigidbody.position - _movement);
                }
            }

            // Set contact filter
            contactFilter.useTriggers = false;

            // Cast collider down and executes associated code
            RaycastHit2D[] _hit = new RaycastHit2D[16];
            _count = collider.Cast(Vector2.down, _hit, .1f);
            for (int _i = 0; _i < _count; _i++)
            {
                string _layerName = LayerMask.LayerToName(_hit[_i].transform.gameObject.layer);

                // Set isGrounded value to true if a platform is at least .1f down
                if (_layerName == "Platform")
                {
                    if (!isGrounded) IsGrounded = true;
                }
            }
        }
    }

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
        IsPlayable = isPlayable;
        this.OnHitSomething += OnHitSomethingCallback;

        StartCoroutine(OverlapCollisions());
    }
    #endregion

    #endregion
}
