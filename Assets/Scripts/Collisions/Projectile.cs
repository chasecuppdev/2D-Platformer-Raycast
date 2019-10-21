using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IHitboxResponder
{
    public int damage;
    Hitbox hitbox;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);
    }

    public void collidedWith(Collider2D collider)
    {
        if (collider.tag != "Obstacle")
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox.ApplyAttack(damage);
        }
    }
}
