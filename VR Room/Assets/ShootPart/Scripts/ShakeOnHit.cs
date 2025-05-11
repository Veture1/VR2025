using UnityEngine;

public class ShakeOnHit : MonoBehaviour
{
    public float shakeDuration = 0.3f;     // 持续时间
    public float shakeMagnitude = 0.05f;   // 抖动强度

    private Vector3 originalPosition;

    public void Shake()
    {
        StopAllCoroutines(); // 防止重复抖动叠加
        StartCoroutine(ShakeCoroutine());
    }

    private System.Collections.IEnumerator ShakeCoroutine()
    {
        originalPosition = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
