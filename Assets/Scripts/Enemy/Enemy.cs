using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int health;
    AnimatorController animator;
    string[] takeDamageAnimationInfo = new string[2];

    private void Start()
    {
        animator = GetComponent<AnimatorController>();
        takeDamageAnimationInfo[0] = "Enemy2_Hit";
        takeDamageAnimationInfo[1] = "TakeDamage";
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animator.TriggerTakeDamageAnimation(takeDamageAnimationInfo);
        Debug.Log(damage + " damage taken.");
    }
}
