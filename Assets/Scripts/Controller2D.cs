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

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

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

    void HorizontalCollisions(ref Vector3 moveDistance)
    {
        float directionX = Mathf.Sign(moveDistance.x); //Get the horizontal direction of movement
        float rayLength = Mathf.Abs(moveDistance.x) + skinWidth; //Get the length of the movement and remember to add back in skin width

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;  //If we are moving left, iteratively cast left from the bottom left, and if not, iteratively cast right from the bottom right
            rayOrigin += Vector2.up * (horizontalRaySpacing * i); //Updating the starting position for each horizontal raycast for each loop iteration
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); //Draw a horizontal ray and check for collision

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit) //returns null if nothing was hit
            {
                moveDistance.x = (hit.distance - skinWidth) * directionX; //We set the distance we want to move horizontally equal to the distance of the raycast that intersected with an obstacle, keeping in mind the skin width
                //Setting the rayLength here is very important. Suppose the collider is moving right into a step such that the collider is twice as tall as the step.
                //If we did not set the raylength once we hit something, then the last ray (the uppermost ray in this case) would always have priority in setting the moveDistance,
                //and in this example we would go through the step and collide with the next step. But since we have set the rayLength to be the distance in which we collided with
                //closest step, none of the raycasts that are taller than the step will be long enough to collide with the second step and we will collide with the first step.
                //Basically, the closest object a raycast hits in this example will be how much we should move down in the horizontal direction.
                rayLength = hit.distance; //Set the rayLength each time a collision is detected so we ensure we collide with the closest obstacle
            }
        }
    }

    void VerticalCollisions(ref Vector3 moveDistance)
    {
        float directionY = Mathf.Sign(moveDistance.y); //Get the vertical direction of movement
        float rayLength = Mathf.Abs(moveDistance.y) + skinWidth; //Get the length of the movement and remember to add back in skin width

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; //If we are moving down, iteratively cast down from the bottom left, and if not, iteratively cast up from the top left
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveDistance.x); //Updating the starting position for each vertical raycast for each loop iteration
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask); //Draw a vertical ray and check for collision

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) //Returns null if nothing was hit
            {
                moveDistance.y = (hit.distance - skinWidth) * directionY; //We set the distance we want to move vertically equal to the distance of the raycast that intersected with an obstacle, keeping in mind the skin width
                //Setting the rayLength here is very important. Suppose the left half of the collider is over a ledge and the right half is over the ground.
                //If we did not set the raylength once we hit something, then the last ray (the rightmost ray in this case) would always have priority in setting the moveDistance,
                //and in this example we would go through the ledge and collide with the ground. But since we have set the rayLength to be the distance in which we collided with
                //the higher up ledge, none of the raycasts that are directly over the ground will be long enough to collide with the ground and we will collide with the ledge.
                //Basically, the highest object a raycast hits in this example will be how much we should move down in the vertical direction.
                rayLength = hit.distance; //Set the rayLength each time a collision is detected so we ensure we collide with the closest obstacle
            }
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
