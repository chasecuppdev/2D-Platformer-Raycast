using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SplicerAudio : MonoBehaviour
{

    private AudioSource splicerAudioSource;
    private AudioClip hurtClip;
    private AudioClip deathClip;
    private AudioClip attackClip;


    private void Awake()
    {
        splicerAudioSource = GetComponentInChildren<AudioSource>();
        hurtClip = SoundManager.Instance.SplicerHurt;
        deathClip = SoundManager.Instance.SplicerDeath;
        attackClip = SoundManager.Instance.SplicerAttack;
    }

    /// <summary>
    /// Plays SplicerHurt clip and is hooked up as an animation event
    /// </summary>
    public void PlayHurtSFX()
    {
        if (hurtClip != null)
        {
            splicerAudioSource.volume = SoundManager.Instance.SplicerHurtVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(splicerAudioSource, hurtClip);
        }
    }

    /// <summary>
    /// Plays SplicerDeath clip and is hooked up as an animation event
    /// </summary>
    public void PlayDeathSFX()
    {
        if (deathClip != null)
        {
            splicerAudioSource.volume = SoundManager.Instance.SplicerDeathVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(splicerAudioSource, deathClip);
        }
    }

    /// <summary>
    /// Plays SplicerAttack clip and is hooked up as an animation event
    /// </summary>
    public void PlayAttackSFX()
    {
        if (attackClip != null)
        {
            splicerAudioSource.volume = SoundManager.Instance.SplicerAttackVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(splicerAudioSource, attackClip);
        }
    }
}
