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
    private float playerColliderExtentX;
    

    private void Start()
    {
        
        movementController = GetComponentInParent<MovementController>();
        parentAnimator = GetComponentInParent<Animator>();
        parentAnimatorController = GetComponentInParent<AnimatorController>();
        player = GetComponentInParent<Controller2D>();
        cooldown = GetComponent<Cooldown>();

        playerColliderExtentX = player.collider.bounds.extents.x;

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
        //There seems to be a fringe case where perhaps a collisions happens on the very last frame of the coroutine elapsed time
        //When this happens, I think this flag gets set but the collided section doesn't get a chance to run. It has no ill effects,
        //other than this flag still be set and thus causing an instant collision with nothing.
        collided = false; 
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
            if (collided) //collided is set directly from the Baton_Projectile.cs script
            {
                collided = false;
                projectile.velocity = Vector2.zero;
                projectileAnimator.Play("Baton_Destroy");
                parentAnimator.SetBool("IsTeleporting", true);
                parentAnimator.Play("Player_Teleport_Out");

                yield return new WaitForSeconds(teleportOutClipLength);

                //We will draw a small ray from the player and not update their horizontal position if the
                //the ray returns a hit, indicating the player is already very close to a collidable

                //We want to draw a ray from the vertical midpoint of the players collider
                raySpacing = (player.collider.bounds.max.y - player.collider.bounds.min.y) / 2;

                //Determine the the side the ray should be cast from based on the player's face direction
                rayOrigin = (faceDirectionSnapshot == 1) ? new Vector2(player.collider.bounds.max.x, player.collider.bounds.min.y + raySpacing) : new Vector2(player.collider.bounds.min.x, player.collider.bounds.min.y + raySpacing);
                hit = Physics2D.Raycast(rayOrigin, Vector2.right * faceDirectionSnapshot, minimumTeleportDistance, collisionMask); //Draw a vertical ray and check for collision

                if (hit)
                {
                    player.transform.position = (faceDirectionSnapshot == 1) ? new Vector3(player.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(player.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
                }
                else 
                {
                    //We want to cast a ray a little further back from the projectile position and determine where exactly the collidable is
                    rayOrigin = (faceDirectionSnapshot == 1) ? new Vector2(projectile.transform.position.x - 1.0f, projectile.transform.position.y) : new Vector2(projectile.transform.position.x + 1.0f, projectile.transform.position.y);
                    hit = Physics2D.Raycast(rayOrigin, Vector2.right * faceDirectionSnapshot, 4f, collisionMask);
                    Debug.DrawRay(rayOrigin, Vector2.right * faceDirectionSnapshot * 4f, Color.red, 3f);

                    if (hit) //This SHOULD happen 100% of the time, but make it an if else just in case
                    {
                        player.transform.position = (faceDirectionSnapshot == 1) ? new Vector3(hit.point.x - playerColliderExtentX - 0.1f, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(hit.point.x + playerColliderExtentX + 0.1f, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
                    }
                    else
                    {
                        player.transform.position = (faceDirectionSnapshot == 1) ? new Vector3(projectile.transform.position.x - teleportOffsetX, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(projectile.transform.position.x + teleportOffsetX, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
                    }
                }

                Destroy(projectile.gameObject);

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
        
        parentAnimator.Play("Player_Teleport_In");
        player.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);

        //If the player is in the air and is very close to the wall, we will update their position to be touching the wall so they begin wallsliding
        //Distance between pivot points of player and projectile
        raySpacing = (player.collider.bounds.max.y - player.collider.bounds.min.y) / 2;

        //We actually want to start the ray from the back of the players collider depending on the face direction here
        //By doing this we take care of two problems with one raycast
        //1) If the player doesn't quite hit the wall but is close enough (minimumTeleportDistance), then we will put them on the wall so they immediately wallslide
        //2) If the player didn't collide but might still have their collider within the collidable, it will readjust to take the player's collider out of the collidable
        rayOrigin = (faceDirectionSnapshot == 1) ? new Vector2(player.collider.bounds.min.x, player.collider.bounds.min.y + raySpacing) : new Vector2(player.collider.bounds.max.x, player.collider.bounds.min.y + raySpacing);
        hit = Physics2D.Raycast(rayOrigin, Vector2.right * faceDirectionSnapshot, (playerColliderExtentX * 2) + minimumTeleportDistance, collisionMask); //Draw a vertical ray and check for collision

        if (hit)
        {
            player.transform.position = (faceDirectionSnapshot == 1) ? new Vector3(hit.point.x - playerColliderExtentX - 0.1f, projectile.transform.position.y - teleportOffsetY, player.transform.position.z) : new Vector3(hit.point.x + playerColliderExtentX + 0.1f, projectile.transform.position.y - teleportOffsetY, player.transform.position.z);
        }

        Destroy(projectile.gameObject);
        yield return new WaitForSeconds(teleportInClipLength);
        parentAnimator.SetBool("IsTeleporting", false);
    }
}
