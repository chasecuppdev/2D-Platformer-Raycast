using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    public Transform target;
    public Rigidbody2D projectile;
    public float projectileSpeed;

    Animator prefabAnimator;
    AnimationClip[] animationClips;
    [HideInInspector]
    public Cooldown cooldown;
    public int faceDirectionSnapshot; //Gets set in PlayerInput.cs when the player activates the attack

    private float teleportOutClipLength;
    private float teleportInClipLength;

    public void OnAttackStart()
    {
        Vector3 vectorToTarget = transform.position - target.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        Rigidbody2D projectileInstance = Instantiate(projectile, transform.position, q);
        //Rigidbody2D projectileInstance = Instantiate(projectile, transform.position, Quaternion.Euler(new Vector2(0,0)));
        float speed = (faceDirectionSnapshot == 1) ? projectileSpeed : -projectileSpeed;

        projectileInstance.velocity = (transform.position - target.transform.position).normalized * speed;
        //projectileInstance.velocity = new Vector2(speed, 0);
    }
}
