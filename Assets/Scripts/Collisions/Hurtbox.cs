using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hurtbox : MonoBehaviour
{
    public LayerMask hurtboxMask;
    BoxCollider2D collider;
    IDamageable parentHealthComponent;

    public Vector2 hurtboxSize = Vector3.one;

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        parentHealthComponent = GetComponentInParent<IDamageable>();
        collider.size = hurtboxSize * 2;
    }

    private void UpdateHurtbox(Vector2 newHurtboxSize)
    {
        hurtboxSize = newHurtboxSize;
        collider.size = hurtboxSize * 2;
    }

    public void ApplyAttack(int damage)
    {
        parentHealthComponent.TakeDamage(damage);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(Vector3.zero, hurtboxSize * 2);

    }
}
