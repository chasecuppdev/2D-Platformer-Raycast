using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(MovementController))]
public class PlayerInput : MonoBehaviour
{
    Vector2 directionalInput;
    MovementController movement;
    Controller2D controller;
    string[] attackParameters = new string[2];

    void Start()
    {
        controller = GetComponent<Controller2D>();
        movement = GetComponent<MovementController>();
    }

    void Update()
    {
        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //if (directionalInput.x != 0 || directionalInput.y != 0)
        //{
        //    EventManager.Instance.PostNotification(EVENT_TYPE.MOVE, this, directionalInput);
        //}
        movement.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MessageKit.post(EventTypes.JUMP_INPUT_DOWN);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            MessageKit.post(EventTypes.JUMP_INPUT_UP);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.Attack1Animation, PlayerAnimationParameters.Attack1Parameter);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (controller.collisions.below)
            {
                MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.GroundedTeleportAttackAnimation, PlayerAnimationParameters.TeleportAttackParameter);
            }
            else
            {
                MessageKit<string, string>.post(EventTypes.ATTACK_INPUT_DOWN_2P, PlayerAnimationClips.AirTeleportAttackAnimation, PlayerAnimationParameters.TeleportAttackParameter);
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
