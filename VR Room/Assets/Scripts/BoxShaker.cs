using UnityEngine;
using System.Collections;

public class BoxShaker : MonoBehaviour
{
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.02f;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = originalPosition + randomOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
