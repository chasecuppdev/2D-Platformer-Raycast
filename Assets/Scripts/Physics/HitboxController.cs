using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HitboxController : MonoBehaviour
{
    // Set these in the editor
    public PolygonCollider2D frame2;
    public PolygonCollider2D frame3;

    // Used for organization
    private PolygonCollider2D[] colliders;

    // Collider on this game object
    private PolygonCollider2D localCollider;

    // We say box, but we're still using polygons.
    public enum hitBoxes
    {
        frame2Box,
        frame3Box,
        clear // special case to remove all boxes
    }

    struct HitboxInfo
    {
        public Vector2 center;
        float left;
        float right;
        float bottom;
        float top;

        int damage;

        public HitboxInfo(Transform _hitboxPosition, Vector2 _size, float _offsetX, float _offsetY, int _damage)
        {
            left = _hitboxPosition.position.x + _offsetX; //Start the hit box at the end of the source bounds
            right = _hitboxPosition.position.x + _size.x + _offsetX; //Extend the hitbox from the end of the source bounds to the desired length
            bottom = _hitboxPosition.position.y - (_size.y / 2) + _offsetY;
            top = _hitboxPosition.position.y + (_size.y / 2) + _offsetY;

            center = new Vector2((left + right) / 2, (bottom + top) / 2);
            damage = _damage;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set up an array so our script can more easily set up the hit boxes
        colliders = new PolygonCollider2D[] { frame2, frame3 };

        // Create a polygon collider
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = true; // Set as a trigger so it doesn't collide with our environment
        localCollider.pathCount = 0; // Clear auto-generated polygons
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Collider hit something!");
    }

    public void setHitBox(hitBoxes val)
    {
        if (val != hitBoxes.clear)
        {
            localCollider.SetPath(0, colliders[(int)val].GetPath(0));
            return;
        }
        localCollider.pathCount = 0;
    }

    private void OnDrawGizmosSelected()
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawCube(collider.transform.position, size);
        }
    }
