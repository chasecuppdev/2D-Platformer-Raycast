using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

public class PlayerAttack : MonoBehaviour
{
    public Transform leftAttackPosition;
    public Transform rightAttackPosition;
    [HideInInspector]
    public Transform currentAttackPosition;
    public LayerMask hitMask;
    public Vector2 attackSize;
    public Controller2D controller;
    public int damage;

    bool isAttacking;

    private void Start()
    {
        currentAttackPosition = rightAttackPosition;
        controller = GetComponent<Controller2D>();
    }

    void Attack()
    {
        if (controller.collisions.faceDir == 1)
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
            MessageKit<float, float>.post(EventTypes.CAMERA_SHAKE_2P, 0.05f, 0.15f);
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
            //Gizmos.DrawWireCube(currentAttackPosition.position, attackSize);
        }
        else
        {
            Gizmos.DrawWireCube(rightAttackPosition.position, attackSize);
        }
        
        
    }
}
