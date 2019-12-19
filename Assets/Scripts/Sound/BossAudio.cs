using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{
    private AudioSource bossAudioSource;
    private AudioClip hurtClip;
    private AudioClip deathClip;
    private AudioClip deathImpactClip;
    private AudioClip attackClip;
    private AudioClip footstepClip;


    private void Awake()
    {
        bossAudioSource = GetComponentInChildren<AudioSource>();
        hurtClip = SoundManager.Instance.BossHurt;
        deathClip = SoundManager.Instance.BossDeath;
        deathImpactClip = SoundManager.Instance.BossDeathImpact;
        attackClip = SoundManager.Instance.BossHammerAttack;
        footstepClip = SoundManager.Instance.BossFootsteps;
    }

    /// <summary>
    /// Plays BossHurt clip and is hooked up as an animation event
    /// </summary>
    public void PlayHurtSFX()
    {
        if (hurtClip != null)
        {
            bossAudioSource.volume = SoundManager.Instance.BossHurtVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(bossAudioSource, hurtClip);
        }
    }

    /// <summary>
    /// Plays BossDeath clip and is hooked up as an animation event
    /// </summary>
    public void PlayDeathSFX()
    {
        if (deathClip != null)
        {
            bossAudioSource.volume = SoundManager.Instance.BossDeathVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(bossAudioSource, deathClip);
        }
    }

    /// <summary>
    /// Plays SplicerDeath clip and is hooked up as an animation event
    /// </summary>
    public void PlayDeathImpactSFX()
    {
        if (deathClip != null)
        {
            bossAudioSource.volume = SoundManager.Instance.BossDeathImpactVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(bossAudioSource, deathImpactClip);
        }
    }

    /// <summary>
    /// Plays BossHammerAttack clip and is hooked up as an animation event
    /// </summary>
    public void PlayAttackSFX()
    {
        if (attackClip != null)
        {
            bossAudioSource.volume = SoundManager.Instance.BossHammerAttackVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(bossAudioSource, attackClip);
        }
    }

    /// <summary>
    /// Plays BossFootsteps clip and is hooked up as an animation event
    /// </summary>
    public void PlayFootstepsSFX()
    {
        if (footstepClip != null)
        {
            bossAudioSource.volume = SoundManager.Instance.BossFootstepskVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(bossAudioSource, footstepClip);
        }
    }
}
