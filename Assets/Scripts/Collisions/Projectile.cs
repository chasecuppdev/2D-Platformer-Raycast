using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IHitboxResponder
{
    public int damage;
    public string destroyAnimationClipName;
    Hitbox hitbox;
    Animator animator;
    Rigidbody2D thisProjectile;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);

        animator = GetComponent<Animator>();
        thisProjectile = GetComponent<Rigidbody2D>();
    }

    public void collidedWith(Collider2D collider)
    {
        if (collider.tag != "Obstacle" && collider.tag != "Platform")
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox.ApplyAttack(damage);
        }

        thisProjectile.velocity = Vector2.zero;
        animator.Play(destroyAnimationClipName);
    }

    public void DestroyThisProjectile()
    {
        Destroy(gameObject);
    }
}
