using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private Boss _boss;
    private BossAnimator animController;

    public AttackState(Boss boss) : base(boss.gameObject)
    {
        _boss = boss;
        animController = _boss.GetComponent<BossAnimator>();
    }

    public override Type Tick()
    {
        animController.TriggerAttackAnimation(BossAnimationClips.HammerAttackAnimation, BossAnimationParameters.HammerAttackParamater);

        return typeof(ChaseState);
    }

    //public bool CheckAttackDistance()
    //{
    //    int directionX = movementController.controller2D.collisions.faceDir;
    //    Vector3 attackRayOrigin = collider.bounds.center + ((Vector3.right * directionX) * collider.bounds.extents.x);
    //    Vector3 targetDirection = (player.transform.position - attackRayOrigin).normalized * BossSettings.AttackDistance;
    //    Debug.DrawRay(attackRayOrigin, targetDirection);
    //
    //    RaycastHit2D hit = Physics2D.Raycast(attackRayOrigin, targetDirection.normalized, BossSettings.AttackDistance, BossSettings.DetectionMask);
    //
    //    if (hit)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
