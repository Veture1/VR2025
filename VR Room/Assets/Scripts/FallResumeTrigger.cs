using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallResumeTrigger : MonoBehaviour
{
    [SerializeField] private Transform resetPoint;
    [SerializeField] private AudioClip fallSound;
    [SerializeField] private AudioClip popSound;
    [SerializeField] private float volume = 1.0f;

    private HashSet<Collider> processingSet = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        // 只处理带有 Rigidbody 的物体（非触发器区域等）
        if (other.attachedRigidbody == null) return;
        if (!other.CompareTag("Grabbalbe")) return;
        if (!processingSet.Contains(other))
        {
            processingSet.Add(other);
            StartCoroutine(HandleFallAndReset(other));
        }
    }

    private IEnumerator HandleFallAndReset(Collider other)
    {
        Debug.Log($"Handling fall for: {other.name}");
        Vector3 soundPosition = other.transform.position;

        // 播放掉落音效
        if (fallSound != null)
        {
            AudioSource.PlayClipAtPoint(fallSound, soundPosition, volume);
            yield return new WaitForSeconds(fallSound.length);
        }

        Transform t = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform;

        // 传送到复位点
        t.position = resetPoint.position;
        t.rotation = resetPoint.rotation;
        Debug.Log($"Resetting position for: {other.name} to {resetPoint.position}");
        // 清除速度
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null) 
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 播放冒出音效
        if (popSound != null)
        {
            AudioSource.PlayClipAtPoint(popSound, resetPoint.position, volume);
        }

        // 稍微延迟再移除，避免在触发区内反复触发
        yield return new WaitForSeconds(0.5f);
        processingSet.Remove(other);
    }
}
