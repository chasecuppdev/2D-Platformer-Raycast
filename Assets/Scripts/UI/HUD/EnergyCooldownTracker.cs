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

    public void FixedUpdate()
    {
        if (energyCooldown.timer <= 0)
        {
            cooldownImage.fillAmount = 1;
        }
        else
        {
            cooldownImage.fillAmount = 1 - ((float)energyCooldown.timer / (float)energyCooldown.cooldown);
        }
        
    }
}
