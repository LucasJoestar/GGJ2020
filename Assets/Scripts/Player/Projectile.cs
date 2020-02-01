using System;
using System.Collections;
using UnityEngine;

public class Projectile : Movable
{
    #region Fields / Properties
    /**********************
     *****   FIELDS   *****
     *********************/

    [SerializeField]
    private int             bounceCount =                   5;
    #endregion

    #region Memory & Coroutines
    private Coroutine       projectileMoveCoroutine =       null;
    #endregion

    #region Methods

    #region Original Methods
    /***********************
     *****   METHODS   *****
     **********************/

    private IEnumerator ProjectileMove()
    {
        while (true)
        {
            yield return null;
            PerformMovement(velocity);
        }
    }

    public void Bounce(Vector3 _normal)
    {
        velocity = Vector2.Reflect(velocity, _normal);
    }

    private void CheckHit(RaycastHit2D _hit)
    {
        if (_hit.collider.gameObject.HasTag("Player"))
        {
            _hit.collider.GetComponent<MyPlayercontroller>().Kill();
            DestroyProjectile();
            return;
        }

        if (_hit.collider.gameObject.HasTag("Projectile"))
        {
            _hit.collider.GetComponent<Projectile>().Bounce(velocity);
        }

        bounceCount--;

        if ((bounceCount < 1) || _hit.collider.gameObject.HasTag("Pic"))
        {
            DestroyProjectile();
            return;
        }
    }

    public void DestroyProjectile()
    {
        // Feedback time

        if (projectileMoveCoroutine != null)
        {
            StopCoroutine(projectileMoveCoroutine);
            projectileMoveCoroutine = null;
        }

        Destroy(gameObject);
    }

    public void Init(Vector2 _direction)
    {
        velocity = _direction.normalized;
        projectileMoveCoroutine = StartCoroutine(ProjectileMove());
    }
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (useGravity) UseGravity = false;
        OnHitSomething += CheckHit;
    }
    #endregion

    #endregion
}
