using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TestDoorButton : MonoBehaviour
{
    public static TestDoorButton Instance { get; private set; }
    [SerializeField] private TestDoor testDoor;

    private XRBaseInteractable interactable;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        interactable = GetComponent<XRBaseInteractable>();
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
        if (testDoor == null) return;

        testDoor.Toggle();
        Debug.Log("[TestDoorButton] Door toggled.");
    }
}
