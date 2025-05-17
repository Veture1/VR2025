using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlackScreenFader : MonoBehaviour
{
    public static BlackScreenFader Instance { get; private set; }
    public Image blackImage; // È«ÆÁºÚÉ«Image
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //blackImage = GetComponent<Image>();
        blackImage= transform.GetChild(0).GetComponent<Image>();
        blackImage.gameObject.SetActive(false); // ³õÊ¼Òþ²Ø
    }

    public IEnumerator FadeIn()
    {
        Debug.Log("FadeIn");
        blackImage.gameObject.SetActive(true);
        Color c = blackImage.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            blackImage.color = c;
            yield return null;
        }
        c.a = 1;
        blackImage.color = c;
    }

    public IEnumerator FadeOut()
    {
        Color c = blackImage.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            blackImage.color = c;
            yield return null;
        }
        c.a = 0;
        blackImage.color = c;
        blackImage.gameObject.SetActive(false);
    }
}
