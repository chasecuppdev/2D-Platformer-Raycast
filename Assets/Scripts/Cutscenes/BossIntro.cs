using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

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
    private GameObject canvas; //Hooked up in editor
    [SerializeField]
    private float triggerDistance = 16;
    [SerializeField]
    private float cameraSpeed = 2f;
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private TileBase currentTile;

    private float moveTime;
    private float pauseTime = 0;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool cameraMoveEnabled = false;

    private bool firstPass = true;
    private bool introFinished = false;

    private void Awake()
    {
        gameObject.GetComponentInChildren<Canvas>().enabled = true;
        boss.GetComponent<SpriteRenderer>().flipX = false;
        BossSettings.FightStarted = false;
    }

    private void Update()
    {
        if (TriggerBossIntro())
        {
            if (firstPass)
            {
                SetUpCameraMovement();
                firstPass = false;
            }
            if (cameraMoveEnabled && !introFinished)
            {
                moveTime += cameraSpeed * Time.deltaTime;
                camera.transform.position = Vector3.Lerp(initialPosition, targetPosition, moveTime);
                if (camera.transform.position.x >= targetPosition.x - 0.1f)
                {
                    pauseTime += Time.deltaTime;
                    if (pauseTime >= 1f)
                    {
                        music.GetComponent<AudioSource>().pitch = 0.85f;
                        music.GetComponent<AudioSource>().Play();
                        moveTime = 0;
                        introFinished = true;
                    }
                }
            }
            else if (cameraMoveEnabled && introFinished)
            {
                moveTime += cameraSpeed * Time.deltaTime;
                camera.transform.position = Vector3.Lerp(targetPosition, initialPosition, moveTime);
                if (camera.transform.position.x == initialPosition.x)
                {
                    cameraMoveEnabled = false;
                    SetWall();
                    ReturnControlToPlayer();
                }
            }
        }
    }

    private void SetWall()
    {
        int xPos = 113;
        int initialYPos = -4;
        Vector3Int positionVector;

        for (int i = initialYPos; i < 8; i++)
        {
            positionVector = new Vector3Int(xPos, i, 0);
            tilemap.SetTile(positionVector, currentTile);
        }
    }

    private void SetUpCameraMovement()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        player.GetComponent<MovementController>().SetDirectionalInput(Vector2.zero);
        camera.GetComponent<CameraFollow>().enabled = false;
        music.GetComponent<AudioSource>().Stop();
        canvas.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        initialPosition = camera.transform.position;
        targetPosition = new Vector3(boss.transform.position.x - 4f, camera.transform.position.y, camera.transform.position.z);
        cameraMoveEnabled = true;
    }

    private void ReturnControlToPlayer()
    {
        camera.GetComponent<CameraFollow>().enabled = true;
        gameObject.GetComponentInChildren<Canvas>().enabled = false;
        player.GetComponent<PlayerInput>().enabled = true;
        boss.GetComponent<SpriteRenderer>().flipX = true;
        BossSettings.FightStarted = true;
        Destroy(gameObject); //We are done with the intro so go ahead and destroy the gameobject
    }

    private bool TriggerBossIntro()
    {
        //This script should be attached to child gameobject of the boss and it's position should be used as the trigger to start the intro
        if (player.transform.position.x > transform.position.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
