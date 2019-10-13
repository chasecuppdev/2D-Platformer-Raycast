using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baton_Projectile : MonoBehaviour, IHitboxResponder
{
    public int damage;
    Hitbox hitbox;
    GameObject spawningGameObject;
    PlayerTeleportAttack spawningAttack;

    // Start is called before the first frame update
    void Start()
    {
        spawningGameObject = GameObject.Find("Player_Teleport_Attack");
        spawningAttack = spawningGameObject.GetComponent<PlayerTeleportAttack>();
        hitbox = GetComponent<Hitbox>();
        hitbox.addResponder(this);
    }

    public void collidedWith(Collider2D collider)
    {
        spawningAttack.collided = true;
        if (collider.tag != "Obstacle")
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox.ApplyAttack(damage);
        }
    }
}
