using System.Collections;
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
    float accelerationTimeGrounded = 0.01f;

    Controller2D controller;
    GravityController gravityController;

    //Input
    [HideInInspector] public Vector2 directionalInput;

    [HideInInspector] public bool isAttacking = false;

    //Getters
    public float VelocityXSmoothing
    {
        get { return velocityXSmoothing; }
        set { velocityXSmoothing = value; }
    }

    public void Update()
    {
        if (!isAttacking)
        {
            PlayerMove();
        }
    }

    private void Start()
    {
        //EventManager.Instance.AddListener(EVENT_TYPE.MOVE, this);
        controller = GetComponent<Controller2D>();
        gravityController = GetComponent<GravityController>();
    }

    /// <summary>
    /// Calculates x velocity based on direction input and y velocity based on gravity
    /// </summary>
    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : gravityController.AccelerationTimeAirborne);
        velocity.y += gravityController.Gravity * Time.deltaTime;
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
    void PlayerMove()
    {
        CalculateVelocity();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravityController.Gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }
}
