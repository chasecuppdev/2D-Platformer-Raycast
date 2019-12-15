using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource playerAudioSource;

    private AudioClip footstepClip;
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
        footstepClip = SoundManager.Instance.PlayerFootsteps;
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
            playerAudioSource.volume = SoundManager.Instance.PlayerWhipBaseVolume; ;
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, whipAttackClip);
        }
    }

    /// <summary>
    /// Plays PlayerFootsteps clip and is hooked up as an animation event
    /// </summary>
    public void PlayFootstepSFX()
    {
        if (footstepClip != null)
        {
            playerAudioSource.volume = SoundManager.Instance.PlayerFootstepsVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, footstepClip);
        }
    }

    /// <summary>
    /// Plays PlayerJump clip and is hooked up as an animation event
    /// </summary>
    public void PlayJumpSFX()
    {
        if (jumpClip != null)
        {
            playerAudioSource.volume = SoundManager.Instance.PlayerJumpVolume;
            SoundManager.Instance.Play(playerAudioSource, jumpClip);
        }
    }

    /// <summary>
    /// Plays PlayerLand clip and is hooked up as an animation event
    /// </summary>
    public void PlayLandSFX()
    {
        if (landClip != null)
        {
            playerAudioSource.volume = SoundManager.Instance.PlayerLandVolume;
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
            playerAudioSource.volume = SoundManager.Instance.PlayerHurtVolume;
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
            playerAudioSource.volume = SoundManager.Instance.PlayerDeathVolume;
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
            playerAudioSource.volume = SoundManager.Instance.PlayerWallSlideVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, wallSlideClip);
        }
    }

    public void StopWallSlideSFX()
    {
        SoundManager.Instance.Stop(playerAudioSource, wallSlideClip);
    }

    /// <summary>
    /// Plays PlayerThrowBaton clip and is hooked up as an animation event
    /// </summary>
    public void PlayThrowBatonSFX()
    {
        if (throwBatonClip != null)
        {
            playerAudioSource.volume = SoundManager.Instance.PlayerThrowBatonVolume;
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
            playerAudioSource.volume = SoundManager.Instance.PlayerTeleportOutVolume;
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
            playerAudioSource.volume = SoundManager.Instance.PlayerTeleportInVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, teleportInClip);
        }
    }
}
