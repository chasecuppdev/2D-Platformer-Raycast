﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

public class PlayerAnimator : AnimatorController
{
    protected override void Start()
    {
        base.Start();
        MessageKit.addObserver(EventTypes.JUMP_INPUT_DOWN, JumpAnimation);
        MessageKit<string, string>.addObserver(EventTypes.ATTACK_INPUT_DOWN_2P, TriggerAttackAnimation);
        MessageKit<string, string>.addObserver(EventTypes.DASH_ATTACK_INPUT_DOWN_2P, TriggerDashAttackAnimation);
    }

    protected override void LateUpdate()
    {
        animator.SetBool("IsJumping", false);
        base.LateUpdate();
    }

    protected override void UpdateAnimationStates()
    {
        base.UpdateAnimationStates();
        animationStates.isJumping = animator.GetBool("IsJumping");
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

    /// <summary>
    /// Triggers the AttackAnimation coroutine. animationClipInfo[0] = Animation_Clip_Name, animationClipInfo[1] = Animator_Parameter_Name
    /// </summary>
    /// <param name="animationClipInfo"></param>
    protected void TriggerDashAttackAnimation(string clipName, string clipParameter)
    {
        if (!animationStates.isDashAttacking)
        {
            AttackCoroutine = AttackAnimation(clipName, clipParameter);
            StartCoroutine(AttackCoroutine);
        }
    }

    /// <summary>
    /// Calls the AttackAnimation coroutine. animationClipInfo[0] = Animation_Clip_Name, animationClipInfo[1] = Animator_Parameter_Name
    /// </summary>
    /// <param name="animationClipInfo"></param>
    /// <returns></returns>
    protected IEnumerator DashAttackAnimation(string clipName, string clipParameter)
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
            animationStates.isAttacking = true;
            animator.SetBool("IsFalling", false);
            animator.SetBool(clipParameter, true);

            yield return new WaitForSeconds(currentAnimationClip.length);

            animator.SetBool(clipParameter, false);
            animationStates.isAttacking = false;
            currentAnimationClip = null;
        }
    }
}
