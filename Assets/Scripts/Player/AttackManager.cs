using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    PlayerTeleportAttack teleportAttack;

    private void Start()
    {
        teleportAttack = GetComponentInChildren<PlayerTeleportAttack>();
    }

    public void StartTeleportAttack()
    {
        teleportAttack.OnAttackStart();
    }
}
