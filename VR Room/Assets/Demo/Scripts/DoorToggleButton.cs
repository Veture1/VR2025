using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorToggleButton : MonoBehaviour
{
    public static DoorToggleButton Instance { get; private set; } // Singleton instance
    [Tooltip("Door to control")]
    [SerializeField] private Door door;

    private bool doorIsOpen = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set singleton instance
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogError("[DoorToggleButton] No XRBaseInteractable component found.");
        }

        if (door == null)
        {
            Debug.LogError("[DoorToggleButton] Door reference not set.");
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

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (door == null) return;

        doorIsOpen = !doorIsOpen;

        if (doorIsOpen)
        {
            door.Open();
            Debug.Log("[DoorToggleButton] Door opened.");
        }
        else
        {
            door.Close();
            Debug.Log("[DoorToggleButton] Door closed.");
        }
    }
}
