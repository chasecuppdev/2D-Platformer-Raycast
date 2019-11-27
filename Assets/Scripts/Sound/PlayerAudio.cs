using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioClip whipAttackClip;

    private void Awake()
    {
        whipAttackClip = SoundManager.Instance.PlayerWhipBase;
    }

    //Hooked up via Animation Events
    public void PlayWhipAttackClip()
    {
        SoundManager.Instance.PlayWithRandomizedPitch(whipAttackClip);
    }
}
