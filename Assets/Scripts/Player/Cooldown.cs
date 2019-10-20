using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
    public bool active;                 //Is this timer active?
    public float cooldown;              //How often this cooldown may be used
    public float timer;                 //Time left on timer, can be used at 0

    void FixedUpdate()
    {
        if (active)
            timer -= Time.deltaTime;    //Subtract the time since last frame from the timer.
        if (timer < 0)
        {
            timer = 0;                  //If timer is less than 0, reset it to 0 as we don't want it to be negative
            active = false;
        }
        //UpdateUI();
    }

    void UpdateUI()
    {
        //For purpose of demo
    }

    public void StartCooldown()
    {
        if (timer > 0)                  //If timer greater than 0, don't complete action
            return;

        active = false;                 //Set active to be false so timer does not start moving until after action is complete. Remove this if you want timer to restart right away.

        //Run Action Logic

        timer = cooldown;

        active = true;
    }
}
