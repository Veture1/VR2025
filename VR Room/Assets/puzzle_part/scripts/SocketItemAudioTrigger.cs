using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketItemAudioTrigger : MonoBehaviour
{
    [Header("Target Matching")]
    [SerializeField] private Transform targetObject; // 目标物品（匹配 name 或引用）

    [Header("Audio")]
    [SerializeField] private AudioClip triggerSound;
    private AudioSource audioSource;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    private void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        socket.selectEntered.AddListener(OnObjectSnapped);
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnObjectSnapped);
    }

    private void OnObjectSnapped(SelectEnterEventArgs args)
    {
        Transform placedObject = args.interactableObject.transform;

        // 方式一：直接比引用（推荐）
        if (placedObject == targetObject)
        {
            PlaySound();
        }

        // 方式二：用名称匹配（不推荐但可用）
        // if (placedObject.name == targetObject.name)
        // {
        //     PlaySound();
        // }
    }

    private void PlaySound()
    {
        if (triggerSound != null)
        {
            audioSource.PlayOneShot(triggerSound);
        }
        else
        {
            Debug.LogWarning("[SocketItemAudioTrigger] No sound assigned.");
        }
    }
}
