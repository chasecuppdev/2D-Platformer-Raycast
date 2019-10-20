using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31.MessageKit;

public class ShakeUIElement : MonoBehaviour
{

    public float duration;
    public float magnitude;
    RectTransform UIElement;

    Vector3 orignalPosition;

    private bool isShaking = false;

    private void Start()
    {
        UIElement = GetComponent<RectTransform>();
        MessageKit<string>.addObserver(EventTypes.UI_ELEMENT_SHAKE_1P, TriggerShake);
    }

    void TriggerShake(string elementName)
    {
        if (gameObject.name == elementName && !isShaking)
        {
            StartCoroutine(Shake(elementName));
        }
    }

    public IEnumerator Shake(string elementName)
    {
        isShaking = true;
        orignalPosition = UIElement.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = orignalPosition.x + Random.Range(-1f, 1f) * magnitude;
            float y = orignalPosition.y + Random.Range(-1f, 1f) * magnitude;

            UIElement.localPosition = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        UIElement.localPosition = orignalPosition;
        isShaking = false;
    }
}
