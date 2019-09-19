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

    public LayerMask detectionMask;

    int ObstacleLayer = 9;
    int PlayerHurtboxLayer = 16;

    int ObstacleMask;
    int PlayerHurtboxMask;

    private Vector2 horizontalDirection;

    [SerializeField] private float detectionLength;

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
        PlayerHurtboxMask = 1 << PlayerHurtboxLayer;
    }

    private void FixedUpdate()
    {
        UpdateDirection();
        if ((movementController.controller2D.collisions.right && horizontalDirection.x > 0) || (movementController.controller2D.collisions.left && horizontalDirection.x < 0))
        {
            movementController.SetDirectionalInput(Vector2.zero);
        }
        else
        {
            movementController.SetDirectionalInput(horizontalDirection);
        }
        DetectTarget();
    }

    private void DetectTarget()
    {
        int directionX = movementController.controller2D.collisions.faceDir;
        float rayLength = detectionLength;
        Vector3 rayOrigin = collider.bounds.center + ((Vector3.right * directionX) * collider.bounds.extents.x);
        Debug.DrawRay(rayOrigin, (Vector3.right * directionX) * rayLength);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, (Vector3.right * directionX), rayLength, detectionMask);

        if (hit)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox") && !movementController.isAttacking)
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
            int directionX = 1;
            float rayLength = detectionLength;
            Vector3 rayOrigin = collider.bounds.center + ((Vector3.right * directionX) * collider.bounds.extents.x);
            Debug.DrawRay(rayOrigin, (Vector3.right * directionX) * rayLength);
        }
    }
}
