using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baton_Projectile : MonoBehaviour, IHitboxResponder
{
    public int damage;
    Hitbox hitbox;
    GameObject spawningGameObject;
    PlayerTeleportAttack spawningAttack;
    private AudioSource playerAudioSource;
    private AudioClip batonOnHitClip;

    // Start is called before the first frame update
    void Start()
    {
        playerAudioSource = GameObject.Find("PlayerAudioSource").GetComponent<AudioSource>();
        batonOnHitClip = SoundManager.Instance.PlayerBatonOnHit;
        spawningGameObject = GameObject.Find("Player_Teleport_Attack");
        spawningAttack = spawningGameObject.GetComponent<PlayerTeleportAttack>();
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);
    }

    public void collidedWith(Collider2D collider)
    {
        //if (gameObject.GetComponent<Rigidbody>().velocity.x > 0) //if we are throwing to the right
        //{
        //    //The x-extents of our player's collider is 0.62, so we subtract one more than that to ensure the collider will not overlap with the obstacle
        //    transform.position = new Vector3(collider.bounds.min.x - 0.63f, transform.position.y, transform.position.z); 
        //}
        //else
        //{
        //    //The x-extents of our player's collider is 0.62, so we subtract one more than that to ensure the collider will not overlap with the obstacle
        //    transform.position = new Vector3(collider.bounds.max.x + 0.63f, transform.position.y, transform.position.z);
        //}
        spawningAttack.SetCollider(collider);
        spawningAttack.collided = true;

        if (batonOnHitClip != null)
        {
            playerAudioSource.volume = SoundManager.Instance.PlayerBatonOnHitVolume;
            SoundManager.Instance.PlayWithRandomizedPitch(playerAudioSource, batonOnHitClip);
        }

        if (collider.tag != "Obstacle")
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox.ApplyAttack(damage);
        }
    }
}
