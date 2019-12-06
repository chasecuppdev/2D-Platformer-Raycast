using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    private Boss _boss;

    public IdleState(Boss boss) : base(boss.gameObject)
    {
        _boss = boss;
    }

    public override Type Tick()
    {
        if (BossSettings.FightStarted)
        {
            return typeof(ChaseState);
        }

        return null;
    }
}
