using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prime31.MessageKit;

public class SceneController : MonoBehaviour
{

    Checkpoint checkpoint;
    GameObject[] enemies;
    GameObject player;
    GameObject healthPickup;

    private void Awake()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        player = GameObject.Find("Player");
        healthPickup = GameObject.Find("HealthPickup");
        checkpoint = GameObject.Find("Checkpoint").GetComponent<Checkpoint>();
        MessageKit.addObserver(EventTypes.PLAYER_DIED, LoadTitleScene);
        MessageKit.addObserver(EventTypes.BOSS_DIED, LoadWinScene);

        if (checkpoint.hasReachedCheckpoint == true)
        {
            SetUpSceneForCheckpoint();
        }
    }

    public void LoadWinScene()
    {
        MessageKitManager.clearAllMessageTables();
        checkpoint.hasReachedCheckpoint = false;
        checkpoint.checkpointCollider.isTrigger = true;
        SceneManager.LoadScene(3);
    }

    public static void LoadTitleScene()
    {
        MessageKitManager.clearAllMessageTables();
        SceneManager.LoadScene(0);
    }

    public static void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void SetUpSceneForCheckpoint()
    {
        healthPickup.SetActive(false);

        player.transform.position = new Vector3(checkpoint.transform.position.x, player.transform.position.y, player.transform.position.z);

        foreach (GameObject enemy in enemies)
        {
            if (enemy.transform.position.x < checkpoint.transform.position.x)
            {
                enemy.SetActive(false);
            }
        }
    }
}
