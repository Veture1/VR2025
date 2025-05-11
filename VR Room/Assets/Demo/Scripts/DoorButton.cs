// // DoorButton.cs
// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;

// public class DoorButton : MonoBehaviour
// {
//     [SerializeField] private Door door;
//     [SerializeField] private AudioClip errorSound;
//     [SerializeField] private ParticleSystem successParticles;

//     private bool isActivated = false;
//     private AudioSource audioSource;
//     private XRBaseInteractable interactable;

//     private void Awake()
//     {
//         interactable = GetComponent<XRBaseInteractable>();
//         audioSource = GetComponent<AudioSource>();
//         if (audioSource == null)
//         {
//             audioSource = gameObject.AddComponent<AudioSource>();
//         }
//     }

//     private void OnEnable()
//     {
//         interactable.selectEntered.AddListener(OnButtonPressed);
//     }

//     private void OnDisable()
//     {
//         interactable.selectEntered.RemoveListener(OnButtonPressed);
//     }

//     public void Activate()
//     {
//         isActivated = true;
//         if (successParticles != null)
//         {
//             successParticles.Play();
//         }
//     }

//     private void OnButtonPressed(SelectEnterEventArgs args)
//     {
//         if (!isActivated)
//         {
//             if (errorSound != null)
//             {
//                 audioSource.PlayOneShot(errorSound);
//             }
//             Debug.Log("[DoorButton] Puzzle not complete yet!");
//             return;
//         }

//         door.Open();
//         Debug.Log("[DoorButton] Door opened via button.");
//     }
// }
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorButton : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Door door;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip successSound;

    [Header("VFX")]
    [SerializeField] private ParticleSystem successParticles;

    private bool isActivated = false;
    private AudioSource audioSource;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnButtonPressed);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnButtonPressed);
    }

    // 被 PuzzleManager 调用，激活按钮
    public void Activate()
    {
        isActivated = true;

        if (successParticles != null)
        {
            successParticles.Play();
        }

        Debug.Log("[DoorButton] Button activated after puzzle completion.");
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (!isActivated)
        {
            // 按得太早，播放错误音效
            if (errorSound != null)
            {
                audioSource.PlayOneShot(errorSound);
            }

            Debug.Log("[DoorButton] Puzzle not complete yet!");
            return;
        }

        // 成功开启门
        door.Open();

        // 播放成功音效
        if (successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        Debug.Log("[DoorButton] Door opened via button.");
    }
}
