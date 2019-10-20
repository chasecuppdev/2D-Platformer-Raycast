using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.MessageKit;

public class CameraShake : MonoBehaviour
{
    private void Start()
    {
        MessageKit<float, float>.addObserver(EventTypes.CAMERA_SHAKE_2P, TriggerShake);
    }

    void TriggerShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = orignalPosition.x + Random.Range(-1f, 1f) * magnitude;
            float y = orignalPosition.y + Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        transform.position = orignalPosition;
    }
}
