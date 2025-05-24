using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HintMusic : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip Sound;

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

    public void Activate()
    {
        isActivated = true;
        Debug.Log("[DoorButton] Button activated after puzzle completion.");
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (!isActivated)
        {
            if (Sound != null)
            {
                audioSource.PlayOneShot(Sound);
            }

            return;
        }
    }
}
