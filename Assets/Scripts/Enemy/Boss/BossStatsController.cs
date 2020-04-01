using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStatsController : MonoBehaviour, IDamageable
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
            animator.TriggerTakeDamageAnimation(BossAnimationClips.HurtAnimation, BossAnimationParameters.HurtParameter);
        }
        else
        {
            animator.TriggerDieAnimation(BossAnimationClips.DeathAnimation, BossAnimationParameters.DeathParameter);
        }
    }

    public void Heal(int healAmount)
    {

    }
}
