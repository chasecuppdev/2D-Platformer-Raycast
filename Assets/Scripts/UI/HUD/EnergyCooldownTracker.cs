using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyCooldownTracker : MonoBehaviour
{
    [SerializeField]
    Image cooldownImage;
    [SerializeField]
    Cooldown energyCooldown;

    AudioSource energyBarAudioSource;
    AudioClip HUDAttackReadyClip;

    private bool shouldPlaySound = false;

    private void Awake()
    {
        energyBarAudioSource = GetComponent<AudioSource>();
        HUDAttackReadyClip = SoundManager.Instance.HUDAttackReady;
    }

    public void FixedUpdate()
    {
        if (energyCooldown.timer <= 0)
        {
            cooldownImage.fillAmount = 1;
            if (shouldPlaySound)
            {
                energyBarAudioSource.volume = SoundManager.Instance.HUDAttackReadyVolume;
                SoundManager.Instance.Play(energyBarAudioSource, HUDAttackReadyClip);
                shouldPlaySound = false;
            }
        }
        else
        {
            shouldPlaySound = true;
            cooldownImage.fillAmount = 1 - ((float)energyCooldown.timer / (float)energyCooldown.cooldown);
        }
        
    }
}
