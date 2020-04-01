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
        if (health > 0)
        {
            animator.TriggerTakeDamageAnimation(SplicerAnimationClips.TakeDamageAnimation, SplicerAnimationParameters.HurtParameter);
        }
        else
        {
            animator.TriggerDieAnimation(SplicerAnimationClips.DeathAnimation, SplicerAnimationParameters.DeathParameter);
        }
    }

    public void Heal(int healAmount)
    {

    }
}
