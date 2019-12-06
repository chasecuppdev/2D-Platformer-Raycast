using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSettings : MonoBehaviour
{

    [SerializeField] private float attackDistance = 2f;
    public static float AttackDistance => Instance.attackDistance;

    [SerializeField] private LayerMask detectionMask;
    public static LayerMask DetectionMask => Instance.detectionMask;

    public static bool fightStarted = false;
    public static bool FightStarted
    {
        get => fightStarted;
        set => fightStarted = value;
    }

    public static BossSettings Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
