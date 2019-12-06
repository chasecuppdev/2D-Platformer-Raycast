using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : BaseState
{
    private Boss _boss;
    [SerializeField] private float maxStunTime;
    private float currentStunTime;


    public StunnedState(Boss boss) : base(boss.gameObject)
    {
        _boss = boss;
    }

    public override Type Tick()
    {
        var hasControl = CheckStunStatus();

        if (hasControl)
        {
            return typeof(ExplodeState);
        }

        return null;
    }

    private bool CheckStunStatus()
    {
        if (currentStunTime > maxStunTime)
        {
            currentStunTime = 0f;
            return true;
        }
        else
        {
            currentStunTime += Time.deltaTime;
            return false;
        }
    }
}
