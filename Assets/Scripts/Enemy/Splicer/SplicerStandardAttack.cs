using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

public class SplicerStandardAttack : MonoBehaviour, IHitboxResponder
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
