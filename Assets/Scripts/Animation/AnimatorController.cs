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

    protected IEnumerator AttackCoroutine;

    [HideInInspector]
    public bool facingRight = true;

    public struct AnimationStates
    {
        public float speed;
        public float directionalInput;
        public bool isFalling;
        public bool isLanding;
        public bool isAttacking;
        public bool isHurt;
        public bool isGrounded;
        public bool isDead;

        //Player specific state
        public bool isWallSliding;
        public bool isTeleportAttacking;
        public bool isTeleporting;
        public bool isJumping;
    }

    protected virtual void Start()
    {
        //MessageKit.addObserver(EventTypes.JUMP_INPUT_DOWN, JumpAnimation);
        //MessageKit<string[]>.addObserver(EventTypes.ATTACK_INPUT_DOWN_1P, TriggerAttackAnimation);
        controller = GetComponent<Controller2D>();
        movementController = GetComponent<MovementController>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        animationClips = animator.runtimeAnimatorController.animationClips;
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
        animationStates.directionalInput = animator.GetFloat("DirectionalInput");
        animationStates.isFalling = animator.GetBool("IsFalling");
        animationStates.isLanding = animator.GetBool("IsLanding");
        animationStates.isAttacking = animator.GetBool("IsAttacking");
        animationStates.isHurt = animator.GetBool("IsHurt");
        animationStates.isDead = animator.GetBool("IsDead");

        animator.SetBool("IsGrounded", controller.collisions.below);
        animationStates.isGrounded = animator.GetBool("IsGrounded");
    }

    public bool HasControl()
    {
        if (animationStates.isAttacking || animationStates.isHurt)
        {
            return false;
        }
        else
        {
            return true;
        }
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
            gameObject.SetActive(false);
            
            if (gameObject.name == "Player")
            {
                MessageKit.post(EventTypes.PLAYER_DIED);
            }
        }
    }

    public void TriggerTakeDamageAnimation(string clipName, string clipParameter)
    {
        StartCoroutine(TakeDamageAnimation(clipName, clipParameter));
    }

    protected virtual IEnumerator TakeDamageAnimation(string clipName, string clipParameter)
    {
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == clipName)
            {
                currentAnimationClip = animationClips[i];
            }
        }

        if (currentAnimationClip)
        {
            if (AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
            }
            animator.SetBool("IsLanding", false);
            animator.SetBool("IsAttacking", false);
            animator.SetBool(clipParameter, true);

            yield return new WaitForSeconds(currentAnimationClip.length);

            animator.SetBool(clipParameter, false);
            currentAnimationClip = null;
        }
    }

    /// <summary>
    /// Triggers the AttackAnimation coroutine. animationClipInfo[0] = Animation_Clip_Name, animationClipInfo[1] = Animator_Parameter_Name
    /// </summary>
    /// <param name="animationClipInfo"></param>
    public void TriggerAttackAnimation(string clipName, string clipParameter)
    {
        //if (controller.collisions.below && !controller.collisions.slidingDownMaxSlope)
        //{
            if (!animationStates.isAttacking)
            {
                AttackCoroutine = AttackAnimation(clipName, clipParameter);
                StartCoroutine(AttackCoroutine);
            }
        //}
    }

    /// <summary>
    /// Calls the AttackAnimation coroutine. animationClipInfo[0] = Animation_Clip_Name, animationClipInfo[1] = Animator_Parameter_Name
    /// </summary>
    /// <param name="animationClipInfo"></param>
    /// <returns></returns>
    public IEnumerator AttackAnimation(string clipName, string clipParameter)
    {
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == clipName)
            {
                currentAnimationClip = animationClips[i];
            }
        }
    
        if (currentAnimationClip)
        {
            //Calling DirectionController here is a special case for continuously attacking enemy AIs
            DirectionController(); //The way this coroutine is called, this needs to be updated just before the attack begins, as the parameters will be set before DirectionController is run again in FixedUpdate
            animationStates.isAttacking = true; //setting this state directly so that we don't have to wait for the next update 
            animator.SetBool("IsAttacking", true); //Setting this parameter for any attack so other animations know we are attacking
            animator.SetBool("IsFalling", false);
            animator.SetBool(clipParameter, true); //This is the parameter for the specific attack

            yield return new WaitForSeconds(currentAnimationClip.length);
    
            animator.SetBool(clipParameter, false);
            animator.SetBool("IsAttacking", false);
            animationStates.isAttacking = false;
            currentAnimationClip = null;
        }
    }

    /// <summary>
    /// Simple check to see if the character is falling or landing in order to play the correct animations
    /// </summary>
    protected virtual void CheckFallingAndLanding()
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
    protected virtual void DirectionController()
    {
        if (HasControl())
        { 
            //float directionX = Mathf.Sign(movementController.velocity.x); //Get the horizontal direction of movement
            float directionX = movementController.directionalInput.x; //Get the horizontal direction of movement

            if (directionX == 1)
            {
                if (!facingRight)
                {
                    transform.localScale = new Vector3(1,1,1);
                    facingRight = true;
                }
            }
            else if (directionX == -1)
            {
                if (facingRight)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    facingRight = false;
                }
            }
        }
    }
}
