using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    //Jumping Parameters
    [Header("Jump Height")]
    [SerializeField] float maxJumpHeight;
    [SerializeField] float minJumpHeight;
    [SerializeField] float timeToJumpApex;
    [SerializeField] float accelerationTimeAirborne;
    [SerializeField] bool useGravity = true;
    float baseGravity;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;

    //Getters
    public float Gravity
    {
        get { return gravity; }
    }

    public float MaxJumpVelocity
    {
        get { return maxJumpVelocity; }
    }

    public float MinJumpVelocity
    {
        get { return minJumpVelocity; }
    }

    public float AccelerationTimeAirborne
    {
        get { return accelerationTimeAirborne; }
    }

    void Start()
    {
        CalculateGravity();
        CalculateJumpVelocity();
    }

    void CalculateGravity()
    {
        if (useGravity)
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        }
        else
        {
            gravity = 0;
        }
    }

    /// <summary>
    /// Calculates the min and max jump velocity using gravity and kinematics equations
    /// </summary>
    void CalculateJumpVelocity()
    {
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    public void ToggleGravity()
    {
        useGravity = useGravity ? false : true;
        CalculateGravity();
        CalculateJumpVelocity();
    }
}
