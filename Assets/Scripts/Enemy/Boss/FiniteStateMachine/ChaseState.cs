using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private Boss _boss;
    private MovementController movementController;
    private BoxCollider2D collider;
    private BossAnimator animController;
    private GameObject player;
    private Vector2 horizontalDirection;

    public ChaseState(Boss boss) : base(boss.gameObject)
    {
        _boss = boss;
        movementController = _boss.GetComponent<MovementController>();
        collider = _boss.GetComponent<BoxCollider2D>();
        animController = _boss.GetComponent<BossAnimator>();
        player = GameObject.Find("Player");
    }

    public override Type Tick()
    {
        var canAttack = CheckAttackDistance();
        var isDead = animController.animationStates.isDead;

        if (isDead)
        {
            return typeof(DeathState);
        }
        else if (canAttack)
        {
            return typeof(AttackState);
        }

        UpdateDirection();
        movementController.SetDirectionalInput(horizontalDirection);

        return null;
    }

    public bool CheckAttackDistance()
    {
        int directionX = movementController.controller2D.collisions.faceDir;
        Vector3 attackRayOrigin = collider.bounds.center + ((Vector3.right * directionX) * collider.bounds.extents.x);
        Vector3 targetDirection = (player.transform.position - attackRayOrigin).normalized * BossSettings.AttackDistance;
        Debug.DrawRay(attackRayOrigin, targetDirection);

        RaycastHit2D hit = Physics2D.Raycast(attackRayOrigin, targetDirection.normalized, BossSettings.AttackDistance, BossSettings.DetectionMask);

        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks player position and faces towards them
    /// </summary>
    protected virtual void UpdateDirection()
    {
        //if (CanAttack())
        //{
        if (player.transform.position.x > transform.position.x)
        {
            horizontalDirection = Vector2.right;
        }
        else
        {
            horizontalDirection = Vector2.left;
        }
        //}
    }
}
