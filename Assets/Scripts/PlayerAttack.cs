using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public CameraShake cameraShake;
    public Camera mainCamera;
    public Transform leftAttackPosition;
    public Transform rightAttackPosition;
    [HideInInspector]
    public Transform currentAttackPosition;
    public LayerMask hitMask;
    public Vector2 attackSize;
    public Controller2D player;
    public int damage;

    bool isAttacking;

    private void Start()
    {
        currentAttackPosition = rightAttackPosition;
        mainCamera = Camera.main;
        cameraShake = mainCamera.GetComponent<CameraShake>();
        player = GetComponentInParent<Controller2D>();
    }
    void Attack()
    {
        if (player.collisions.faceDir == 1)
        {
            currentAttackPosition = rightAttackPosition;
        }
        else
        {
            currentAttackPosition = leftAttackPosition;
        }

        Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(currentAttackPosition.position, attackSize, 0, hitMask);
        if (enemiesToDamage.Length != 0)
        {
            StartCoroutine(cameraShake.Shake(0.05f, 0.15f));
        }
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);

        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (Application.isPlaying)
        {
            Gizmos.DrawWireCube(currentAttackPosition.position, attackSize);
        }
        else
        {
            Gizmos.DrawWireCube(rightAttackPosition.position, attackSize);
        }
        
        
    }
}
