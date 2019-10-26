using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SplicerAI : EnemyAI
{
    protected override void Attack()
    {
        animController.TriggerAttackAnimation(SplicerAnimationClips.Attack1Animation, SplicerAnimationParameters.Attack1Parameter);
    }
}
