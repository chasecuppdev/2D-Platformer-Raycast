using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAI : EnemyAI
{
    ProjectileAttack projectileAttack;

    protected override void Start()
    {
        base.Start();
        projectileAttack = GetComponentInChildren<ProjectileAttack>();
    }

    protected override void Attack()
    {
        animController.TriggerAttackAnimation(DroneAnimationClips.Attack1Animation, DroneAnimationParameters.Attack1Parameter);
    }

    public void DroneTriggerAttack()
    {
        projectileAttack.OnAttackStart();
    }

    protected override void Patrol()
    {
        if (!currentlyFollowingTarget)
        {
            if (currentDistance < patrolDistance)
            {
                currentDistance += Mathf.Abs(movementController.velocity.x) * Time.deltaTime;
            }
            else
            {
                patrolDirection = -patrolDirection;
                horizontalDirection = -horizontalDirection;
                currentDistance = 0;
            }


            //Use raycast to check if we are about to fly into a wall
            //Adding 0.5f to min.y due to floating point precision sometimes spawning the raycast inside the obstacle collider
            Vector2 rayOrigin = (patrolDirection == -1) ? new Vector2(collider.bounds.min.x, collider.bounds.min.y + 0.5f) : new Vector2(collider.bounds.max.x, collider.bounds.min.y + 0.5f);
            Debug.DrawRay(rayOrigin, Vector3.right * patrolDirection);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.right * patrolDirection, 1f, ObstacleMask | PlatformMask);

            if (hit || movementController.controller2D.collisions.right || movementController.controller2D.collisions.left)
            {
                currentDistance = 0;
                patrolDirection = -patrolDirection;
                horizontalDirection = -horizontalDirection;
            }
        }
    }
}
