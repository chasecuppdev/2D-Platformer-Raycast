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

    public void FixedUpdate()
    {
        if (teleportCooldown.active)
        {
            cooldownImage.enabled = true;
            cooldownImage.fillAmount = (float)teleportCooldown.timer / (float)teleportCooldown.cooldown;
        }
        else
        {
            cooldownImage.enabled = false;
        }
    }
}
