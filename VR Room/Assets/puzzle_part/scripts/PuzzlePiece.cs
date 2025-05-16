using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzlePiece : MonoBehaviour
{
    [SerializeField] private PuzzleManager linkedPuzzleManager;
    [SerializeField] private Transform CorrectPuzzlePiece;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    private void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        if (socket == null)
        {
            Debug.LogError("[PuzzlePiece] XRSocketInteractor not found on this GameObject!");
        }
        else
        {
            Debug.Log("[PuzzlePiece] XRSocketInteractor successfully assigned.");
        }
    }

    private void OnEnable()
    {
        socket.selectEntered.AddListener(ObjectSnapped);
        socket.selectExited.AddListener(ObjectRemoved);
        Debug.Log("[PuzzlePiece] Listeners added for selectEntered and selectExited.");
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(ObjectSnapped);
        socket.selectExited.RemoveListener(ObjectRemoved);
        Debug.Log("[PuzzlePiece] Listeners removed for selectEntered and selectExited.");
    }

    private void ObjectSnapped(SelectEnterEventArgs args0)
    {
        var snappedObject = args0.interactableObject;
        Debug.Log($"[PuzzlePiece] Object snapped: {snappedObject.transform.name}");

        if (snappedObject.transform.name == CorrectPuzzlePiece.name)
        {
            Debug.Log("[PuzzlePiece] Correct object snapped! Completing puzzle task.");
            linkedPuzzleManager.CompletePuzzleTask();
        }
        else
        {   
            Debug.Log($"snappedObject name: {snappedObject.transform.name}");
            Debug.Log($"CorrectPuzzlePiece name: {CorrectPuzzlePiece.name}");
            Debug.LogWarning("[PuzzlePiece] Incorrect object snapped!");
        }
    }

    private void ObjectRemoved(SelectExitEventArgs args0)
    {
        var removedObject = args0.interactableObject;
        Debug.Log($"[PuzzlePiece] Object removed: {removedObject.transform.name}");

        if (removedObject.transform.name == CorrectPuzzlePiece.name)
        {
            Debug.Log("[PuzzlePiece] Correct object removed! Decreasing puzzle task count.");
            linkedPuzzleManager.PuzzlePieceRemoved();
        }
        else
        {
            Debug.LogWarning("[PuzzlePiece] Incorrect object removed!");
        }
    }
}
