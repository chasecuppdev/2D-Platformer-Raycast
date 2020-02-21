using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(JumpController))]
public class WallSliding : MonoBehaviour
{
    [Header("Wall Climbing")]
    [SerializeField] Vector2 wallJumpClimb = new Vector2(5f, 25f);
    [SerializeField] Vector2 wallJumpOff = new Vector2(5f, 5f);
    [SerializeField] Vector2 wallLeap = new Vector2(20f, 6f);
    [SerializeField] float wallSlideSpeedMax = 3f;
    [SerializeField] float wallStickTime = 0.25f;
    int wallDirectionX;
    float timeToWallUnstick;

    Controller2D controller;
    GravityController gravityController;
    MovementController movementController;
    JumpController jumpController;
    AnimatorController animController;
    PlayerAudio playerAudio;

    private void Start()
    {
        MessageKit.addObserver(EventTypes.JUMP_INPUT_DOWN, OnJumpInputDown);
        controller = GetComponent<Controller2D>();
        gravityController = GetComponent<GravityController>();
        movementController = GetComponent<MovementController>();
        jumpController = GetComponent<JumpController>();
        animController = GetComponent<AnimatorController>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    private void FixedUpdate()
    {
        HandleWallSliding();
    }

    /// <summary>
    /// Function to handle all wallsliding and implements a wall stick time in order to be more lenient on leap input
    /// </summary>
    void HandleWallSliding()
    {
        //We only want the WallSliding Component to ever be the one to update wallSliding to true, since it is optional
        //If the player get hurt or dies, we will stop the wallsliding for the duration of those animations
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && movementController.velocity.y < 0
            && !(animController.animationStates.isHurt || animController.animationStates.isDead))
        {
            if (!controller.collisions.wallSliding)
            {
                controller.collisions.wallSliding = true;
                playerAudio.PlayWallSlideSFX();
            }
        }
        else
        {
            if (controller.collisions.wallSliding)
            {
                controller.collisions.wallSliding = false;
                playerAudio.StopWallSlideSFX();
            }
        }

        //If we have a collision on the right or left and we are falling with no collisions below
        if (controller.collisions.wallSliding)
        {
            wallDirectionX = (controller.collisions.left) ? -1 : 1; //Get the wall direction
            //Make sure we don't fall faster than max wall slide speed
            if (movementController.velocity.y < -wallSlideSpeedMax)
            {
                movementController.velocity.y = -wallSlideSpeedMax;
            }

            //We want to give the player a small window to move away from the wall to perform a leap
            if (timeToWallUnstick > 0)
            {
                //Check if the player input is opposite the wall direction
                if (movementController.directionalInput.x != wallDirectionX && movementController.directionalInput.x != 0)
                {
                    movementController.VelocityXSmoothing = 0;
                    movementController.velocity.x = 0;
                    //This will keep track of the window to leap as long as they keep the correct direction held down
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime; //Reset unstick time if they stop input opposite the wall
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    public void OnJumpInputDown()
    {
        if (controller.collisions.wallSliding)
        {
            //Wall Climb
            if (wallDirectionX == movementController.directionalInput.x)
            {
                movementController.velocity.x = -wallDirectionX * wallJumpClimb.x;
                movementController.velocity.y = wallJumpClimb.y;
            }
            //Jump off the wall
            else if (movementController.directionalInput.x == 0)
            {
                movementController.velocity.x = -wallDirectionX * wallJumpClimb.x;
                movementController.velocity.y = wallJumpOff.y;
            }
            //Large horizontal leap off the wall 
            else
            {
                movementController.velocity.x = -wallDirectionX * wallLeap.x;
                movementController.velocity.y = wallLeap.y;
            }
        }
    }
}
