using UnityEngine;

public class HideSimulatorOnAndroid : MonoBehaviour
{
    void Awake()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }
}