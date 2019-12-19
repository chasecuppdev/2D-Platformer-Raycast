using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31.MessageKit;
using Rewired;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(MovementController))]
public class PlayerInput : MonoBehaviour
{
    Vector2 directionalInput;
    MovementController movementController;
    Controller2D controller;
    JumpController jumpController;
    PlayerAttackInfo standardAttack;
    PlayerTeleportAttack teleportAttack;
    AnimatorController animatorController;
    [SerializeField]
    Canvas pauseMenu;

    public int rewiredPlayerId = 0;
    public Rewired.Player rewiredPlayer;

    private void Awake()
    {
        rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
        controller = GetComponent<Controller2D>();
        movementController = GetComponent<MovementController>();
        jumpController = GetComponent<JumpController>();
        standardAttack = GetComponentInChildren<PlayerAttackInfo>();
        teleportAttack = GetComponentInChildren<PlayerTeleportAttack>();
        animatorController = GetComponent<AnimatorController>();
    }

    void Update()
    {
        directionalInput = new Vector2(rewiredPlayer.GetAxisRaw("Move Horizontal"), rewiredPlayer.GetAxisRaw("Move Vertical"));
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

        if (rewiredPlayer.GetButtonDown("Jump"))
        {
            //Checking for these collision parameters here because we don't want to keep send the jump command in the air, as it resets preJump 
            if (controller.collisions.below || controller.collisions.wallSliding)
            {
                MessageKit.post(EventTypes.JUMP_INPUT_DOWN);
            }
        }

        if (rewiredPlayer.GetButtonUp("Jump"))
        {
            MessageKit.post(EventTypes.JUMP_INPUT_UP);
        }
        
        if (rewiredPlayer.GetButtonDown("Attack"))
        {
            if (teleportAttack != null
                && !animatorController.animationStates.isAttacking
                && !animatorController.animationStates.isWallSliding
                && !animatorController.animationStates.isHurt)
            {
                if (!standardAttack.cooldown.active)
                {
                    if (controller.collisions.below)
                    {
                        standardAttack.cooldown.StartCooldown();
                        MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.GroundedAttack1Animation, PlayerAnimationParameters.GroundedAttack1Parameter);
                    }
                    else
                    {
                        standardAttack.cooldown.StartCooldown();
                        MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.AirAttack1Animation, PlayerAnimationParameters.AirAttack1Parameter);
                    }
                }
                else
                {
                    MessageKit<string>.post(EventTypes.UI_ELEMENT_SHAKE_1P, "Energy_Portrait");
                }
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

        if (rewiredPlayer.GetButtonDown("Teleport Attack"))
        {
            if (teleportAttack != null 
                && !animatorController.animationStates.isAttacking 
                && !animatorController.animationStates.isWallSliding 
                && !animatorController.animationStates.isHurt)
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

        if (rewiredPlayer.GetButtonDown("Pause"))
        {
            MessageKit.post(EventTypes.UI_PAUSE_MENU);
        }

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    attackParameters[0] = "Player_Attack_Heavy";
        //    attackParameters[1] = "IsHeavyAttack";
        //    MessageKit<string[]>.post(EventTypes.ATTACK_INPUT_DOWN_1P, attackParameters);
        //}
    }
}
