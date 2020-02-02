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


    [SerializeField]
    private Animator                animator =                      null;
    #endregion

    #region Memory & Coroutines
    private bool                    doHit =                         false;

    private Coroutine               BallMoveCoroutine =             null;


    public bool DoHit { get { return doHit; } }
    #endregion

    #region Methods
    /***********************
     *****   METHODS   *****
     **********************/
    private IEnumerator Activate()
    {
        OnHitSomething += (RaycastHit2D _hit) =>
        {
            transform.position = _hit.point;
            UseGravity = false;
            animator.SetTrigger("Activate");
        };

        float _timer = 0;
        while (true)
        {
            if (!doHit)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);

                yield return null;
                _timer += Time.deltaTime;
                if (_timer >= .5f) doHit = true;

                continue;
            }

            yield return new WaitForSeconds(destructionTime);
            animator.SetTrigger("Activate");
            yield break;
        }
    }

    protected override bool CheckColliderTag(RaycastHit2D _hit, Vector2 _movement)
    {
        if (_hit.collider.gameObject.HasTag("Player") && !doHit) return false;
        return true;
    }

    public void Destroy() => Destroy(gameObject);

    public void DestroyBall()
    {
        if (BallMoveCoroutine != null)
        {
            StopCoroutine(BallMoveCoroutine);
            BallMoveCoroutine = null;
        }

        // Feedback time
        animator.SetTrigger("Activate");
    }

    public void Init(bool _isFacingRight)
    {
        velocity = Vector2.left * (_isFacingRight ? 1 : -1) * speed;
        BallMoveCoroutine = StartCoroutine(Activate());
    }
    #endregion
}
