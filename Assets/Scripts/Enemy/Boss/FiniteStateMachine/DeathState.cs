using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : BaseState
{
    private Boss _boss;

    public DeathState(Boss boss) : base(boss.gameObject)
    {
        _boss = boss;
    }

    public override Type Tick()
    {
        //The death state doesn't really need to do anything, as it should be the final state. It exists to ensure no other states are transitioned to
        return null;
    }
}
