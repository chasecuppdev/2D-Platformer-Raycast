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
    public int faceDirectionSnapshot; //Gets set in PlayerInput.cs when the player activates the attack

    private float teleportOutClipLength;
    private float teleportInClipLength;

    private Collider2D collidedObject;
    

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

    public void SetCollider(Collider2D collider)
    {
        collidedObject = collider;
    }

    private void Update()
    {
        float raySpacing = (player.collider.bounds.max.y - player.collider.bounds.min.y) / 2;
        Vector2 rayOrigin = (faceDirectionSnapshot == 1) ? new Vector2(player.collider.bounds.max.x, player.collider.bounds.min.y + raySpacing) : new Vector2(player.collider.bounds.min.x, player.collider.bounds.min.y + raySpacing);
        Debug.DrawRay(rayOrigin, (Vector3.right * faceDirectionSnapshot) * minimumTeleportDistance, Color.red);
    }

    public void OnAttackStart()
    {
        Rigidbody2D batonInstance = Instantiate(batonProjectile, transform.position, Quaternion.Euler(new Vector2(0, 0)));
        float speed = (faceDirectionSnapshot == 1) ? projectileSpeed : -projectileSpeed;

        batonInstance.velocity = new Vector2(speed, 0);

        StartCoroutine(TeleportSequence(batonInstance));
    }

    public IEnumerator TeleportSequence(Rigidbody2D projectile)
    {
        float raySpacing;
        Vector2 rayOrigin;
        RaycastHit2D hit;
        float elapsed = 0;
        Animator projectileAnimator = projectile.GetComponent<Animator>();

        while (elapsed < projectileTime)
        {
            if (collided) //collided is set direction from the baton projectile prefab
            {
                collided = false;
                projectile.velocity = Vector2.zero;
                projectileAnimator.Play("Baton_Destroy");
                parentAnimator.SetBool("IsTeleporting", true);
                //parentAnimator.SetBool("IsTeleporting", true);
                parentAnimator.Play("Player_Teleport_Out");

                yield return new WaitForSeconds(teleportOutClipLength);

                //Distance between pivot points of player and projectile
                raySpacing = (player.collider.bounds.max.y - player.collider.bounds.min.y) / 2;

                rayOrigin = (faceDirectionSnapshot == 1) ? new Vector2(player.collider.bounds.max.x, player.collider.bounds.min.y + raySpacing) : new Vector2(player.collider.bounds.min.x, player.collider.bounds.min.y + raySpacing);
                hit = Physics2D.Raycast(rayOrigin, Vector2.right * faceDirectionSnapshot, minimumTeleportDistance, collisionMask); //Draw a vertical ray and check for collision

                if (hit)
                {
                    player.transform.position = (faceDirectionSnapshot == 1) ? new Vector3(player.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(player.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
                }
                else
                {
                    player.transform.position = (faceDirectionSnapshot == 1) ? new Vector3(projectile.transform.position.x - teleportOffsetX, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(projectile.transform.position.x + teleportOffsetX, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
                }


                Destroy(projectile.gameObject);

                //If the player is in the air and is very close to the wall, we will update their position to be touching the wall so they begin wallsliding
                //Distance between pivot points of player and projectile
                raySpacing = (player.collider.bounds.max.y - player.collider.bounds.min.y) / 2;

                rayOrigin = (faceDirectionSnapshot == 1) ? new Vector2(player.collider.bounds.max.x, player.collider.bounds.min.y + raySpacing) : new Vector2(player.collider.bounds.min.x, player.collider.bounds.min.y + raySpacing);
                hit = Physics2D.Raycast(rayOrigin, Vector2.right * faceDirectionSnapshot, minimumTeleportDistance, collisionMask); //Draw a vertical ray and check for collision

                if (hit)
                {
                    player.transform.position = (faceDirectionSnapshot == 1) ? new Vector3(player.transform.position.x + hit.distance, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(player.transform.position.x - hit.distance, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
                }

                parentAnimator.Play("Player_Teleport_In");
                yield return new WaitForSeconds(teleportInClipLength);
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
        
        parentAnimator.Play("Player_Teleport_In");
        player.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
        Destroy(projectile.gameObject);
        yield return new WaitForSeconds(teleportInClipLength);
        parentAnimator.SetBool("IsTeleporting", false);
    }
}
