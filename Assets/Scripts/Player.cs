using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    //Jump Height
    [Header("Jump Height")]
    [SerializeField] float maxJumpHeight = 4;
    [SerializeField] float minJumpHeight = 2;
    [SerializeField] float timeToJumpApex = 0.4f;
    float maxJumpVelocity;
    float minJumpVelocity;
    float gravity; 

    //Wall Sliding and Wall Jumping
    [Header ("Wall Climbing")]
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax;
    public float wallStickTime;
    bool wallSliding;
    int wallDirectionX;
    float timeToWallUnstick;

    bool facingRight = true;
    bool isAttacking;

    //Velocity
    float moveSpeed = 5;
    Vector3 velocity;
    float velocityXSmoothing;
    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.01f;

    //Components
    Controller2D controller;
    Animator animator;
    SpriteRenderer sprite;
    
    //Input
    Vector2 directionalInput;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        //collisionRenderer = GetComponent<Renderer>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        //sprite.enabled = false;
        //collisionRenderer.enabled = false;

        CalculateGravity();
        CalculateJumpVelocity();
        Debug.Log("Gravity: " + gravity + " |||||| " + "JumpVelocity:" + maxJumpVelocity);
    }

    private void Update()
    {
        if (!isAttacking)
        {
            PlayerMove();
        }
    }

    void PlayerMove()
    {
        CalculateVelocity();
        HandleWallSliding();
        CheckFallingAndLanding();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

        DirectionController();
        RunController();
    }

    private void LateUpdate()
    {
        animator.SetBool("IsLanding", false);
        animator.SetBool("IsJumping", false);
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            //Wall Climb
            if (wallDirectionX == directionalInput.x)
            {
                velocity.x = -wallDirectionX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            //Jump off the wall
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirectionX * wallJumpClimb.x;
                velocity.y = wallJumpOff.y;
            }
            //Large horizontal leap off the wall 
            else
            {
                velocity.x = -wallDirectionX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                //Check to make sure the player isn't trying to jump up the slope
                //We don't want them jumping via wallSliding, but I might change this later
                //Seems weird to be able to climp vertical walls but not steep slopes
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                {
                    animator.SetBool("IsJumping", true);
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x * 10f;

                }
            }
            else
            {
                animator.SetBool("IsJumping", true);
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void OnAttackInput()
    {
        if (controller.collisions.below && !controller.collisions.slidingDownMaxSlope)
        {
            //We want to reset these animator bools to avoid a race condition with hitting attack as soon as the player hits the ground
            //Right now this is because attack input is check in Update, while resetting these flags are in FixedUpdate
            //Will probably want to change this later
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsFalling", false);
            isAttacking = true;
            animator.SetBool("IsAttacking", true);
            velocity.x = 0; //We don't want the player moving right now while attacking
        }
    }

    /// <summary>
    /// Called at the end of the attack animation. Is currently attached as an Animation Event to the Player_Standard_Attack animation
    /// </summary>
    public void OnAttackEnd()
    {
        animator.SetBool("IsAttacking", false);
        isAttacking = false;
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    void HandleWallSliding()
    {
        wallDirectionX = (controller.collisions.left) ? -1 : 1; //Get the wall direction
        wallSliding = false; //Reset wallSliding for each check

        //If we have a collision on the right or left and we are falling with no collisions below
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            //Make sure we don't fall faster than max wall slide speed
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            //We want to give the player a small window to move away from the wall to perform a leap
            if (timeToWallUnstick > 0)
            {
                //Check if the player input is opposite the wall direction
                if (directionalInput.x != wallDirectionX && directionalInput.x != 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;
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

    void CalculateGravity()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }

    void CalculateJumpVelocity()
    {
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void RunController()
    {
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));

        if (Mathf.Abs(velocity.x) > 0.1f)
        {
            if (controller.collisions.below)
            {
                animator.SetBool("IsRunning", true);
            }
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    void CheckFallingAndLanding()
    {
        if (velocity.y < 0 && !controller.collisions.below)
        {
            animator.SetBool("IsFalling", true);
        }

        if (animator.GetBool("IsFalling") && controller.collisions.below)
        {
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsLanding", true);
        }
    }

    void DirectionController()
    {
        if (Mathf.Abs(velocity.x) != 0)
        {
            float directionX = Mathf.Sign(velocity.x); //Get the horizontal direction of movement

            if (directionX == 1)
            {
                if (!facingRight)
                {
                    //sprite.transform.Rotate(new Vector3(0, 180, 0));
                    sprite.flipX = false;
                    facingRight = true;
                }
            }
            else if (directionX == -1)
            {
                if (facingRight)
                {
                    //sprite.transform.Rotate(new Vector3(0, 180, 0));
                    sprite.flipX = true;
                    facingRight = false;
                }
            }
        }
    }
}
