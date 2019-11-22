using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prime31.MessageKit;

public class SceneController : MonoBehaviour
{
    private void Awake()
    {
        MessageKit.addObserver(EventTypes.PLAYER_DIED, LoadTitleScene);
    }

    static public void LoadTitleScene()
    {
        MessageKitManager.clearAllMessageTables();
        SceneManager.LoadScene(0);
    }

    static public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
