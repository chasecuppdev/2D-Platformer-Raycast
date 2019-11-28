using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource playerAudioSource;
    private AudioClip footStepClip;
    private AudioClip jumpClip;
    private AudioClip landClip;
    private AudioClip hurtClip;
    private AudioClip deathClip;
    private AudioClip wallSlideClip;
    private AudioClip whipAttackClip;
    private AudioClip throwBatonClip;
    private AudioClip teleportOutClip;
    private AudioClip teleportInClip;


    private void Awake()
    {
        playerAudioSource = GetComponentInChildren<AudioSource>();
        footStepClip = SoundManager.Instance.PlayerFootsteps;
        jumpClip = SoundManager.Instance.PlayerJump;
        landClip = SoundManager.Instance.PlayerLand;
        hurtClip = SoundManager.Instance.PlayerHurt;
        deathClip = SoundManager.Instance.PlayerDeath;
        wallSlideClip = SoundManager.Instance.PlayerWallSlide;
        whipAttackClip = SoundManager.Instance.PlayerWhipBase;
        throwBatonClip = SoundManager.Instance.PlayerThrowBaton;
        teleportOutClip = SoundManager.Instance.PlayerTeleportOut;
        teleportInClip = SoundManager.Instance.PlayerTeleportIn;
    }

    /// <summary>
    /// Plays PlayerWhipBase clip and is hooked up as an animation event
    /// </summary>
    public void PlayWhipAttackSFX()
    {
        if (whipAttackClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, whipAttackClip);
        }
    }

    /// <summary>
    /// Plays PlayerFootsteps clip and is hooked up as an animation event
    /// </summary>
    public void PlayFootstepSFX()
    {
        if (footStepClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, footStepClip);
        }
    }

    /// <summary>
    /// Plays PlayerJump clip and is hooked up as an animation event
    /// </summary>
    public void PlayJumpSFX()
    {
        if (jumpClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, jumpClip);
        }
    }

    /// <summary>
    /// Plays PlayerLand clip and is hooked up as an animation event
    /// </summary>
    public void PlayLandSFX()
    {
        if (landClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, landClip);
        }
    }

    /// <summary>
    /// Plays PlayerHurt clip and is hooked up as an animation event
    /// </summary>
    public void PlayHurtSFX()
    {
        if (hurtClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, hurtClip);
        }
    }

    /// <summary>
    /// Plays PlayerDeath clip and is hooked up as an animation event
    /// </summary>
    public void PlayDeathSFX()
    {
        if (deathClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, deathClip);
        }
    }

    /// <summary>
    /// Plays PlayerWallSlide clip and is hooked up as an animation event
    /// </summary>
    public void PlayWallSlideSFX()
    {
        if (wallSlideClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, wallSlideClip);
        }
    }

    /// <summary>
    /// Plays PlayerThrowBaton clip and is hooked up as an animation event
    /// </summary>
    public void PlayThrowBatonSFX()
    {
        if (throwBatonClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, throwBatonClip);
        }
    }

    /// <summary>
    /// Plays PlayerTeleportOut clip and is hooked up as an animation event
    /// </summary>
    public void PlayTeleportOutSFX()
    {
        if (teleportOutClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, teleportOutClip);
        }
    }

    /// <summary>
    /// Plays PlayerTeleportIn clip and is hooked up as an animation event
    /// </summary>
    public void PlayTeleportInSFX()
    {
        if (teleportInClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, teleportInClip);
        }
    }
}
