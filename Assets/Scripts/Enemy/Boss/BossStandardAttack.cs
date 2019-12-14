using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStandardAttack : MonoBehaviour, IHitboxResponder
{
    public int damage;

    Hitbox hitbox;

    private void Start()
    {
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);
    }

    public void collidedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        hurtbox.ApplyAttack(damage);
    }
}
