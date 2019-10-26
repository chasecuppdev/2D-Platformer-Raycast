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
        Rigidbody2D projectileInstance = Instantiate(projectile, transform.position, Quaternion.Euler(new Vector3(0, 0, Vector3.Angle(transform.position, target.transform.position))));
        float speed = (faceDirectionSnapshot == 1) ? projectileSpeed : -projectileSpeed;

        projectileInstance.velocity = (target.transform.position - transform.position).normalized * speed;
    }
}
