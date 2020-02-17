using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    protected GameObject player;
    protected MovementController movementController;
    protected AnimatorController animController;
    [SerializeField]
    protected BoxCollider2D collider;

    public float attackCooldown;
    protected float lastAttackTimestamp;

    //LayerMasks
    public LayerMask detectionMask;

    protected int ObstacleLayer = 9;
    protected int PlatformLayer = 10;
    protected int PlayerHurtboxLayer = 16;

    protected int ObstacleMask;
    protected int PlatformMask;
    protected int PlayerHurtboxMask;

    protected Vector2 horizontalDirection;

    [SerializeField]
    protected float patrolDistance = 5;
    protected float currentDistance;
    protected bool currentlyFollowingTarget = false;

    [SerializeField]
    protected float attackdDetectionLength;
    [SerializeField]
    protected float aggroRange;

    [SerializeField]
    protected bool patrollingEnabled;
    [SerializeField]
    protected Vector2 InitialDirection = Vector2.right;
    protected int patrolDirection; //Keeping up with the face direction here because it doesn't always get updated fast enough in the game loop

    protected Vector3 PlayerPosition
    {
        get { return player.transform.position; }
    }

    protected virtual void Start()
    {
        player = GameObject.Find("Player");
        movementController = GetComponent<MovementController>();
        animController = GetComponent<AnimatorController>();

        ObstacleMask = 1 << ObstacleLayer;
        PlatformMask = 1 << PlatformLayer;
        PlayerHurtboxMask = 1 << PlayerHurtboxLayer;

        if (patrollingEnabled)
        {
            horizontalDirection = Vector2.right;
            patrolDirection = (int)horizontalDirection.x;
        }
    }

    /// <summary>
    /// Since the MovementController is used for both player controlled characters and AI, we mimic input in order to move ai characters to keep it general
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (!animController.animationStates.isDead)
        {
            TryToMoveTowardsTarget();
            AttackWhenInRange();

            //Don't keep walking forward if walking into an obstacle
            if ((movementController.controller2D.collisions.right && horizontalDirection.x > 0) || (movementController.controller2D.collisions.left && horizontalDirection.x < 0))
            {
                movementController.SetDirectionalInput(Vector2.zero);
            }
            else
            {
                movementController.SetDirectionalInput(horizontalDirection);
            }
        }
        else
        {
            movementController.SetDirectionalInput(Vector2.zero);
        }
    }

    protected virtual void Attack()
    {
        //override this
    }

    protected virtual void Patrol()
    {
        if (!currentlyFollowingTarget)
        {
            RaycastHit2D hit = CheckLedges();

            if (!hit || movementController.controller2D.collisions.right || movementController.controller2D.collisions.left)
            {
                currentDistance = 0;
                patrolDirection = -patrolDirection;
                if (horizontalDirection == Vector2.zero)
                {
                    horizontalDirection.x = -movementController.controller2D.collisions.faceDir;
                }
                else
                {
                    horizontalDirection = -horizontalDirection;
                }
            }

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
        }
    }

    protected virtual void TryToMoveTowardsTarget()
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
                UpdateDirection();
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

    protected virtual void AttackWhenInRange()
    {
        int directionX = movementController.controller2D.collisions.faceDir;
        Vector3 attackRayOrigin = collider.bounds.center + ((Vector3.right * directionX) * collider.bounds.extents.x);
        Vector3 targetDirection = (PlayerPosition - attackRayOrigin).normalized * attackdDetectionLength;
        Debug.DrawRay(attackRayOrigin, targetDirection);

        RaycastHit2D hit = Physics2D.Raycast(attackRayOrigin, targetDirection.normalized, attackdDetectionLength, detectionMask);

        if (hit)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox") && animController.HasControl())
            {
                if (Time.time >= lastAttackTimestamp + attackCooldown)
                {
                    lastAttackTimestamp = Time.time;
                    Attack();
                }
            }
        }
    }

    /// <summary>
    /// Use raycast to check if we are about to walk off a ledge
    /// </summary>
    /// <returns></returns>
    protected virtual RaycastHit2D CheckLedges()
    {
        //Adding 0.5f to min.y due to floating point precision sometimes spawning the raycast inside the obstacle collider
        Vector2 rayOrigin = (movementController.controller2D.collisions.faceDir == -1) ? new Vector2(collider.bounds.min.x, collider.bounds.min.y + 0.5f) : new Vector2(collider.bounds.max.x, collider.bounds.min.y + 0.5f);
        Debug.DrawRay(rayOrigin, Vector3.down);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.down, 1f, ObstacleMask | PlatformMask);

        return hit;
    }

    /// <summary>
    /// Checks player position and faces towards them
    /// </summary>
    protected virtual void UpdateDirection()
    {
        //if (CanAttack())
        //{
        if (PlayerPosition.x > transform.position.x)
        {
            horizontalDirection = Vector2.right;
        }
        else
        {
            horizontalDirection = Vector2.left;
        }
        //}
    }

    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            //AttackRangeRay
            int directionX = 1;
            Vector3 attackRayOrigin = collider.bounds.center + ((Vector3.right * directionX) * collider.bounds.extents.x);
            Debug.DrawRay(attackRayOrigin, (Vector3.right * directionX) * attackdDetectionLength);
        }
        //else
        //{
        //    //AggroRangeRay
        //    Vector3 aggroRayOrigin = collider.bounds.center;
        //    Vector3 targetDirection = (PlayerPosition - aggroRayOrigin).normalized * aggroRange;
        //    Debug.DrawRay(aggroRayOrigin, targetDirection);
        //}
    }
}
