using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportCooldownTracker : MonoBehaviour
{
    [SerializeField]
    Image cooldownImage;
    [SerializeField]
    Cooldown teleportCooldown;

    private AudioSource teleportCooldownAudioSource;
    private AudioClip HUDTeleportAtackReadyClip;

    private bool shouldPlaySound = false;

    private void Awake()
    {
        teleportCooldownAudioSource = GetComponent<AudioSource>();
        HUDTeleportAtackReadyClip = SoundManager.Instance.HUDTeleportAttackReady;
    }

    public void FixedUpdate()
    {
        if (teleportCooldown.active)
        {
            cooldownImage.enabled = true;
            cooldownImage.fillAmount = (float)teleportCooldown.timer / (float)teleportCooldown.cooldown;
            shouldPlaySound = true;
        }
        else
        {
            if (shouldPlaySound)
            {
                teleportCooldownAudioSource.volume = SoundManager.Instance.HUDTeleportAttackReadyVolume;
                SoundManager.Instance.Play(teleportCooldownAudioSource, HUDTeleportAtackReadyClip);
                shouldPlaySound = false;
            }
            cooldownImage.enabled = false;
        }
    }
}
