using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31.MessageKit;

public class HUDController : MonoBehaviour
{
    Image playerHealthBar;

    void Start()
    {
        playerHealthBar = GameObject.Find("HP_Bar").GetComponent<Image>();
        MessageKit<int, int>.addObserver(EventTypes.PLAYER_TAKE_DAMAGE_1P, UpdatePlayerHealthBar);
    }

    public void UpdatePlayerHealthBar(int currentHealth, int maxHealth)
    {
        playerHealthBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }
}
