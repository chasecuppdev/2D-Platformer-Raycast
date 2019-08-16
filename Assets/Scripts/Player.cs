using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [SerializeField] float jumpHeight = 4;
    [SerializeField] float timeToJumpApex = 0.4f;
    float jumpVelocity;
    float gravity;
    bool facingRight = true;

    float moveSpeed = 5;
    Vector3 velocity;
    float velocityXSmoothing;

    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0;

    Controller2D controller;
    Animator animator;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponentInChildren<Animator>();
        this.GetComponentInChildren<SpriteRenderer>().enabled = false;
        //this.GetComponent<Renderer>().enabled = false;
        
        CalculateGravity();
        CalculateJumpVelocity();
        Debug.Log("Gravity: " + gravity + " |||||| " + "JumpVelocity:" + jumpVelocity );
    }

    void CalculateGravity()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }

    void CalculateJumpVelocity()
    {
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    void RenderAnimations()
    {
        float directionX = Mathf.Sign(velocity.x); //Get the horizontal direction of movement

        if (velocity.x > 0.1f || velocity.x < -0.1f)
        {
            if (controller.collisions.below)
            {
                if (directionX == 1)
                {
                    if (!facingRight)
                    {
                        animator.transform.Rotate(new Vector3(0, 180, 0));
                        facingRight = true;
                    }
                    animator.SetBool("Run", true);
                }
                else if (directionX == -1)
                {
                    if (facingRight)
                    {
                        animator.transform.Rotate(new Vector3(0, 180, 0));
                        facingRight = false;
                    }
                    animator.SetBool("Run", true);
                }
            }
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }

    private void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        RenderAnimations();
    }
}
