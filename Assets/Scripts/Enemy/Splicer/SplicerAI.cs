using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SplicerAI : EnemyAI
{
    protected override void Attack()
    {
        animController.TriggerAttackAnimation(SplicerAnimationClips.Attack1Animation, SplicerAnimationParameters.Attack1Parameter);
    }

    protected override void TryToMoveTowardsTarget()
    {
        Vector3 aggroRayOrigin = collider.bounds.center;
        Vector3 targetDirection = (PlayerPosition - aggroRayOrigin).normalized * aggroRange;

        RaycastHit2D hit = Physics2D.Raycast(aggroRayOrigin, (PlayerPosition - aggroRayOrigin).normalized, aggroRange, detectionMask);
        Debug.DrawRay(aggroRayOrigin, targetDirection);

        if (hit)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox"))
            {
                currentlyFollowingTarget = true;
                RaycastHit2D ledgeHit = CheckLedges();
                if (!ledgeHit)
                {
                    horizontalDirection = Vector2.zero;
                }
                else
                {
                    UpdateDirection();
                }
                //TO-DO Need to allow the face direction to change without being tied to movement, otherwise the direction the raycast check never changes
            }
            else
            {
                currentlyFollowingTarget = false;
                if (patrollingEnabled)
                {
                    Patrol();
                }
                else
                {
                    horizontalDirection = Vector2.zero;
                }
            }
        }
        else
        {
            currentlyFollowingTarget = false;
            if (patrollingEnabled)
            {
                Patrol();
            }
            else
            {
                horizontalDirection = Vector2.zero;
            }
        }
    }
}
