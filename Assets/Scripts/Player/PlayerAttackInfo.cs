using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

[RequireComponent(typeof(Hitbox))]
public class PlayerAttackInfo : MonoBehaviour, IHitboxResponder
{
    public int damage;

    Hitbox hitbox;
    public GameObject WhipEffectPrefab; //Hooked up in editor
    public AudioClip WhipHitSound; //Hooked up in editor

    private void Start()
    {
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);
    }

    public void collidedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        SoundManager.Instance.Play(WhipHitSound);
        Instantiate(WhipEffectPrefab, collider.transform);
        hurtbox.ApplyAttack(damage);
    }
}
