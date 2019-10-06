using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class PlayerDashAttack : MonoBehaviour, IHitboxResponder
{
    public int damage;

    Hitbox hitbox;
    MovementController movementController;

    private void Start()
    {
        movementController = GetComponentInParent<MovementController>();
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);
    }

    public void collidedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        hurtbox.ApplyAttack(damage);
    }
}
