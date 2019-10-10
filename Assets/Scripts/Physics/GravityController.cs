﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    //Jumping Parameters
    [Header("Jump Height")]
    [SerializeField] float maxJumpHeight = 4;
    [SerializeField] float minJumpHeight = 2;
    [SerializeField] float timeToJumpApex = 0.4f;
    [SerializeField] float accelerationTimeAirborne = 0.2f;
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


    // Start is called before the first frame update
    void Start()
    {
        CalculateGravity();
        CalculateJumpVelocity();
    }

    void CalculateGravity()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    }

    /// <summary>
    /// Calculates the min and max jump velocity using gravity and kinematics equations
    /// </summary>
    void CalculateJumpVelocity()
    {
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
}