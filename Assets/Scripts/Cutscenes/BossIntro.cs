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
    private float triggerDistance = 16;
    [SerializeField]
    private float cameraSpeed = 2f;

    private Vector3 targetPosition;
    private bool cameraMoveEnabled = false;
    
    private void Update()
    {
        if (TriggerBossIntro())
        {
            SetUpCameraMovement();
            if (cameraMoveEnabled)
            {
                camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, cameraSpeed* Time.deltaTime);
                if (camera.transform.position.x == boss.transform.position.x)
                {
                    cameraMoveEnabled = false;
                }
            }
        }
    }

    private void SetUpCameraMovement()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        player.GetComponent<MovementController>().SetDirectionalInput(Vector2.zero);
        camera.GetComponent<CameraFollow>().enabled = false;
        music.GetComponent<AudioSource>().Stop();
        targetPosition = new Vector3(boss.transform.position.x, camera.transform.position.y, camera.transform.position.z);
        cameraMoveEnabled = true;
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
