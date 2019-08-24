using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject enemyPrefab;
    float elapsed = 0;
    // Update is called once per frame

    void Update()
    {
        if (elapsed >= 5f)
        {
            elapsed = 0;
            Instantiate(enemyPrefab, new Vector2(10.02f, -2.73f), Quaternion.identity);
        }
        else
        {
            elapsed += Time.deltaTime; 
        }
    }
}
