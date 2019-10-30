using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

public class DeathByFalling : MonoBehaviour
{
    GameObject player;
    public float triggerDeathPositionY;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    private void FixedUpdate()
    {
        if (player.transform.position.y < triggerDeathPositionY)
        {
            MessageKit.post(EventTypes.PLAYER_DIED);
        }
    }
}
