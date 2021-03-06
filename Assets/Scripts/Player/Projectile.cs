﻿using System;
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

    [SerializeField]
    private float           rotationSpeed =                 .25f;


    public bool DoHit { get { return doHit; } }
    #endregion

    #region Memory & Coroutines
    private bool            doHit =                         false;

    private Coroutine       projectileMoveCoroutine =       null;

    [SerializeField]
    private Vector2         originalVelocity =              new Vector2();
    #endregion

    #region Methods

    #region Original Methods
    /***********************
     *****   METHODS   *****
     **********************/

    private IEnumerator ProjectileMove()
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

                PerformMovement(originalVelocity * Time.deltaTime * speed * 2);
            }
            else PerformMovement(originalVelocity * Time.deltaTime * speed);
        }
    }

    protected override bool CheckColliderTag(RaycastHit2D _hit, Vector2 _movement)
    {
        if (_hit.collider.gameObject.HasTag("Player") && !doHit) return false;
        return true;
    }

    public void Bounce(Vector2 _normal)
    {
        originalVelocity = Vector2.Reflect(originalVelocity, _normal);
    }

    private void CheckHit(RaycastHit2D _hit)
    {
        // Extract from collider if overlap
        ColliderDistance2D _distance = collider.Distance(_hit.collider);
        if (_distance.isOverlapped)
        {
            Vector2 _movement = _distance.pointA - _distance.pointB;
            _movement = _movement.normalized * (_movement.magnitude - Physics2D.defaultContactOffset);
            transform.position = (Vector2)transform.position - _movement;
            rigidbody.MovePosition(rigidbody.position - _movement);
        }

        if (_hit.collider.gameObject.HasTag("Player") && doHit)
        {
            _hit.collider.GetComponent<MyPlayercontroller>().Kill(originalVelocity);
            DestroyProjectile();
            return;
        }

        if (_hit.collider.gameObject.HasTag("Projectile"))
        {
            _hit.collider.GetComponent<Projectile>().Bounce(originalVelocity);
        }

        bounceCount--;

        if ((bounceCount < 1) || _hit.collider.gameObject.HasTag("Spike"))
        {
            DestroyProjectile();
            return;
        }

        Bounce(_hit.normal);
    }

    public void DestroyProjectile()
    {
        // Feedback time

        if (projectileMoveCoroutine != null)
        {
            StopCoroutine(projectileMoveCoroutine);
            projectileMoveCoroutine = null;
        }

        GameManager.PlayClipAtPoint(GameManager.I?.OrganicImpact, transform.position);
        Destroy(gameObject);
    }

    public void Init(Vector2 _direction)
    {
        originalVelocity = _direction.normalized;
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
