using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleportAttack : MonoBehaviour
{
    public LayerMask collisionMask;
    public Rigidbody2D batonProjectile;
    public float projectileSpeed;
    public float projectileTime;

    public float teleportOffsetX;
    public float teleportOffsetY;

    public float minimumTeleportDistance;

    Animator prefabAnimator;
    Animator parentAnimator;
    AnimatorController parentAnimatorController;
    Controller2D player;
    MovementController movementController;
    AnimationClip[] animationClips;
    [HideInInspector]
    public Cooldown cooldown;

    public bool collided = false;
    private bool facingRight;

    private float teleportOutClipLength;
    private float teleportInClipLength;
    

    private void Start()
    {
        
        movementController = GetComponentInParent<MovementController>();
        parentAnimator = GetComponentInParent<Animator>();
        parentAnimatorController = GetComponentInParent<AnimatorController>();
        player = GetComponentInParent<Controller2D>();
        cooldown = GetComponent<Cooldown>();

        animationClips = parentAnimator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name == "Player_Teleport_Out")
            {
                teleportOutClipLength = animationClips[i].length;
            }
            if (animationClips[i].name == "Player_Teleport_In")
            {
                teleportInClipLength = animationClips[i].length;
            }
        }
    }

    private void Update()
    {
        int directionX = facingRight ? 1 : -1;
        float raySpacing = (player.collider.bounds.max.y - player.collider.bounds.min.y) / 2;
        Vector2 rayOrigin = facingRight ? new Vector2(player.collider.bounds.max.x, player.collider.bounds.min.y + raySpacing) : new Vector2(player.collider.bounds.min.x, player.collider.bounds.min.y + raySpacing);
        Debug.DrawRay(rayOrigin, (Vector3.right * directionX) * minimumTeleportDistance, Color.red);
    }

    public void OnAttackStart()
    {
        facingRight = parentAnimatorController.facingRight;
        Rigidbody2D batonInstance = Instantiate(batonProjectile, transform.position, Quaternion.Euler(new Vector2(0, 0)));
        float speed = facingRight ? projectileSpeed : -projectileSpeed;

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
                parentAnimator.SetBool("IsTeleporting", true);
                //parentAnimator.SetBool("IsTeleporting", true);
                parentAnimator.Play("Player_Teleport_Out");

                yield return new WaitForSeconds(teleportOutClipLength);

                //Distance between pivot points of player and projectile
                int directionX = facingRight ? 1 : -1;
                float raySpacing = (player.collider.bounds.max.y - player.collider.bounds.min.y) / 2;

                Vector2 rayOrigin = facingRight ? new Vector2(player.collider.bounds.max.x, player.collider.bounds.min.y + raySpacing) : new Vector2(player.collider.bounds.min.x, player.collider.bounds.min.y + raySpacing);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, minimumTeleportDistance, collisionMask); //Draw a vertical ray and check for collision

                if (hit)
                {
                    player.transform.position = facingRight ? new Vector3(player.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(player.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
                }
                else
                {
                    player.transform.position = facingRight ? new Vector3(projectile.transform.position.x - teleportOffsetX, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(projectile.transform.position.x + teleportOffsetX, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
                }

                Destroy(projectile.gameObject);
                parentAnimator.Play("Player_Teleport_In");
                collided = false;
                yield return new WaitForSeconds(teleportOutClipLength);
                parentAnimator.SetBool("IsTeleporting", false);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return 0;
        }

        projectile.velocity = Vector2.zero;
        projectileAnimator.Play("Baton_Destroy");
        parentAnimator.SetBool("IsTeleporting", true);
        //parentAnimator.SetBool("IsTeleporting", true);
        parentAnimator.Play("Player_Teleport_Out");

        yield return new WaitForSeconds(teleportOutClipLength);

        Debug.Log("Player position: " + player.transform.position);
        Debug.Log("Projectile position: " + projectile.transform.position);
        player.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
        Destroy(projectile.gameObject);
        parentAnimator.Play("Player_Teleport_In");
        yield return new WaitForSeconds(teleportOutClipLength);
        parentAnimator.SetBool("IsTeleporting", false);
    }
}
