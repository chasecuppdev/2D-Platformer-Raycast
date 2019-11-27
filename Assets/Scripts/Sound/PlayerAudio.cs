using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip whipAttackClip;

    public void PlayWhipAttackClip()
    {
        SoundManager.Instance.Play(whipAttackClip);
    }
}
