using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public int health;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(damage + " damage taken.");
    }
}
