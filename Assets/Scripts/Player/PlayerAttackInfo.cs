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
    private AudioClip whipHitSound; //Hooked up in editor

    private void Awake()
    {
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);

        whipHitSound = SoundManager.Instance.PlayerWhipOnHit;
    }

    public void collidedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        SoundManager.Instance.Play(whipHitSound);
        Instantiate(whipEffectPrefab, collider.transform);
        hurtbox.ApplyAttack(damage);
    }
}
