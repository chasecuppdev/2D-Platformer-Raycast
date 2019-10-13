using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleportAttack : MonoBehaviour
{
    public Rigidbody2D batonProjectile;
    public float projectileSpeed;
    public float projectileTime;

    public float teleportOffsetX;

    Animator prefabAnimator;
    Animator parentAnimator;
    AnimatorController parentAnimatorController;
    SpriteRenderer batonSprite;
    Controller2D player;
    MovementController movementController;
    AnimationClip[] animationClips;

    public bool collided = false;

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
        float elapsed = 0;
        Animator projectileAnimator = projectile.GetComponent<Animator>();

        while (elapsed < projectileTime)
        {
            if (collided)
            {
                projectile.velocity = Vector2.zero;
                projectileAnimator.Play("Baton_Destroy");
                parentAnimator.Play("Player_Teleport_Out");

                yield return new WaitForSeconds(teleportInClipLength);

                player.transform.position = parentAnimatorController.facingRight ? new Vector3(projectile.transform.position.x - teleportOffsetX, player.transform.position.y, player.transform.position.z) : new Vector3(projectile.transform.position.x + teleportOffsetX, player.transform.position.y, player.transform.position.z);
                Destroy(projectile.gameObject);
                parentAnimator.Play("Player_Teleport_In");
                collided = false;
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return 0;
        }

        projectile.velocity = Vector2.zero;
        projectileAnimator.Play("Baton_Destroy");
        parentAnimator.Play("Player_Teleport_Out");

        yield return new WaitForSeconds(teleportInClipLength);

        player.transform.position = new Vector3(projectile.transform.position.x, player.transform.position.y, player.transform.position.z);
        Destroy(projectile.gameObject);
        parentAnimator.Play("Player_Teleport_In");
    }
}
