using System.Collections;
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
}
