using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;

    public int PlayerLayer = 8;
    public int ObstacleLayer = 9;
    public int PlatformLayer = 10;

    public const float skinWidth = 0.015f; //We use a "skin" width so the rays aren't fire directly from the edges of the collider
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;

    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public bool insidePlatform;

        public bool climbingSlope;
        public float slopeAngle;
        public float slopeAngleOld;

        public bool descendingSlope;

        public Vector3 velocityOld;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
            insidePlatform = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    public virtual void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2); //We want the rays firing from slightly within the collider so that we still have room to cast rays even when grounded or touching an obstacle

        //set the raycast origins to each corner of the collider, taking into account skin width
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    public void CalculateRaySpacing()
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
