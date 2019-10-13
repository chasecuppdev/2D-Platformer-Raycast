using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class PlayerTeleportAttack : MonoBehaviour, IHitboxResponder
{
    public int damage;
    public Rigidbody2D batonProjectile;
    public float projectileSpeed;
    public float projectileTime;

    Animator prefabAnimator;
    Animator parentAnimator;
    AnimatorController parentAnimatorController;
    SpriteRenderer batonSprite;
    Hitbox hitbox;
    Controller2D player;
    MovementController movementController;
    AnimationClip[] animationClips;

    private float teleportInClipLength;
    

    private void Start()
    {
        
        movementController = GetComponentInParent<MovementController>();
        batonSprite = GetComponentInChildren<SpriteRenderer>();
        parentAnimator = GetComponentInParent<Animator>();
        parentAnimatorController = GetComponentInParent<AnimatorController>();
        player = GetComponentInParent<Controller2D>();

        animationClips = parentAnimator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == "Player_Teleport_Out")
            {
                teleportInClipLength = animationClips[i].length;
            }
        }

        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);
    }

    public void collidedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        hurtbox.ApplyAttack(damage);
    }

    public void OnAttackStart()
    {
        Rigidbody2D batonInstance = Instantiate(batonProjectile, transform.position, Quaternion.Euler(new Vector2(0, 0)));
        float speed = parentAnimatorController.facingRight ? projectileSpeed : -projectileSpeed;

        batonInstance.velocity = new Vector2(speed, 0);

        StartCoroutine(TeleportSequence(batonInstance));
    }

    public IEnumerator TeleportSequence(Rigidbody2D projectile)
    {
        Animator projectileAnimator = projectile.GetComponent<Animator>();
        yield return new WaitForSeconds(projectileTime);

        projectile.velocity = Vector2.zero;
        projectileAnimator.Play("Baton_Destroy");
        parentAnimator.Play("Player_Teleport_Out");

        yield return new WaitForSeconds(teleportInClipLength);

        player.transform.position = new Vector3(projectile.transform.position.x, player.transform.position.y, player.transform.position.z);
        Destroy(projectile.gameObject);
        parentAnimator.Play("Player_Teleport_In");
    }
}
