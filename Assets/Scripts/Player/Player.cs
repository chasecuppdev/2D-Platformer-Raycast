using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

public class Player : MonoBehaviour, IDamageable
{
    public int maxHealth;
    public int currentHealth;
    public int startingHealth;
    AnimatorController animator;

    public void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<AnimatorController>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        MessageKit<float, float>.post(EventTypes.CAMERA_SHAKE_2P, 0.05f, 0.15f);
        MessageKit<int, int>.post(EventTypes.PLAYER_TAKE_DAMAGE_1P, currentHealth, maxHealth);

        if (currentHealth > 0)
        {
            animator.TriggerTakeDamageAnimation(PlayerAnimationClips.HurtAnimation, PlayerAnimationParameters.HurtParameter);
        }
        else
        {
            animator.TriggerDieAnimation(PlayerAnimationClips.DeathAnimation, PlayerAnimationParameters.DeathParameter);
        }
    }
}
