using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31.MessageKit;

public class HUDController : MonoBehaviour
{
    Image playerHealthBar;

    void Awake()
    {
        playerHealthBar = GameObject.Find("HP_Bar").GetComponent<Image>();
        MessageKit<int, int>.addObserver(EventTypes.PLAYER_TAKE_DAMAGE_2P, UpdatePlayerHealthBar);
        MessageKit<int, int>.addObserver(EventTypes.PLAYER_HEAL_DAMAGE_2P, UpdatePlayerHealthBar);
    }

    public void UpdatePlayerHealthBar(int currentHealth, int maxHealth)
    {
        playerHealthBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }
}
