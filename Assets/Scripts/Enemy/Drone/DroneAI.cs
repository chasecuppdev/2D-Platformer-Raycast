using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAI : EnemyAI
{
    ProjectileAttack projectileAttack;

    protected override void Start()
    {
        base.Start();
        projectileAttack = GetComponentInChildren<ProjectileAttack>();
    }

    protected override void Attack()
    {
        animController.TriggerAttackAnimation(DroneAnimationClips.Attack1Animation, DroneAnimationParameters.Attack1Parameter);
    }

    public void DroneTriggerAttack()
    {
        projectileAttack.OnAttackStart();
    }
}
