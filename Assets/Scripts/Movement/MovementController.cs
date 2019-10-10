﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(GravityController))]
public class MovementController : MonoBehaviour
{
    //Velocity
    [HideInInspector] public Vector2 velocity;
    [SerializeField] float moveSpeed = 5;
    float velocityXSmoothing;
    float accelerationTimeGrounded = 0f;
    private bool movementStopped = false;

    public Controller2D controller2D;
    GravityController gravityController;
    AnimatorController animatorController;

    //Input
    [HideInInspector] public Vector2 directionalInput;

    [HideInInspector] public bool isAttacking = false;

    //Getters
    public float VelocityXSmoothing
    {
        get { return velocityXSmoothing; }
        set { velocityXSmoothing = value; }
    }

    public bool MovementStopped
    {
        get { return movementStopped; }
        set { movementStopped = value; }
    }

    public void Update()
    {
        //if (!isAttacking)
        //{
            Move();
        //}
    }

    private void Awake()
    {
        //EventManager.Instance.AddListener(EVENT_TYPE.MOVE, this);
        controller2D = GetComponent<Controller2D>();
        gravityController = GetComponent<GravityController>();

        animatorController = GetComponent<AnimatorController>();
    }

    /// <summary>
    /// Calculates x velocity based on direction input and y velocity based on gravity
    /// </summary>
    void CalculateVelocity()
    {
        if (animatorController?.animationStates.isAttacking == true || animatorController?.animationStates.isTakingDamage == true)
        {
            velocity.x = 0;
            velocity.y += gravityController.Gravity * Time.deltaTime;
        }
        //else if (animatorController?.animationStates.isDashAttacking == true)
        //{
        //    float targetVelocityX = controller2D.collisions.faceDir * moveSpeed;
        //    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX * 3, ref velocityXSmoothing, (controller2D.collisions.below) ? accelerationTimeGrounded : gravityController.AccelerationTimeAirborne);
        //    velocity.y = 0;
        //}
        else
        {
            float targetVelocityX = directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller2D.collisions.below) ? accelerationTimeGrounded : gravityController.AccelerationTimeAirborne);
            velocity.y += gravityController.Gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Takes in the direction input when called from the PlayerInput class
    /// </summary>
    /// <param name="input"></param>
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    /// <summary>
    /// Calls all necessary movement methods and also does some minor collision checking
    /// </summary>
    void Move()
    {
        CalculateVelocity();

        controller2D.Move(velocity * Time.deltaTime, directionalInput);

        if (controller2D.collisions.above || controller2D.collisions.below)
        {
            if (controller2D.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller2D.collisions.slopeNormal.y * -gravityController.Gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }
}