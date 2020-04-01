using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool hasReachedCheckpoint = false;
    public BoxCollider2D checkpointCollider;
    Canvas canvas;


    //Singleton instance
    public static Checkpoint Instance = null;

    //Initialize singleton
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        canvas = GetComponentInChildren<Canvas>();
        checkpointCollider = GetComponent<BoxCollider2D>();

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            hasReachedCheckpoint = true;
            checkpointCollider.isTrigger = false;
            StartCoroutine(FlashCheckpointText());
        }
    }

    private IEnumerator FlashCheckpointText()
    {
        canvas.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(1);
        canvas.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }
}
