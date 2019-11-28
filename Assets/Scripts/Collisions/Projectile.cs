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
    private AudioSource droneAudioSource; //Hooked up in editor
    private AudioClip projectileOnHitClip;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);

        animator = GetComponent<Animator>();
        thisProjectile = GetComponent<Rigidbody2D>();

        droneAudioSource = GameObject.Find("DroneAudioSource").GetComponent<AudioSource>();
        projectileOnHitClip = SoundManager.Instance.DroneProjectileOnHit;
    }

    public void collidedWith(Collider2D collider)
    {
        if (collider.tag != "Obstacle" && collider.tag != "Platform")
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox.ApplyAttack(damage);
        }

        if (projectileOnHitClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(droneAudioSource, projectileOnHitClip);
        }

        thisProjectile.velocity = Vector2.zero;
        hitbox.EndCheckCollisions(); //We don't want to continue collision detecting for hits once the particle has hit an obstacle and is playing the destroy animation
        animator.Play(destroyAnimationClipName);
    }

    public void DestroyThisProjectile()
    {
        Destroy(gameObject);
    }
}
