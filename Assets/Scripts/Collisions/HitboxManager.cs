using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    Hitbox[] hitboxArray;

    private void Start()
    {
        hitboxArray = GetComponentsInChildren<Hitbox>();
    }

    public void OpenCollisionDetection(string hitboxName)
    {
        for (int i = 0; i < hitboxArray.Length; i++)
        {
            if (hitboxArray[i].name == hitboxName)
            {
                hitboxArray[i].StartCheckCollisions();
                return;
            }
        }
    }

    public void CloseCollisionDetection(string hitboxName)
    {
        for (int i = 0; i < hitboxArray.Length; i++)
        {
            if (hitboxArray[i].name == hitboxName)
            {
                hitboxArray[i].EndCheckCollisions();
                return;
            }
        }
    }
}
