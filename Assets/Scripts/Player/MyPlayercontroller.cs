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
    private Animator                        animator =              null;

    [SerializeField]
    private Warp                            currentWarp =           null;


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

            if (!value) Die();
        }
    }

    public override bool IsGrounded
    {
        get { return base.IsGrounded; }
        protected set
        {
            base.IsGrounded = value;

            animator?.SetBool("IsGrounded", value);
        }
    }

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

    /**************************
     *******   REPAIR   *******
     *************************/

    private void Die()
    {
        // Stop what needs to be
        StopJump();

        // Play sound & animations
        animator.SetTrigger("Die");

        // Increase score
        GameManager.I.IncreaseScore(!playerInputs.IsPlayerOne);
    }

    public void Kill()
    {
        if (isDead) return;
        IsDead = true;

        Debug.Log(name + " Player is Dead !!");
    }


    /***************************
     ******   MOVEMENTS   ******
     **************************/

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
        bool _isOverlappingWarp;
        int _count;
        Collider2D[] _colliders = new Collider2D[16];

        while (true)
        {
            yield return null;
            _isOverlappingWarp = false;

            // Set contact filter
            contactFilter.useTriggers = true;

            // Extract player from overlapping colliders
            _count = collider.OverlapCollider(contactFilter, _colliders);
            for (int _i = 0; _i < _count; _i++)
            {
                if (_colliders[_i].isTrigger)
                {
                    if (_colliders[_i].gameObject.HasTag("Warp"))
                    {
                        _isOverlappingWarp = true;
                        if (currentWarp == null) currentWarp = _colliders[_i].GetComponent<Warp>();
                    }

                    continue;
                }

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

            // Teleport if needed
            if (!_isOverlappingWarp && (currentWarp != null))
            {
                currentWarp.TryToTeleport(this);
                currentWarp = null;
            }

            // Cast collider down and executes associated code
            RaycastHit2D[] _hit = new RaycastHit2D[1];
            if (collider.Cast(Vector2.down, _hit, .1f) > 0)
            {
                string _layerName = LayerMask.LayerToName(_hit[0].transform.gameObject.layer);

                // Set isGrounded value to true if a platform is at least .1f down
                if (_layerName == "Platform")
                {
                    if (!isGrounded) IsGrounded = true;
                }
                // Kill player if jumped on his head
                else if (_layerName == "Player")
                {
                    Debug.Log("Hit Player");
                    if (Mathf.Approximately(_hit[0].normal.y, 1)) _hit[0].transform.GetComponent<MyPlayercontroller>()?.Kill();
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
        IsPlayable = true;
        this.OnHitSomething += OnHitSomethingCallback;

        StartCoroutine(OverlapCollisions());
    }
    #endregion

    #endregion
}
