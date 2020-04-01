using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickupAudio : MonoBehaviour
{
    private AudioSource heartPickupAudioSource;

    private AudioClip heartPickupClip;

    private void Awake()
    {
        heartPickupAudioSource = SoundManager.Instance.GetComponentInChildren<AudioSource>();

        heartPickupClip = SoundManager.Instance.HeartPickup;
    }

    /// <summary>
    /// Plays HeartPickup clip and is triggered during collision between the heart pickup and player in HearPickup.cs
    /// </summary>
    public void PlayHeartPickupSFX()
    {
        if (heartPickupClip != null)
        {
            heartPickupAudioSource.volume = SoundManager.Instance.HeartPickupVolume;
            SoundManager.Instance.Play(heartPickupAudioSource, heartPickupClip);
        }
    }
}
