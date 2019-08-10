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

    float moveSpeed = 6;
    Vector3 velocity;
    float velocityXSmoothing;

    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.1f;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();
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
    }
}
