using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SplicerAI : MonoBehaviour
{
    GameObject player;
    MovementController movementController;
    SplicerStandardAttack standardAttack;
    AnimatorController animController;
    [SerializeField] BoxCollider2D collider;

    //LayerMasks
    public LayerMask detectionMask;

    int ObstacleLayer = 9;
    int PlatformLayer = 10;
    int PlayerHurtboxLayer = 16;

    int ObstacleMask;
    int PlatformMask;
    int PlayerHurtboxMask;

    private Vector2 horizontalDirection;

    [SerializeField] private float patrolDistance;
    private float currentDistance;
    private bool currentlyFollowingTarget = false;

    [SerializeField] private float attackdDetectionLength;
    [SerializeField] private float aggroRange;

    [SerializeField] private bool patrollingEnabled;

    string[] attackAnimationInfo = new string[2];

    private Vector3 PlayerPosition
    {
        get { return player.transform.position; }
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        movementController = GetComponent<MovementController>();
        standardAttack = GetComponent<SplicerStandardAttack>();
        animController = GetComponent<AnimatorController>();

        attackAnimationInfo[0] = "Enemy2_Attack";
        attackAnimationInfo[1] = "IsAttacking";

        ObstacleMask = 1 << ObstacleLayer;
        PlatformMask = 1 << PlatformLayer;
        PlayerHurtboxMask = 1 << PlayerHurtboxLayer;

        if (patrollingEnabled)
        {
            horizontalDirection = Vector2.right;
        }
    }

    private void FixedUpdate()
    {
        ScanForTarget();
        AttackWhenInRange();
        if ((movementController.controller2D.collisions.right && horizontalDirection.x > 0) || (movementController.controller2D.collisions.left && horizontalDirection.x < 0))
        {
            movementController.SetDirectionalInput(Vector2.zero);
        }
        else
        {
            movementController.SetDirectionalInput(horizontalDirection);
        }
    }

    public bool CanAttack()
    {
        if (animController.animationStates.isAttacking || animController.animationStates.isTakingDamage)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Patrol()
    {
        if (!currentlyFollowingTarget)
        {
            if (currentDistance < patrolDistance)
            {
                currentDistance += Mathf.Abs(movementController.velocity.x) * Time.deltaTime;
                Debug.Log("currentDistance: " + currentDistance);
            }
            else
            {
                horizontalDirection = -horizontalDirection;
                currentDistance = 0;
            }

            int directionX = movementController.controller2D.collisions.faceDir;
            Vector2 rayOrigin = (directionX == -1) ? new Vector2(collider.bounds.min.x, collider.bounds.min.y) : new Vector2(collider.bounds.max.x, collider.bounds.min.y);
            Debug.DrawRay(rayOrigin, Vector3.down);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.down, 1, ObstacleMask | PlatformMask);

            if (!hit || movementController.controller2D.collisions.right || movementController.controller2D.collisions.left)
            {
                currentDistance = 0;
                horizontalDirection = -horizontalDirection;
            }
        }
    }

    private void ScanForTarget()
    {
        Vector3 aggroRayOrigin = collider.bounds.center;
        Vector3 targetDirection = (PlayerPosition - aggroRayOrigin).normalized * aggroRange;

        RaycastHit2D hit = Physics2D.Raycast(aggroRayOrigin, (PlayerPosition - aggroRayOrigin).normalized, aggroRange, detectionMask);
        Debug.DrawRay(aggroRayOrigin, targetDirection);

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

    private void AttackWhenInRange()
    {
        int directionX = movementController.controller2D.collisions.faceDir;
        Vector3 rayOrigin = collider.bounds.center + ((Vector3.right * directionX) * collider.bounds.extents.x);
        Debug.DrawRay(rayOrigin, (Vector3.right * directionX) * attackdDetectionLength);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, (Vector3.right * directionX), attackdDetectionLength, detectionMask);

        if (hit)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox") && CanAttack())
            {
                animController.TriggerAttackAnimation(attackAnimationInfo);
            }
        }
    }

    /// <summary>
    /// Checks player position and faces towards them
    /// </summary>
    private void UpdateDirection()
    {
        if (PlayerPosition.x > transform.position.x)
        {
            horizontalDirection =  Vector2.right;
        }
        else
        {
            horizontalDirection = Vector2.left;
        }
    }

    private void OnDrawGizmos()
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
