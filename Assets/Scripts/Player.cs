using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [SerializeField] float maxJumpHeight = 4;
    [SerializeField] float minJumpHeight = 2;
    [SerializeField] float timeToJumpApex = 0.4f;
    float maxJumpVelocity;
    float minJumpVelocity;
    float gravity;
    bool facingRight = true;

    bool wallSliding;
    int wallDirectionX;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallStickTime;
    float timeToWallUnstick;

    float moveSpeed = 5;
    public float wallSlideSpeedMax;
    Vector3 velocity;
    float velocityXSmoothing;

    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.01f;

    //Renderer collisionRenderer;
    Controller2D controller;
    Animator animator;
    SpriteRenderer sprite;

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

        CalculateVelocity();
        HandleWallSliding();
        CheckFallingAndLanding();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        DirectionController();
        RunController();

    }

    private void FixedUpdate()
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
            if (wallDirectionX == directionalInput.x)
            {
                velocity.x = -wallDirectionX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirectionX * wallJumpClimb.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirectionX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (controller.collisions.below)
        {
            animator.SetBool("IsJumping", true);
            velocity.y = maxJumpVelocity;
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    void HandleWallSliding()
    {
        wallDirectionX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                if (directionalInput.x != wallDirectionX && directionalInput.x != 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
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
