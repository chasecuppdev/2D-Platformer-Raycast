using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Controller2D target;
    public Vector2 focusAreaSize;
    public float verticalOffset;
    public float lookAheadDistanceX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirectionX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped = true;

    //Define the area that the player can move in before the camera moves to follow
    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left;
        float right;
        float bottom;
        float top;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            //Create the camera bounds based on the target collider size
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = Vector2.zero;
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;

            //If the target has moved beyond the horizontal bounds, update the horizontal bounds in the direction of target movement
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left; //The result of this will be negative in the x direction since the target bounds will be less than left
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right; //The result of this will be positive in the x direction since the target bounds will be greater than right
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;

            //If the target has moved beyond the vertical bounds, update the vertical bounds in the direction of target movement
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom; //The result of this will be negative in the y direction since the target bounds will be less than bottom
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top; //The result of this will be positive in the y direction since the target bounds will be greater than top
            }
            bottom += shiftY;
            top += shiftY;
            
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

    private void Start()
    {
        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
    }

    private void LateUpdate()
    {
        focusArea.Update(target.collider.bounds);

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        //This if statement depends on there being some residual decceleration when stopping. If we stop instantly then the incremental camera pan won't work
        if (focusArea.velocity.x != 0)
        {
            lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x); //Get the direction the camera is moving
            //Check if the player is attempting to move in the same direction that the camera needs to move
            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
            }
            else
            {
                if (!lookAheadStopped) //If the player released the input mid camera pan, only adjust it slightly as to not feel "jerky"
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX) / 4f;
                }
            }
        }
        //Smooth the horizontal camera movement
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        //Smooth and set the vertical camera movement component
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

        //Add the smoothed horizontal camera movement
        focusPosition += Vector2.right * currentLookAheadX;

        //Apply the movement to the camera. Vector3.forward * -10 is to keep the camera in front of the scene on the z-axis
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0.4f, 0.5f, 0.25f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }
}
