using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int health;
    AnimatorController animator;

    private void Start()
    {
        animator = GetComponent<AnimatorController>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animator.TakeDamageAnimation();
        Debug.Log(damage + " damage taken.");
    }
}
