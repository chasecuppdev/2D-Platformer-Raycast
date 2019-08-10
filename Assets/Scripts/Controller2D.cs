using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    public LayerMask collisionMask;

    const float skinWidth = 0.015f; //We use a "skin" width so the rays aren't fire directly from the edges of the collider
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float maxClimbAngle = 80.0f;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public bool climbingSlope;
        public float slopeAngle;
        public float slopeAngleOld;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
            climbingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector3 moveDistance)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        
        if (moveDistance.x != 0) //No need to check for collisions if we aren't moving horizontally
        {
            HorizontalCollisions(ref moveDistance);
        }
        
        if (moveDistance.y != 0) //No need to check for collisions if we aren't moving vertically
        {
            VerticalCollisions(ref moveDistance);
        }

        transform.Translate(moveDistance);
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x); //Get the horizontal direction of movement
        float rayLength = Mathf.Abs(velocity.x) + skinWidth; //Get the length of the movement and remember to add back in skin width

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;  //If we are moving left, iteratively cast left from the bottom left, and if not, iteratively cast right from the bottom right
            rayOrigin += Vector2.up * (horizontalRaySpacing * i); //Updating the starting position for each horizontal raycast for each loop iteration
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); //Draw a horizontal ray and check for collision

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit) //returns null if nothing was hit
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up); //Find the angle between our raycast and the normal of whatever we hit

                if (i == 0 && slopeAngle <= maxClimbAngle) //We will only want to calculate how to climb with the bottom raycast
                {
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) //Only redo this calculation if we are hitting a new slope
                    {
                        //When we hit a slope, the raycast will be a little farther out from our bounds but ClimbSlope will starting calculating from this position.
                        //ClimbSlope sets the x and y position based on the position of our object when it gets called, so it's not "sitting" on the slope exactly
                        //and if we didn't keep track of the distance to the slope, that would be lost to use after calling ClimbSlope. By removing the distanceToSlopeStart
                        //from our x moveDistance, we allow the ClimbSlope calculations to happen correctly based on where the object is when the raycast detects the slope,
                        //and then add back in the distance such that our object will be sitting on the slope and not hovering over it. 
                        distanceToSlopeStart = hit.distance - skinWidth; //Since the raycast is a little in front of our object, we need to keep track of that distance so we don't float on the slope after climbing
                        velocity.x -= distanceToSlopeStart * directionX; //Our velocity (or position) is where it will be after this frame, not where it is now that we have hit the slope. We want to calculate the climbing vector with our current position
                    }
                    ClimbSlope(ref velocity, slopeAngle); 
                    velocity.x += (distanceToSlopeStart) * directionX; //Add back in the horizontal distance we removed earlier so we aren't misaligned horizontally and thus floating on the slope
                }

                //If we aren't on a slope or if we hit a wall (even if we are on a slope)
                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) 
                {
                    velocity.x = (hit.distance - skinWidth) * directionX; //We set the distance we want to move horizontally equal to the distance of the raycast that intersected with an obstacle, keeping in mind the skin width
                    //Setting the rayLength here is very important. Suppose the collider is moving right into a step such that the collider is twice as tall as the step.
                    //If we did not set the raylength once we hit something, then the last ray (the uppermost ray in this case) would always have priority in setting the moveDistance,
                    //and in this example we would go through the step and collide with the next step. But since we have set the rayLength to be the distance in which we collided with
                    //closest step, none of the raycasts that are taller than the step will be long enough to collide with the second step and we will collide with the first step.
                    //Basically, the closest object a raycast hits in this example will be how much we should move down in the horizontal direction.
                    rayLength = hit.distance; //Set the rayLength each time a collision is detected so we ensure we collide with the closest obstacle

                    //If we hit an obstacle horizontally that is not climbable while climbing, we need to adjust our y velocity as our x velocity is reducing (since we hit something and are no longer moving)
                    if (collisions.climbingSlope) 
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }
                    collisions.left = (directionX == -1);
                    collisions.right = (directionX == 1);
                }
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y); //Get the vertical direction of movement
        float rayLength = Mathf.Abs(velocity.y) + skinWidth; //Get the length of the movement and remember to add back in skin width

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; //If we are moving down, iteratively cast down from the bottom left, and if not, iteratively cast up from the top left
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x); //Updating the starting position for each vertical raycast for each loop iteration
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask); //Draw a vertical ray and check for collision

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) //Returns null if nothing was hit
            {
                velocity.y = (hit.distance - skinWidth) * directionY; //We set the distance we want to move vertically equal to the distance of the raycast that intersected with an obstacle, keeping in mind the skin width
                //Setting the rayLength here is very important. Suppose the left half of the collider is over a ledge and the right half is over the ground.
                //If we did not set the raylength once we hit something, then the last ray (the rightmost ray in this case) would always have priority in setting the moveDistance,
                //and in this example we would go through the ledge and collide with the ground. But since we have set the rayLength to be the distance in which we collided with
                //the higher up ledge, none of the raycasts that are directly over the ground will be long enough to collide with the ground and we will collide with the ledge.
                //Basically, the highest object a raycast hits in this example will be how much we should move down in the vertical direction.
                rayLength = hit.distance; //Set the rayLength each time a collision is detected so we ensure we collide with the closest obstacle

                //If we hit an obstacle above us while climbing, we need to adjust our x velocity as our y velocity is reducing (since we hit something and are no longer moving)
                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = (directionY == -1);
                collisions.above = (directionY == 1);
            }
        }
    }
    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY) //Climbing the slope and not jumping
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }

    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds; 
        bounds.Expand(skinWidth * -2); //We want the rays firing from slightly within the collider so that we still have room to cast rays even when grounded or touching an obstacle

        //set the raycast origins to each corner of the collider, taking into account skin width
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2); //We want the rays firing from slightly within the collider so that we still have room to cast rays even when grounded or touching an obstacle

        //Ensure that we always have at least 2 raycasts for both horizontal and vertical
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //This will keep the spacing between the raycasts completely even no matter how many raycasts we have
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
}
