using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : RaycastController
{
    public float maxSlopeAngle = 80.0f;

    [HideInInspector]
    public Vector2 playerInput;

    public CollisionInfo collisions;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
    }

    //Overloading move method so the platform controller doesn't need to worry about passing the input vector
    public void Move(Vector2 moveDistance, bool standingOnPlatfrom)
    {
        Move(moveDistance, Vector2.zero, standingOnPlatfrom);
    }

    //We have standingOnPlatform bool her because with platforms, we can be moving up vertically but still want to be able to jump
    public void Move(Vector2 moveDistance, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = moveDistance;
        playerInput = input;


        if (moveDistance.y < 0)
        {
            DescendSlope(ref moveDistance);
        }

        if (moveDistance.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(moveDistance.x);
        }

        //if (moveDistance.x != 0) //No need to check for collisions if we aren't moving horizontally
        //{
        //    HorizontalCollisions(ref moveDistance);
        //}

        HorizontalCollisions(ref moveDistance);

        if (moveDistance.y != 0) //No need to check for collisions if we aren't moving vertically
        {
            VerticalCollisions(ref moveDistance);
        }

        transform.Translate(moveDistance);
        
        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveDistance)
    {
        //float directionX = Mathf.Sign(moveDistance.x); //Get the horizontal direction of movement
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveDistance.x) + skinWidth; //Get the length of the movement and remember to add back in skin width

        if (Mathf.Abs(moveDistance.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;  //If we are moving left, iteratively cast left from the bottom left, and if not, iteratively cast right from the bottom right
            rayOrigin += Vector2.up * (horizontalRaySpacing * i); //Updating the starting position for each horizontal raycast for each loop iteration
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); //Draw a horizontal ray and check for collision

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit) //returns null if nothing was hit
            {
                //If the hit.distance is 0, we are "in front" of a moving platform
                if (hit.distance == 0)
                {
                    collisions.insidePlatform = true; //TODO: Come back to this and try to fix the case where platform goes in front of the player while they are on the ground. I think turning removing the Platform collision mask when casting rays will do most of the job, but it will stick to the platform while the platform is in the skinwidth
                    return; //We are assuming the platform is not taller than the object, so continue and let another raycast determine horizontal movement
                }
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up); //Find the angle between our raycast and the normal of whatever we hit

                if (i == 0 && slopeAngle <= maxSlopeAngle) //We will only want to calculate how to climb with the bottom raycast
                {
                    if (collisions.descendingSlope) //If we were descending and then hit another slope that causes us to start ascending
                    {
                        collisions.descendingSlope = false;
                        moveDistance = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) //Only redo this calculation if we are hitting a new slope
                    {
                        //When we hit a slope, the raycast will be a little farther out from our bounds but ClimbSlope will starting calculating from this position.
                        //ClimbSlope sets the x and y position based on the position of our object when it gets called, so it's not "sitting" on the slope exactly
                        //and if we didn't keep track of the distance to the slope, that would be lost to use after calling ClimbSlope. By removing the distanceToSlopeStart
                        //from our x moveDistance, we allow the ClimbSlope calculations to happen correctly based on where the object is when the raycast detects the slope,
                        //and then add back in the distance such that our object will be sitting on the slope and not hovering over it. 
                        distanceToSlopeStart = hit.distance - skinWidth; //Since the raycast is a little in front of our object, we need to keep track of that distance so we don't float on the slope after climbing
                        moveDistance.x -= distanceToSlopeStart * directionX; //Our moveDistance (or position) is where it will be after this frame, not where it is now that we have hit the slope. We want to calculate the climbing vector with our current position
                    }
                    ClimbSlope(ref moveDistance, slopeAngle, hit.normal); 
                    moveDistance.x += (distanceToSlopeStart) * directionX; //Add back in the horizontal distance we removed earlier so we aren't misaligned horizontally and thus floating on the slope
                }

                //If we aren't on a slope or if we hit a wall (even if we are on a slope)
                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) 
                {
                    //moveDistance.x = (hit.distance - skinWidth) * directionX; //We set the distance we want to move horizontally equal to the distance of the raycast that intersected with an obstacle, keeping in mind the skin width
                    moveDistance.x = Mathf.Min(Mathf.Abs(moveDistance.x), (hit.distance - skinWidth)) * directionX;
                    //Setting the rayLength here is very important. Suppose the collider is moving right into a step such that the collider is twice as tall as the step.
                    //If we did not set the raylength once we hit something, then the last ray (the uppermost ray in this case) would always have priority in setting the moveDistance,
                    //and in this example we would go through the step and collide with the next step. But since we have set the rayLength to be the distance in which we collided with
                    //closest step, none of the raycasts that are taller than the step will be long enough to collide with the second step and we will collide with the first step.
                    //Basically, the closest object a raycast hits in this example will be how much we should move down in the horizontal direction.
                    //rayLength = hit.distance; //Set the rayLength each time a collision is detected so we ensure we collide with the closest obstacle
                    rayLength = Mathf.Min(Mathf.Abs(moveDistance.x) + skinWidth, hit.distance);

                    //If we hit an obstacle horizontally that is not climbable while climbing, we need to adjust our y moveDistance as our x moveDistance is reducing (since we hit something and are no longer moving)
                    if (collisions.climbingSlope) 
                    {
                        moveDistance.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveDistance.x);
                    }
                    collisions.left = (directionX == -1);
                    collisions.right = (directionX == 1);
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveDistance)
    {
        float directionY = Mathf.Sign(moveDistance.y); //Get the vertical direction of movement
        float rayLength = Mathf.Abs(moveDistance.y) + skinWidth; //Get the length of the movement and remember to add back in skin width

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; //If we are moving down, iteratively cast down from the bottom left, and if not, iteratively cast up from the top left
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveDistance.x); //Updating the starting position for each vertical raycast for each loop iteration
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask); //Draw a vertical ray and check for collision

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit) //Returns null if nothing was hit
            {
                //We want to be able to jump up through platforms
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0) //check for hit distance zero so we don't get "pushed" up through the platform if we don't quite make it
                    {
                        continue;
                    }
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1) //Allow players to fall through platforms if they press down
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatorm", 0.5f);
                        continue;
                    }
                }
                moveDistance.y = (hit.distance - skinWidth) * directionY; //We set the distance we want to move vertically equal to the distance of the raycast that intersected with an obstacle, keeping in mind the skin width
                //Setting the rayLength here is very important. Suppose the left half of the collider is over a ledge and the right half is over the ground.
                //If we did not set the raylength once we hit something, then the last ray (the rightmost ray in this case) would always have priority in setting the moveDistance,
                //and in this example we would go through the ledge and collide with the ground. But since we have set the rayLength to be the distance in which we collided with
                //the higher up ledge, none of the raycasts that are directly over the ground will be long enough to collide with the ground and we will collide with the ledge.
                //Basically, the highest object a raycast hits in this example will be how much we should move down in the vertical direction.
                rayLength = hit.distance; //Set the rayLength each time a collision is detected so we ensure we collide with the closest obstacle

                //If we hit an obstacle above us while climbing, we need to adjust our x moveDistance as our y moveDistance is reducing (since we hit something and are no longer moving)
                if (collisions.climbingSlope)
                {
                    moveDistance.x = moveDistance.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveDistance.x);
                }

                collisions.below = (directionY == -1);
                collisions.above = (directionY == 1);
            }
        }

        //If we are currently climbing a slope, we need to be checking for another slope (or say a curved slope)
        //Even though this is a horizontal collision check, we want it at the end of vertical collisions since it always gets called second and having this logic run last results in smoother movement.
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveDistance.x); //Get the horizontal direction of movement
            rayLength = Mathf.Abs(moveDistance.x) + skinWidth; //Only check the distance we could move in one frame based on our current x moveDistance
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveDistance.y; //This will check a little further up (our new height after ascending the new slope) than from the bottom based on our y moveDistance
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); //Draw a horizontal ray and check for collision
        
            if (hit) //Returns null if nothing was hit
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle) //If we are on a slope and encounter a new slope
                {
                    //Since we were increasing our y component for the new slope, we should be decreasing our x
                    //Since we already have our new y position, we adjust our x component to be the hit distance to the new slope
                    moveDistance.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 moveDistance, float slopeAngle, Vector2 slopeNormal)
    {
        float moveMagnitude = Mathf.Abs(moveDistance.x); //Get the magnitude of horizontal movement
        //The vertical component of the movement vector will be the sin of the slope angle times moveMagnitude [y = x * sin(theta)]
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveMagnitude; //SOH (opposite / hypotenuse)
        if (moveDistance.y <= climbVelocityY) //Climbing the slope and not jumping
        {
            moveDistance.y = climbVelocityY;
            //The horizontal component of the movement vector will be the cos of the slope angle times moveMagnitude [x = y * cos(theta)]
            moveDistance.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveMagnitude * Mathf.Sign(moveDistance.x);//CAH (adjacent / hypotenuse)

            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 moveDistance)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveDistance.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveDistance.y) + skinWidth, collisionMask);

        //We only want to slide down a slope if one side of the collider is getting a hit detection from the raycast
        //Since we are on a slope, the leading raycast will be too short to detect the slope once we have moved far enough, because the trailing raycast is still on flat ground or another slope
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveDistance);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveDistance);
        }

        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveDistance.x); //Get the horizontal direction of movement
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft; //If we are moving left, cast down from the bottom right, and if not, cast down from the bottom left
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask); // Draw a horizontal ray downward and check for collision

            if (hit) //Returns null if nothing was hit
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                //Since this method is called when we have negative moveDistance, only make adjustments if we are actually descending a valid slope
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX) //if we are actually travelling in the same direction the slope descends
                    {
                        //Since we are casting an infinite ray downwards, check to see the ray hit distance is smaller (closer) than than what our y component of moveDistance would be if we were moving down the slope (based on our current x moveDistance)
                        //If it is then we are close enough to begin descending
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveDistance.x))
                        {
                            float moveMagnitude = Mathf.Abs(moveDistance.x); //Get the magnitude of horizontal movement
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveMagnitude; //The vertical component of the movement vector will be the sin of the slope angle times moveMagnitude [y = x * sin(theta)]
                            moveDistance.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveMagnitude * Mathf.Sign(moveDistance.x); //The horizontal component of the movement vector will be the cos of the slope angle times moveMagnitude [x = y * cos(theta)]
                            moveDistance.y -= descendVelocityY; //Basically the same as setting to -descendVelocity, but this is safer as the y moveDistance can fluctuate with gravity

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    //This needs serious work. 2 major issues
    //1) Slows down and gets a little stuck at the bottom of the slope
    //2) Jumping while sliding down the slope isn't working correctly. The x-component of movement gets reset to 0 right after jumping so the jump goes straight up
    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveDistance)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                //hit.normal.x will give us the direction of the slope
                //If the slope goes down to the left, the x value of the normal vector will be negative
                moveDistance.x = hit.normal.x * (Mathf.Abs(moveDistance.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad) *.4f; //This is going too fast, so scaling it down some

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void ResetFallingThroughPlatorm()
    {
        collisions.fallingThroughPlatform = false;
    }
}
