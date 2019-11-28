using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAudio : MonoBehaviour
{
    private AudioSource droneAudioSource;
    private AudioClip hurtClip;
    private AudioClip deathClip;
    private AudioClip fireProjectileClip;


    private void Awake()
    {
        droneAudioSource = GetComponentInChildren<AudioSource>();
        hurtClip = SoundManager.Instance.DroneHurt;
        deathClip = SoundManager.Instance.DroneDeath;
        fireProjectileClip = SoundManager.Instance.DroneFireProjectile;
    }

    /// <summary>
    /// Plays DroneHurt clip and is hooked up as an animation event
    /// </summary>
    public void PlayHurtSFX()
    {
        if (hurtClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(droneAudioSource, hurtClip);
        }
    }

    /// <summary>
    /// Plays DroneDeath clip and is hooked up as an animation event
    /// </summary>
    public void PlayDeathSFX()
    {
        if (deathClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(droneAudioSource, deathClip);
        }
    }

    /// <summary>
    /// Plays DroneFireProjectile clip and is hooked up as an animation event
    /// </summary>
    public void PlayFireProjectileSFX()
    {
        if (fireProjectileClip != null)
        {
            SoundManager.Instance.PlayWithRandomizedPitch(droneAudioSource, fireProjectileClip);
        }
    }
}
