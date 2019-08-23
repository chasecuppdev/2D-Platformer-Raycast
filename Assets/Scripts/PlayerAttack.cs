using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public CameraShake cameraShake;
    public Camera mainCamera;
    public Transform attackPosition;
    public LayerMask hitMask;
    public Vector2 attackSize;
    public int damage;

    bool isAttacking;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraShake = mainCamera.GetComponent<CameraShake>();
    }
    void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(attackPosition.position, attackSize, 0, hitMask);
        if (enemiesToDamage.Length != 0)
        {
            StartCoroutine(cameraShake.Shake(0.05f, 0.15f));
        }
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackPosition.position, attackSize);
    }
}
