using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string targetScene;
    public AudioClip teleportSound;     // 传送音效
    private AudioSource audioSource;    // 播放器
    private bool triggered = false;
    private bool moved = false;
    private void Awake()
    {
        transform.parent = null; // 确保没有父物体
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("Target Scene Awake");
        }
    }
    //private void FixedUpdate()
    //{
    //    //if (TestDoor.Instance.isOpened && !moved)
    //    //{
    //    //    ArrowFloat.Instance.MoveTo(transform); // 移动箭头到门上方
    //    //    moved = true; // 只移动一次
    //    //}
    //    //if (moved && !TestDoor.Instance.isOpened)
    //    //{
    //    //    ArrowFloat.Instance.MoveTo(TestDoorButton.Instance.transform); // 移回去
    //    //    moved = false; 
    //    //}
    //}
    void Start()
    {
        Debug.Log("Target Scene Start");
 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        Debug.Log("Trigger entered by: " + other.name +", " + other.tag );
        if (TestDoor.Instance.isOpened && other.tag == "PortalTrigger")
        {
            triggered = true;
            StartCoroutine(TransitionScene());
        }
    }

    IEnumerator TransitionScene()
    {
        Debug.Log("Transition started");
        //Door.Instance.transform.parent.gameObject.SetActive(false); // 关闭门对象

        // 播放传送音效
        if (teleportSound != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }


        var fader = BlackScreenFader.Instance;
        if (fader == null)
        {
            Debug.LogError("BlackScreenFader instance not found!");
            yield break;
        }

        yield return fader.FadeIn();  // 等黑屏淡入

        Debug.Log("Fade in done, waiting 3 seconds");
        yield return new WaitForSeconds(3f);

        Debug.Log("3 seconds wait done, loading scene");
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        //while (!asyncLoad.isDone)
        //{
        //    yield return null;
        //}
        Debug.Log("Current Scene: " + SceneManager.GetActiveScene().name);
        Debug.Log("Target Scene: " + targetScene);

        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogError("Scene cannot be loaded: " + targetScene);
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = true;
        // Optional: If you want to force the scene to load completely before activating
        asyncLoad.completed += (AsyncOperation op) => {
            Debug.Log("Scene load completed!");
        };
        Debug.Log("Scene loading started...");

        while (!asyncLoad.isDone)
        {
            Debug.Log("Scene loading progress: " + asyncLoad.progress);
            yield return null;
        }

        Debug.Log("Scene loaded!");

        Debug.Log("Fade out done, transition complete");
        // 场景加载完成后，再做淡出
        yield return fader.FadeOut();
        gameObject.SetActive(false); // 关闭传送门对象
    }
}
