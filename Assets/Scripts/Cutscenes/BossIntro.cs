using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntro : MonoBehaviour
{
    [SerializeField]
    private GameObject player; //Hooked up in editor
    [SerializeField]
    private GameObject boss; //Hooked up in editor
    [SerializeField]
    private GameObject camera; //Hooked up in editor
    [SerializeField]
    private GameObject music; //Hooked up in editor
    [SerializeField]
    private float triggerDistance;
    
    private void FixedUpdate()
    {
        if (TriggerBossIntro())
        {
            player.GetComponent<PlayerInput>().enabled = false;
            player.GetComponent<MovementController>().SetDirectionalInput(Vector2.zero);
            camera.GetComponent<CameraFollow>().enabled = false;
            music.GetComponent<AudioSource>().Stop();
        }
    }

    private IEnumerator PanCameraToBoss()
    {
        yield return 0;
    }

    private bool TriggerBossIntro()
    {
        if (boss.transform.position.x - player.transform.position.x < triggerDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
