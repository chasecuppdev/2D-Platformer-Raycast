using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

public class PlayerAnimator : AnimatorController
{
    protected override void Start()
    {
        base.Start();
        MessageKit.addObserver(EventTypes.JUMP_INPUT_DOWN, JumpAnimation);
        MessageKit<string[]>.addObserver(EventTypes.ATTACK_INPUT_DOWN_1P, TriggerAttackAnimation);
    }
}
