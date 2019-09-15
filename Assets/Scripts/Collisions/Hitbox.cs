using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderState
{
    Closed,
    Open
}

public class Hitbox : MonoBehaviour
{
    public string name;

    public LayerMask hitboxMask;

    public bool useSphere = false;

    public Vector2 hitboxSize = Vector3.one;

    public float radius = 0.5f;

    public Color collisionClosedColor = new Color(202, 17, 14, 134);
    public Color collisionOpenColor = new Color(119, 255, 69, 164);

    public ColliderState colliderState = ColliderState.Closed;

    private IHitboxResponder hitboxResponder;

    private List<Collider2D> hitList = new List<Collider2D>();

    private void checkGizmoColors()
    {
        switch (colliderState)
        {
            case ColliderState.Closed:
                Gizmos.color = collisionClosedColor;
                break;

            case ColliderState.Open:
                Gizmos.color = collisionOpenColor;
                break;
        }
    }

    public void addResponder(IHitboxResponder responder)
    {
        hitboxResponder = responder;
    }

    private void FixedUpdate()
    {
        if (colliderState == ColliderState.Open)
        {
            CheckCollisions();
        }
    }

    public void StartCheckCollisions()
    {
        hitList.Clear();
        colliderState = ColliderState.Open;
    }

    public void EndCheckCollisions()
    {
        colliderState = ColliderState.Closed;
    }

    private void CheckCollisions()
    {
        if (colliderState == ColliderState.Closed) { return; }

        if (!useSphere)
        {
            Debug.DrawRay(transform.position, transform.right * hitboxSize.x);
            Debug.DrawRay(transform.position, transform.up * hitboxSize.y);
            Debug.DrawRay(transform.position, transform.right * -hitboxSize.x);
            Debug.DrawRay(transform.position, transform.up * -hitboxSize.y);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, hitboxSize * 2f, transform.rotation.z, hitboxMask);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (!hitList.Contains(colliders[i]))
                    {
                        hitList.Add(colliders[i]);
                        hitboxResponder?.collidedWith(colliders[i]);
                    }               
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            checkGizmoColors();
        }
        else
        {
            Gizmos.color = new Color(1, 0, 0, 0.4f);
        }

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        //Gizmos.matrix = transform.localToWorldMatrix;

        if (!useSphere)
        {
            Gizmos.DrawCube(Vector3.zero, hitboxSize * 2f);
        }
        else
        {
            Gizmos.DrawSphere(Vector3.zero, radius);
        }

    }
}
