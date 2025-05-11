// using UnityEngine;
// using UnityEngine.Events;

// public class PuzzleManager : MonoBehaviour
// {
//     [SerializeField] private int numberOfTasksToComplete;
//     private int currentlyCompletedTasks = 0;

//     [Header("Completion Events")]
//     public UnityEvent onPuzzleCompletion;

//     [Header("Audio Settings")]
//     [SerializeField] private AudioClip completionSound;
//     private AudioSource audioSource;

//     [Header("Door Settings")]
//     [SerializeField] private Door doorComponent; // 引用门的脚本组件
//     [SerializeField] private bool useUnityEventForDoor = true; // 选择使用哪种方式控制门

//     private void Awake()
//     {
//         audioSource = GetComponent<AudioSource>();
//         if (audioSource == null)
//         {
//             // 如果没加AudioSource，自动加一个
//             audioSource = gameObject.AddComponent<AudioSource>();
//         }

//         // 如果选择不使用UnityEvent且没有手动指定门组件，尝试自动获取
//         if (!useUnityEventForDoor && doorComponent == null)
//         {
//             doorComponent = FindObjectOfType<Door>();
//             if (doorComponent == null)
//             {
//                 Debug.LogWarning("[PuzzleManager] No Door component found in scene.");
//             }
//         }
//     }

//     public void CompletePuzzleTask()
//     {
//         currentlyCompletedTasks++;
//         CheckForPuzzleCompletion();
//     }

//     private void CheckForPuzzleCompletion()
//     {
//         if (currentlyCompletedTasks >= numberOfTasksToComplete)
//         {   
//             Debug.Log("mission complete!");
//             onPuzzleCompletion.Invoke();
//             PlayCompletionSound();
//             OpenDoor(); // 调用开门方法
//         }
//     }

//     public void PuzzlePieceRemoved()
//     {
//         currentlyCompletedTasks--;
//     }

//     private void PlayCompletionSound()
//     {
//         if (completionSound != null)
//         {
//             audioSource.PlayOneShot(completionSound);
//         }
//         else
//         {
//             Debug.LogWarning("[PuzzleManager] No completion sound assigned.");
//         }
//     }

//     private void OpenDoor()
//     {
//         if (useUnityEventForDoor)
//         {
//             // 使用UnityEvent控制门（在Inspector中设置）
//             // 你可以在onPuzzleCompletion事件中添加门的打开方法
//         }
//         else if (doorComponent != null)
//         {
//             // 直接调用门组件的方法
//             doorComponent.Open();
//             Debug.Log("[PuzzleManager] Door opened.");
//         }
//         else
//         {
//             Debug.LogWarning("[PuzzleManager] No method available to open the door.");
//         }
//     }
// }

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
    [SerializeField] private DoorButton doorButton; // 引用控制门的按钮脚本

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (doorButton == null)
        {
            doorButton = FindObjectOfType<DoorButton>();
            if (doorButton == null)
            {
                Debug.LogWarning("[PuzzleManager] No DoorButton component found in scene.");
            }
        }
    }

    public void CompletePuzzleTask()
    {
        currentlyCompletedTasks++;
        CheckForPuzzleCompletion();
    }

    private void CheckForPuzzleCompletion()
    {
        if (currentlyCompletedTasks >= numberOfTasksToComplete)
        {
            Debug.Log("mission complete!");
            onPuzzleCompletion.Invoke();
            PlayCompletionSound();

            if (doorButton != null)
            {
                doorButton.Activate();
            }
        }
    }

    public void PuzzlePieceRemoved()
    {
        currentlyCompletedTasks--;
    }

    private void PlayCompletionSound()
    {
        if (completionSound != null)
        {
            audioSource.PlayOneShot(completionSound);
        }
        else
        {
            Debug.LogWarning("[PuzzleManager] No completion sound assigned.");
        }
    }
}