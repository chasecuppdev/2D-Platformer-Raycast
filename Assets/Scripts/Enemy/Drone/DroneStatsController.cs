using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneStatsController : MonoBehaviour, IDamageable
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
            animator.TriggerTakeDamageAnimation(DroneAnimationClips.TakeDamageAnimation, DroneAnimationParameters.HurtParameter);
        }
        else
        {
            animator.TriggerDieAnimation(DroneAnimationClips.DeathAnimation, DroneAnimationParameters.DeathParameter);
        }
    }

    public void Heal(int healAmount)
    {

    }
}
