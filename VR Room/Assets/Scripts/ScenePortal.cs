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
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    private void FixedUpdate()
    {
        if (TestDoor.Instance.isOpened && !moved)
        {
            ArrowFloat.Instance.MoveTo(transform); // 移动箭头到门上方
            moved = true; // 只移动一次
        }
        if (moved && !TestDoor.Instance.isOpened)
        {
            ArrowFloat.Instance.MoveTo(TestDoorButton.Instance.transform); // 移回去
            moved = false; 
        }
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
        Door.Instance.transform.parent.gameObject.SetActive(false); // 关闭门对象

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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 场景加载完成后，再做淡出
        yield return fader.FadeOut();
        Debug.Log("Fade out done, transition complete");
        gameObject.SetActive(false); // 关闭传送门对象
    }
}
