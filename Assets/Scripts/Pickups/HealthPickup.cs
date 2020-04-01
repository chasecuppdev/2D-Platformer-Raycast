using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    Rigidbody2D heartPickup;
    HeartPickupAudio heartPickupAudio;

    [SerializeField]
    private int healAmount;

    private void Awake()
    {
        heartPickup = GetComponent<Rigidbody2D>();

        heartPickupAudio = GetComponentInChildren<HeartPickupAudio>();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            Hurtbox hurtbox = collider.GetComponentInChildren<Hurtbox>();
            hurtbox.ApplyHealing(healAmount);

            heartPickupAudio.PlayHeartPickupSFX();
            DestroyThisObject();
        }
    }

    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }
}
