﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;

    public float speed;
    int fromWaypointIndex = 0;
    float percentBetweenWaypoints;
    public bool cyclic;
    public float waitTime;
    float nextMoveTime;
    [Range(0, 2)]
    public float easeAmount;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

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

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    private void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        CalulatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if (globalWaypoints.Length != 0)
        {
            if (Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }
            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * (speed / distanceBetweenWaypoints);
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);


            Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            if (percentBetweenWaypoints >= 1)
            {
                percentBetweenWaypoints = 0;
                fromWaypointIndex++;

                if (!cyclic)
                {
                    if (fromWaypointIndex >= globalWaypoints.Length - 1)
                    {
                        fromWaypointIndex = 0;
                        System.Array.Reverse(globalWaypoints);
                    }
                }
                nextMoveTime = Time.time + waitTime;
            }

            return newPos - transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach(PassengerMovement passenger in passengerMovement)
        {
            //Keep a list of passengers to move so we can only call GetComponent<Controller2D> once per passenger
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }

            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                //This originally used the overload with just 2 parameter (no player input), but this caused the camera to not adjust horizontally when on a platform
                //This works now for the player character, but I'm not sure if it will work with ai characters on platforms. Might need to revisit
                passengerDictionary[passenger.transform].Move(passenger.velocity, passengerDictionary[passenger.transform].playerInput, passenger.standingOnPlatform);
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

                if (hit && hit.distance != 0) //We don't want to move the passengers if they are "inside" the platform
                {
                    //We only want to move each object once per frame (without this, an object could update for each raycast)
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        //float pushX = (directionY == 1) ? velocity.x : 0; //The horizontal velocity we will add to the object
                        float pushX = 0;
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

                if (hit && hit.distance != 0) //We don't want to move the passengers if they are "inside" the platform
                {
                    //We only want to move each object once per frame (without this, an object could update for each raycast)
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX; //The horizontal velocity we will add to the object
                        float pushY = -skinWidth; //Add a super small downward velocity when being pushed to flag collisions.below and allow jumping

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

                if (hit && hit.distance != 0) //We don't want to move the passengers if they are "inside" the platform
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

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);

            }
        }
    }
}
