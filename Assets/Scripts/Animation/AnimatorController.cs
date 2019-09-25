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
    protected Controller2D controller;
    protected MovementController movementController;
    protected Animator animator;
    protected SpriteRenderer sprite;
    protected JumpController jumpController;
    protected AnimationClip[] animationClips;
    protected AnimationClip currentAnimationClip;
    public AnimationStates animationStates;

    private IEnumerator AttackCoroutine;

    protected bool facingRight = true;

    public struct AnimationStates
    {
        public float speed;
        public bool isJumping;
        public bool isFalling;
        public bool isLanding;
        public bool isAttacking;
        public bool isTakingDamage;
    }

    protected virtual void Start()
    {
        //MessageKit.addObserver(EventTypes.JUMP_INPUT_DOWN, JumpAnimation);
        //MessageKit<string[]>.addObserver(EventTypes.ATTACK_INPUT_DOWN_1P, TriggerAttackAnimation);
        controller = GetComponent<Controller2D>();
        movementController = GetComponent<MovementController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        animationClips = AnimationUtility.GetAnimationClips(animator.gameObject);
        UpdateAnimationStates();

        if (GetComponent<JumpController>() != null)
        {
            if (GetComponent<JumpController>().isActiveAndEnabled)
            {
                jumpController = GetComponent<JumpController>();
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        
        CheckFallingAndLanding();
        DirectionController();
        RunAnimation();
        UpdateAnimationStates();
    }

    /// <summary>
    /// Using LateUpdate to reset animation flags
    /// </summary>
    protected virtual void LateUpdate()
    {
        animator.SetBool("IsLanding", false);
    }

    protected virtual void UpdateAnimationStates()
    {
        animationStates.speed = animator.GetFloat("Speed");
        animationStates.isJumping = animator.GetBool("IsJumping");
        animationStates.isFalling = animator.GetBool("IsFalling");
        animationStates.isLanding = animator.GetBool("IsLanding");
        animationStates.isAttacking = animator.GetBool("IsAttacking");
        animationStates.isTakingDamage = animator.GetBool("TakeDamage");
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

    public void TriggerDieAnimation(string clipName, string clipParameter)
    {
        StartCoroutine(DieAnimation(clipName, clipParameter));
    }

    private IEnumerator DieAnimation(string clipName, string clipParameter)
    {
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == clipName)
            {
                currentAnimationClip = animationClips[i];
            }
        }
        animator.SetBool("IsDead", true);

        if (currentAnimationClip)
        {
            animator.SetBool(clipParameter, true);

            yield return new WaitForSeconds(currentAnimationClip.length);

            animator.SetBool(clipParameter, false);
            currentAnimationClip = null;
            Destroy(gameObject);
        }
    }

    public void TriggerTakeDamageAnimation(string[] animationClipInfo)
    {
        StartCoroutine(TakeDamageAnimation(animationClipInfo));
    }

    private IEnumerator TakeDamageAnimation(string[] animationClipInfo)
    {
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == animationClipInfo[0])
            {
                currentAnimationClip = animationClips[i];
            }
        }

        if (currentAnimationClip)
        {
            StopCoroutine(AttackCoroutine);
            animator.SetBool("IsLanding", false);
            animator.SetBool("IsAttacking", false);
            animator.SetBool(animationClipInfo[1], true);

            yield return new WaitForSeconds(currentAnimationClip.length);

            animator.SetBool(animationClipInfo[1], false);
            currentAnimationClip = null;
        }
    }

    /// <summary>
    /// Triggers the AttackAnimation coroutine. animationClipInfo[0] = Animation_Clip_Name, animationClipInfo[1] = Animator_Parameter_Name
    /// </summary>
    /// <param name="animationClipInfo"></param>
    public void TriggerAttackAnimation(string[] animationClipInfo)
    {
        if (controller.collisions.below && !controller.collisions.slidingDownMaxSlope)
        {
            if (!animationStates.isAttacking)
            {
                AttackCoroutine = AttackAnimation(animationClipInfo);
                StartCoroutine(AttackCoroutine);
            }
        }
    }

    /// <summary>
    /// Calls the AttackAnimation coroutine. animationClipInfo[0] = Animation_Clip_Name, animationClipInfo[1] = Animator_Parameter_Name
    /// </summary>
    /// <param name="animationClipInfo"></param>
    /// <returns></returns>
    public IEnumerator AttackAnimation(string[] animationClipInfo)
    {
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == animationClipInfo[0])
            {
                currentAnimationClip = animationClips[i];
            }
        }
    
        if (currentAnimationClip)
        {
            animationStates.isAttacking = true;
            animator.SetBool("IsFalling", false);
            animator.SetBool(animationClipInfo[1], true);

            yield return new WaitForSeconds(currentAnimationClip.length);
    
            animator.SetBool(animationClipInfo[1], false);
            animationStates.isAttacking = false;
            currentAnimationClip = null;
        }
    }

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
