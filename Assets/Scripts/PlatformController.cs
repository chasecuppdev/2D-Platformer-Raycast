﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;
    public Vector3 move;
    List<PassengerMovement> passengerMovement;

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = move * Time.deltaTime;

        CalulatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach(PassengerMovement passenger in passengerMovement)
        {
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passenger.transform.GetComponent<Controller2D>().Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    void CalulatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>(); //We need to keep track if multiple objects are on the moving platform
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x); //Get the horizontal direction
        float directionY = Mathf.Sign(velocity.y); //Get the vertical direction

        //Vertically moving platform (up)
        if (velocity.y != 0) 
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth; //Get the length of the movement and remember to add back in skin width

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; //If we are moving down, iteratively cast down from the bottom left, and if not, iteratively cast up from the top left
                rayOrigin += Vector2.right * (verticalRaySpacing * i); //Updating the starting position for each vertical raycast for each loop iteration
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask); //Draw a vertical ray and check for collision

                if (hit)
                {
                    //We only want to move each object once per frame (without this, an object could update for each raycast)
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0; //The horizontal velocity we will add to the object
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY; //The vertical velocity we will add to the object

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }
        //Horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth; //Get the length of the movement and remember to add back in skin width

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;  //If we are moving left, iteratively cast left from the bottom left, and if not, iteratively cast right from the bottom right
                rayOrigin += Vector2.up * (horizontalRaySpacing * i); //Updating the starting position for each horizontal raycast for each loop iteration
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask); //Draw a horizontal ray and check for collision

                if (hit)
                {
                    //We only want to move each object once per frame (without this, an object could update for each raycast)
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX; //The horizontal velocity we will add to the object
                        float pushY = 0; //We won't add any vertical velocity from a platform colliding from the side (assuimg zero friction)

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }
        //Passenger on top of horizontally or downward moving platform
        if (directionY == -1 || (velocity.y == 0 && velocity.x != 0))
        {
            float rayLength =  skinWidth * 2; //Get the length of the movement and remember to add back in skin width

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft; //We know we should be casting up from the platform and the horizontal direction plays no part
                rayOrigin += Vector2.right * (verticalRaySpacing * i); //Updating the starting position for each vertical raycast for each loop iteration
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask); //Draw a vertical ray and check for collision

                if (hit)
                {
                    //We only want to move each object once per frame (without this, an object could update for each raycast)
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        //We don't need to account for skinWidth here since nothing is being "pushed", rather we are just moving along with the platform
                        //We just straight up add the platforms velocity to the objects
                        float pushX = velocity.x; 
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }

    }
}
