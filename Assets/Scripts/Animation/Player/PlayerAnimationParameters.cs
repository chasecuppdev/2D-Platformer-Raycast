using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationParameters : ScriptableObject
{
    static public string Attack1Parameter = "IsAttacking";
    static public string DeathParameter = "IsDead";
    static public string DirectionalInputParameter = "DirectionalInput";
    static public string FallingParameter = "IsFalling";
    static public string HurtParameter = "IsHurt";
    static public string JumpingParameter = "IsJumping";
    static public string LandingParameter = "IsLanding";
    static public string SpeedParameter = "Speed";
    static public string GroundedTeleportAttackParameter = "IsGroundedTeleportAttacking";
    static public string AirTeleportAttackParameter = "IsAirTeleportAttacking";
    static public string TeleportParameter = "IsTeleporting";
    static public string Grounded = "IsGrounded";
    static public string GroundedAttack1Parameter = "IsGroundedAttack1";
    static public string GroundedAttack2Parameter = "IsGroundedAttack2";
    static public string GroundedAttack3Parameter = "IsGroundedAttack3";
    static public string AirAttack1Parameter = "IsAirAttack1";
    static public string AirAttack2Parameter = "IsAirAttack2";
    static public string AirAttack3Parameter = "IsAirAttack3";

    static public string[] AllAttackParameters = 
    {
        Attack1Parameter,
        GroundedAttack1Parameter,
        GroundedAttack2Parameter,
        GroundedAttack3Parameter,
        AirAttack1Parameter,
        AirAttack2Parameter,
        AirAttack3Parameter,
        GroundedTeleportAttackParameter,
        AirTeleportAttackParameter
    };
}
