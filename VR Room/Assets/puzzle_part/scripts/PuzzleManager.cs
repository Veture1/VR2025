
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private int numberOfTasksToComplete;
    private int currentlyCompletedTasks = 0;

    [Header("Completion Events")]
    public UnityEvent onPuzzleCompletion;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip completionSound;
    private AudioSource audioSource;

    [Header("Button Settings")]
    [SerializeField] private DoorButton doorButton;

    [Header("Crystal Glow Settings")]
    [SerializeField] private CrystalGlowController[] crystalsToActivate;

    private bool puzzleCompleted = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (doorButton == null)
        {
            doorButton = FindObjectOfType<DoorButton>();
            if (doorButton == null)
                Debug.LogWarning("[PuzzleManager] No DoorButton component found in scene.");
        }
    }

    public void CompletePuzzleTask()
    {
        if (puzzleCompleted) return;

        currentlyCompletedTasks++;
        CheckForPuzzleCompletion();
    }

    private void CheckForPuzzleCompletion()
    {
        if (puzzleCompleted) return;

        if (currentlyCompletedTasks >= numberOfTasksToComplete)
        {
            puzzleCompleted = true; 

            Debug.Log("mission complete!");
            onPuzzleCompletion.Invoke();
            PlayCompletionSound();

            if (doorButton != null)
                doorButton.Activate();

            ActivateCrystals();
        }
    }

    private void ActivateCrystals()
    {
        if (crystalsToActivate == null || crystalsToActivate.Length == 0)
        {
            Debug.LogWarning("[PuzzleManager] No crystals assigned to activate.");
            return;
        }

        foreach (var crystal in crystalsToActivate)
        {
            if (crystal != null)
                crystal.ActivateGlow();
        }
    }

    public void PuzzlePieceRemoved()
    {
        if (puzzleCompleted) return;

        currentlyCompletedTasks--;
    }

    private void PlayCompletionSound()
    {
        if (completionSound != null)
            audioSource.PlayOneShot(completionSound);
        else
            Debug.LogWarning("[PuzzleManager] No completion sound assigned.");
    }
}
