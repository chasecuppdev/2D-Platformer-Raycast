using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationClips : ScriptableObject
{
    static public string IdleAnimation = "Player_Idle";
    static public string RunAnimation = "Player_Run";
    static public string JumpAnimation = "Player_Jump";
    static public string FallingAnimation = "Player_Falling";
    static public string LandingAnimation = "Player_Landing";
    static public string Attack1Animation = "Player_Standard_Attack";
    static public string HurtAnimation = "Player_Hurt";
    static public string DeathAnimation = "Player_Death";
    static public string GroundedDashAttackAnimation = "Player_Grounded_Dash_Attack";
}
