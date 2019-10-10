using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(MovementController))]
public class JumpController : MonoBehaviour
{
    Controller2D controller;
    GravityController gravityController;
    MovementController movementController;

    private void Start()
    {
        //MessageKit.addObserver(EventTypes.JUMP_INPUT_DOWN, OnJumpInputDown);
        MessageKit.addObserver(EventTypes.JUMP_INPUT_UP, OnJumpInputUp);
        controller = GetComponent<Controller2D>();
        gravityController = GetComponent<GravityController>();
        movementController = GetComponent<MovementController>();
    }

    public void OnJumpInputDown()
    {
        if (!controller.collisions.wallSliding)
        {
            if (controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)
                {
                    //Check to make sure the player isn't trying to jump up the slope
                    //We don't want them jumping via wallSliding, but I might change this later
                    //Seems weird to be able to climp vertical walls but not steep slopes
                    if (movementController.directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                    {
                    movementController.velocity.y = gravityController.MaxJumpVelocity * controller.collisions.slopeNormal.y;
                    movementController.velocity.x = gravityController.MaxJumpVelocity * controller.collisions.slopeNormal.x * 10f;
                    }
                }
                else
                {
                    movementController.velocity.y = gravityController.MaxJumpVelocity;
                }
            }
        }
    }

    /// <summary>
    /// Used to detect if the player released the button, resulting in a shorter jump if they had not reached the apex of the jump
    /// </summary>
    public void OnJumpInputUp()
    {
        if (movementController.velocity.y > gravityController.MinJumpVelocity)
        {
            movementController.velocity.y = gravityController.MinJumpVelocity;
        }
    }
}
