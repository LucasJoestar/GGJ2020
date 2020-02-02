using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Movable
{
    #region Fields / Properties
    /**********************
     *****   FIELDS   *****
     *********************/

    [SerializeField]
    private int                     destructionTime =               5;

    [SerializeField]
    private float                   rotationSpeed =                 .25f;
    #endregion

    #region Memory & Coroutines
    private bool                    doHit =                         false;

    private Coroutine               BallMoveCoroutine =             null;
    #endregion

    #region Methods

    #region Original Methods
    /***********************
     *****   METHODS   *****
     **********************/

    private IEnumerator BallMove()
    {
        float _timer = 0;
        while (true)
        {
            yield return null;
            transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);

            if (!doHit)
            {
                _timer += Time.deltaTime;
                if (_timer >= .5f) doHit = true;
            }
        }
    }

    protected override bool CheckColliderTag(RaycastHit2D _hit, Vector2 _movement)
    {
        if (_hit.collider.gameObject.HasTag("Player") && !doHit) return false;
        return true;
    }

    public void DestroyBall()
    {
        // Feedback time

        if (BallMoveCoroutine != null)
        {
            StopCoroutine(BallMoveCoroutine);
            BallMoveCoroutine = null;
        }

        Destroy(gameObject);
    }

    public void Init(bool _isFacingRight)
    {
        velocity = Vector2.right * (_isFacingRight ? 1 : -1) * speed;
        BallMoveCoroutine = StartCoroutine(BallMove());
    }
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    #endregion

    #endregion
}
