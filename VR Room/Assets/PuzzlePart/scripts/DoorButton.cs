using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorButton : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Door door;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip pressSound;
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
        if (pressSound != null)
        {
            audioSource.PlayOneShot(pressSound);
        }
        if (!isActivated)
        {
            if (errorSound != null)
            {
                audioSource.PlayOneShot(errorSound);
            }

            Debug.Log("[DoorButton] Puzzle not complete yet!");
            return;
        }
        Door.Instance.UnlockPhysics();
        //door.Open();
        Door.Instance.Open();


        //if (successSound != null)
        //{
        //    audioSource.PlayOneShot(successSound);
        //}

        Debug.Log("[DoorButton] Door opened via button.");
    }
}
