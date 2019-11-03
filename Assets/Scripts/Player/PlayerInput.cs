using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(MovementController))]
public class PlayerInput : MonoBehaviour
{
    Vector2 directionalInput;
    MovementController movementController;
    Controller2D controller;
    JumpController jumpController;
    PlayerTeleportAttack teleportAttack;
    AnimatorController animatorController;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        movementController = GetComponent<MovementController>();
        jumpController = GetComponent<JumpController>();
        teleportAttack = GetComponentInChildren<PlayerTeleportAttack>();
        animatorController = GetComponent<AnimatorController>();
    }

    void Update()
    {
        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Debug.Log(directionalInput);
        if (directionalInput.x > 0.05f)
        {
            directionalInput.x = 1;
        }
        else if (directionalInput.x < -0.05f)
        {
            directionalInput.x = -1;
        }
        else
        {
            directionalInput.x = 0;
        }

        //if (directionalInput.x != 0 || directionalInput.y != 0)
        //{
        //    EventManager.Instance.PostNotification(EVENT_TYPE.MOVE, this, directionalInput);
        //}
        movementController.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Checking for these collision parameters here because we don't want to keep send the jump command in the air, as it resets preJump 
            if (controller.collisions.below || controller.collisions.wallSliding)
            {
                MessageKit.post(EventTypes.JUMP_INPUT_DOWN);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            MessageKit.post(EventTypes.JUMP_INPUT_UP);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (controller.collisions.below)
            {
                MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.GroundedAttack1Animation, PlayerAnimationParameters.GroundedAttack1Parameter);
            }
            else
            {
                MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.AirAttack1Animation, PlayerAnimationParameters.AirAttack1Parameter);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (controller.collisions.below)
            {
                MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.GroundedAttack2Animation, PlayerAnimationParameters.GroundedAttack2Parameter);
            }
            else
            {
                MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.AirAttack2Animation, PlayerAnimationParameters.AirAttack2Parameter);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (controller.collisions.below)
            {
                MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.GroundedAttack3Animation, PlayerAnimationParameters.GroundedAttack3Parameter);
            }
            else
            {
                MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.AirAttack3Animation, PlayerAnimationParameters.AirAttack3Parameter);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (teleportAttack != null)
            {
                if (!teleportAttack.cooldown.active)
                {
                    teleportAttack.faceDirectionSnapshot = (animatorController.facingRight) ? 1 : -1;
                    if (controller.collisions.below)
                    {
                        teleportAttack.cooldown.StartCooldown();
                        MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.GroundedTeleportAttackAnimation, PlayerAnimationParameters.GroundedTeleportAttackParameter);
                    }
                    else
                    {
                        teleportAttack.cooldown.StartCooldown();
                        MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.AirTeleportAttackAnimation, PlayerAnimationParameters.AirTeleportAttackParameter);
                    }
                }
                else
                {
                    MessageKit<string>.post(EventTypes.UI_ELEMENT_SHAKE_1P, "Teleport_Icon");
                }
            }
        }

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    attackParameters[0] = "Player_Attack_Heavy";
        //    attackParameters[1] = "IsHeavyAttack";
        //    MessageKit<string[]>.post(EventTypes.ATTACK_INPUT_DOWN_1P, attackParameters);
        //}
    }
}
