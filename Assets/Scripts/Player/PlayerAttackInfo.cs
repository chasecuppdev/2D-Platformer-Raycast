using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

[RequireComponent(typeof(Hitbox))]
public class PlayerAttackInfo : MonoBehaviour, IHitboxResponder
{
    public int damage;

    Hitbox hitbox;
    public GameObject whipEffectPrefab; //Hooked up in editor
    private AudioSource playerAudioSource; //Hooked up in editor
    private AudioClip whipHitSound;
    [HideInInspector] public Cooldown cooldown;

    private void Awake()
    {
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);

        playerAudioSource = GameObject.Find("PlayerAudioSource").GetComponent<AudioSource>();
        whipHitSound = SoundManager.Instance.PlayerWhipOnHit;

        cooldown = GetComponent<Cooldown>();
    }

    public void collidedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        
        if (whipHitSound != null)
        {
            playerAudioSource.volume = SoundManager.Instance.PlayerWhipOnHitVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, whipHitSound);
        }

        Instantiate(whipEffectPrefab, collider.transform);
        hurtbox.ApplyAttack(damage);
    }
}
