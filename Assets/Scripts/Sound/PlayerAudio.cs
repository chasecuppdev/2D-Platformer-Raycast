using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioClip footStep;
    private AudioClip jump;
    private AudioClip land;
    private AudioClip whipAttackClip;


    private void Awake()
    {
        footStep = SoundManager.Instance.PlayerFootsteps;
        jump = SoundManager.Instance.PlayerJump;
        land = SoundManager.Instance.PlayerLand;
        whipAttackClip = SoundManager.Instance.PlayerWhipBase;
    }

    /// <summary>
    /// Plays PlayerWhipBase clip and is hooked up as an animation event
    /// </summary>
    public void PlayWhipAttackSFX()
    {
        if (whipAttackClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(whipAttackClip);
        }
    }

    /// <summary>
    /// Plays PlayerFootsteps clip and is hooked up as an animation event
    /// </summary>
    public void PlayFootstepSFX()
    {
        if (footStep != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(footStep);
        }
    }

    /// <summary>
    /// Plays PlayerJump clip and is hooked up as an animation event
    /// </summary>
    public void PlayJumpSFX()
    {
        if (jump != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(jump);
        }
    }

    /// <summary>
    /// Plays PlayerJump clip and is hooked up as an animation event
    /// </summary>
    public void PlayLandSFX()
    {
        if (land != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(land);
        }
    }
}
