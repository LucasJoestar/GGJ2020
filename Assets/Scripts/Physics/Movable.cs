using EnhancedEditor;
using System;
using System.Collections;
using UnityEngine;

public class Movable : MonoBehaviour
{
    #region Events
    public event Action<RaycastHit2D>   OnHitSomething =        null;
    #endregion

    #region Fields / Properties
    /*********************
     ***   CONSTANTS   ***
     ********************/

    public const float                  MinMovementDistance =   .001f;

    /**********************
     *****   FIELDS   *****
     *********************/

    [SerializeField, HorizontalLine(order = 0), Section("PARAMETERS", 50, 0, order = 1), Space(order = 2)]
    protected bool                      isFacingRight =         true;

    [SerializeField]
    protected bool                      isGrounded =            true;

    [SerializeField, PropertyField]
    protected bool                      useGravity =            false;

    [SerializeField, Min(0)]
    protected float                     speed =                 3;


    [SerializeField, HorizontalLine(2, SuperColor.Chocolate, order = 0), Section("REFERENCES", 50, 0, order = 1), Space(order = 2)]
    protected new Collider2D            collider =              null;

    [SerializeField, HideInInspector]
    protected ContactFilter2D           contactFilter =         new ContactFilter2D();

    [SerializeField]
    protected new Rigidbody2D           rigidbody =             null;

    [SerializeField]
    private Warp                        currentWarp =           null;


    [SerializeField, HorizontalLine(2, SuperColor.Crimson, order = 0), Section("VELOCITY", 50, 0, order = 1), Space(order = 2)]
    protected Vector2                   velocity =              Vector2.zero;


    /**********************
     ***   PROPERTIES   ***
     *********************/

    public virtual bool IsGrounded
    {
        get { return isGrounded; }
        protected set
        {
            isGrounded = value;
        }
    }

    public bool UseGravity
    {
        get { return useGravity; }
        protected set
        {
            useGravity = value;

            #if UNITY_EDITOR
            if (!Application.isPlaying) return;
            #endif

            if (value)
            {
                if (applyGravityCoroutine == null) applyGravityCoroutine = StartCoroutine(ApplyGravity());
            }
            else if (applyGravityCoroutine != null)
            {
                StopCoroutine(applyGravityCoroutine);
                applyGravityCoroutine = null;
            }
        }
    }
    #endregion

    #region Memory & Coroutines
    /**********************
     ***   COROUTINES   ***
     *********************/

    protected Coroutine                 applyGravityCoroutine =     null;

    protected Coroutine                 moveCoroutine =             null;
    #endregion

    #region Methods

    #region Original Methods
    /****************************
     *********   FLIP   *********
     ***************************/

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y * -1, transform.localEulerAngles.z);
    }


    /***************************
     ******   MOVEMENTS   ******
     **************************/

    protected virtual IEnumerator ApplyGravity()
    {
        while (true)
        {
            yield return null;

            if (velocity.y > -20) velocity.y += Physics2D.gravity.y * Time.deltaTime;
            PerformMovement(velocity * Time.deltaTime);
        }
    }

    protected virtual IEnumerator MoveInDirectionTillHit(Vector2 _direction)
    {
        while (true)
        {
            yield return null;
            if (MoveInDirection(_direction)) break;
        }

        moveCoroutine = null;
    }

    protected virtual bool CheckColliderTag(RaycastHit2D _hit, Vector2 _movement)
    {
        return true;
    }

    protected virtual float GetSpeed()
    {
        return speed * Time.deltaTime;
    }

    protected bool MoveInDirection(Vector2 _direction)
    {
        return PerformMovement(_direction * GetSpeed());
    }

    protected bool PerformMovement(Vector2 _movement)
    {
        // Create variables
        bool _isGrounded = false;
        bool _isOverlappingWarp = false;
        int _count = 0;
        int _closestHitIndex = -1;
        float _distance = _movement.magnitude;
        RaycastHit2D[] _hitResults = new RaycastHit2D[16];

        // Return if not enough movement
        if (_distance < MinMovementDistance) return false;

        // Cast collider and collide on obstacles
        contactFilter.useTriggers = true;
        _count = collider.Cast(_movement, contactFilter, _hitResults, _distance + Physics2D.defaultContactOffset);
        contactFilter.useTriggers = false;
        for (int _i = 0; _i < _count; _i++)
        {
            // Check warp
            if (_hitResults[_i].collider.isTrigger)
            {
                if (_hitResults[_i].collider.gameObject.HasTag("Warp") && collider.Distance(_hitResults[_i].collider).isOverlapped)
                {
                    _isOverlappingWarp = true;
                    if (currentWarp == null) currentWarp = _hitResults[_i].collider.GetComponent<Warp>();
                }
                continue;
            }

            if (!CheckColliderTag(_hitResults[_i], _movement)) continue;

            // Cache normal hit
            Vector2 _normal = _hitResults[_i].normal;

            // Y movement related calculs
            if (Mathf.Approximately(_normal.y, 1))
            {
                _isGrounded = true;
                velocity.y = 0;
            }
            else if (Mathf.Approximately(_normal.y, -1))
            {
                velocity.y = 0;
            }

            // Get minimum distance
            float _newDistance = _hitResults[_i].distance - Physics2D.defaultContactOffset;
            if (_newDistance < _distance)
            {
                _distance = _newDistance;
                _closestHitIndex = _i;
            }
        }

        // Teleport if needed
        if (!_isOverlappingWarp && (currentWarp != null))
        {
            currentWarp.TryToTeleport(this);
            currentWarp = null;
        }

        // Return if not enough movement
        if (_distance < MinMovementDistance)
        {
            OnHitSomething?.Invoke(_hitResults[_closestHitIndex]);
            return true;
        }

        // Set isGrounded
        if ((_isGrounded != isGrounded) && (_movement.y != 0))
        {
            IsGrounded = _isGrounded;
        }

        // Flip if needed
        if ((_movement.x != 0) && (Mathf.Sign(_movement.x) != (isFacingRight ? 1 : -1))) Flip();

        // Move the object
        _movement = _movement.normalized * _distance;
        transform.position = (Vector2)transform.position + _movement;
        rigidbody.position += _movement;

        // Return if hit something or not
        if (_closestHitIndex > -1)
        {
            OnHitSomething?.Invoke(_hitResults[_closestHitIndex]);
            return true;
        }
        return false;
    }


    public void StartMoving(Vector2 _direction)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
    }

    public void StopMoving()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }
    #endregion

    #region Unity Methods
    /*****************************
     *****   UNITY METHODS   *****
     ****************************/

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Set contact filter
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);

        // Set gravity
        if (useGravity) UseGravity = true;
    }
    #endregion

    #endregion
}
