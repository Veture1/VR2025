using System;
using UnityEngine;

public class EndZoneTrigger : MonoBehaviour
{
    public EndSequenceController sequenceController;
    [Header("Audio")]
    public AudioClip Success; // 拖入开门音效
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && Door.Instance.isOpened)
        {
            Debug.Log("Ending Trigger Zone Entered by " + other.gameObject.name);
            // 播放开门音效
            if (Success != null)
            {
                audioSource.PlayOneShot(Success);
            }
            sequenceController.TriggerEnd();
        }
    }
}
