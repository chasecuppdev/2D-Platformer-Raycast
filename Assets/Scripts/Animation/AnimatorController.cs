using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prime31.MessageKit;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimatorController : MonoBehaviour
{
    Controller2D controller;
    MovementController movementController;
    Animator animator;
    SpriteRenderer sprite;
    JumpController jumpController;
    AnimationClip[] animationClips;
    AnimationClip currentAnimationClip;

    bool facingRight = true;
    int rememberFaceDir;

    protected virtual void Start()
    {
        //MessageKit.addObserver(EventTypes.JUMP_INPUT_DOWN, JumpAnimation);
        //MessageKit<string[]>.addObserver(EventTypes.ATTACK_INPUT_DOWN_1P, TriggerAttackAnimation);
        controller = GetComponent<Controller2D>();
        movementController = GetComponent<MovementController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        animationClips = AnimationUtility.GetAnimationClips(animator.gameObject);

        if (GetComponent<JumpController>() != null)
        {
            if (GetComponent<JumpController>().isActiveAndEnabled)
            {
                jumpController = GetComponent<JumpController>();
            }
        }
    }

    private void FixedUpdate()
    {
        CheckFallingAndLanding();
        DirectionController();
        RunAnimation();
    }

    /// <summary>
    /// Using LateUpdate to reset animation flags
    /// </summary>
    private void LateUpdate()
    {
        animator.SetBool("IsLanding", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("TakeDamage", false);
    }

    /// <summary>
    /// Determines when to play the running animation
    /// </summary>
    protected void RunAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(movementController.velocity.x));
        animator.SetFloat("DirectionalInput", Mathf.Abs(movementController.directionalInput.x));

        //if (Mathf.Abs(movementController.velocity.x) > 0.1f)
        //{
        //    if (controller.collisions.below)
        //    {
        //        animator.SetBool("IsRunning", true);
        //    }
        //}
        //else
        //{
        //    animator.SetBool("IsRunning", false);
        //}
    }

    protected void JumpAnimation()
    {
        if (jumpController)
        {
            if (controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)
                {
                    if (movementController.directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                    {
                        animator.SetBool("IsJumping", true);
                    }
                }
                else
                {
                    animator.SetBool("IsJumping", true);
                }
            }
        }
    }

    public void TakeDamageAnimation()
    {
        animator.SetBool("TakeDamage", true);
    }

    /// <summary>
    /// Triggers the AttackAnimation coroutine. animationClipInfo[0] = Animation_Clip_Name, animationClipInfo[1] = Animator_Parameter_Name
    /// </summary>
    /// <param name="animationClipInfo"></param>
    public void TriggerAttackAnimation(string[] animationClipInfo)
    {
        if (!movementController.isAttacking)
        {
            //movementController.isAttacking = true;
            rememberFaceDir = movementController.controller2D.collisions.faceDir;
            StartCoroutine(AttackAnimation(animationClipInfo));
            //animator.Play("Enemy2_Attack");
        }
    }

    /// <summary>
    /// Calls the AttackAnimation coroutine. animationClipInfo[0] = Animation_Clip_Name, animationClipInfo[1] = Animator_Parameter_Name
    /// </summary>
    /// <param name="animationClipInfo"></param>
    /// <returns></returns>
    IEnumerator AttackAnimation(string[] animationClipInfo)
    {
        float elapsed = 0f;
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == animationClipInfo[0])
            {
                currentAnimationClip = animationClips[i];
            }
        }

        if (currentAnimationClip)
        {
            if (controller.collisions.below && !controller.collisions.slidingDownMaxSlope)
            {
                movementController.isAttacking = true;
                animator.SetBool("IsFalling", false);
                animator.SetBool(animationClipInfo[1], true);

                while (elapsed < currentAnimationClip.length)
                {
                    elapsed = elapsed + Time.deltaTime;
                    yield return 0;
                }

                animator.SetBool(animationClipInfo[1], false);
                movementController.isAttacking = false;
                currentAnimationClip = null;
                movementController.controller2D.collisions.faceDir = rememberFaceDir;
            }
        }
    }

    //public void AttackAnimationEnd()
    //{
    //    animator.SetBool("IsAttacking", false);
    //    movementController.isAttacking = false;
    //}

    /// <summary>
    /// Simple check to see if the character is falling or landing in order to play the correct animations
    /// </summary>
    protected void CheckFallingAndLanding()
    {
        if (movementController.velocity.y < 0 && !controller.collisions.below)
        {
            animator.SetBool("IsFalling", true);
        }

        if (animator.GetBool("IsFalling") && controller.collisions.below)
        {
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsLanding", true);
        }
    }

    /// <summary>
    /// Checks the horizontal velocity direction and ensures the sprite is facing in that direction
    /// </summary>
    protected void DirectionController()
    {
        //Have to check to see if this is greater than a relatively small value due to floating point precision errors
        if (Mathf.Abs(movementController.velocity.x) > 0.01f )
        {
            float directionX = Mathf.Sign(movementController.velocity.x); //Get the horizontal direction of movement

            if (directionX == 1)
            {
                if (!facingRight)
                {
                    //attackPosition.localPosition.Set(-attackPosition.transform.position.x, attackPosition.transform.position.y, 0);
                    //sprite.flipX = false;
                    transform.localScale = new Vector3(1,1,1);
                    facingRight = true;
                }
            }
            else if (directionX == -1)
            {
                if (facingRight)
                {
                    //attackPosition.transform.Translate(new Vector2(-attackPosition.transform.position.x, attackPosition.transform.position.y));
                    //sprite.flipX = true;
                    transform.localScale = new Vector3(-1, 1, 1);
                    facingRight = false;
                }
            }
        }
        //Remember the direction after the attack animation
        //else 
        //{
        //    if (hitbox.currentAttackPosition == hitbox.leftAttackPosition )
        //    {
        //        sprite.flipX = true;
        //        facingRight = false;
        //    }
        //
        //}
    }
}
