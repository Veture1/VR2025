using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MusicPuzzle : MonoBehaviour
{
    public List<string> targetMelody = new List<string> { "K1", "K1", "K4", "K5", "K3"};
    private List<string> currentInput = new List<string>();
    public AudioSource audioSource;          
    public AudioClip successSound;            
    public AudioClip failureSound;            
    public GameObject boxToUnlock;
    public LidSlider lidSlider;
    public BoxShaker boxShaker;
    private bool solved = false;

    [Header("Reward Settings")]
    public GameObject rewardGem; // 3D绿色宝石
    public GameObject Wall; // 宝石预制体
    public ParticleSystem WallDissolve; // 宝石预制体

    void Start()
    {
        // 自动获取宝石的组件
        if (rewardGem != null)
        {
            // 这里可以添加对宝石的初始化代码
            rewardGem.transform.GetChild(7).GetComponent<Light>().enabled = false;
            rewardGem.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; // 启用宝石的碰撞器


        }
    }
    public void OnNotePlayed(string note)
    {
        if (solved) return;

        currentInput.Add(note);
        if (currentInput.Count > targetMelody.Count)
            currentInput.RemoveAt(0);

    if (currentInput.Count == targetMelody.Count)
    {
        if (currentInput.SequenceEqual(targetMelody))
        {
            Debug.Log("🎉 Solved the music puzzle！");
            solved = true;
            rewardGem.transform.GetChild(7).GetComponent<Light>().enabled = true; // 激活宝石的光效
            rewardGem.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None; // 禁用宝石的碰撞器
            Wall.gameObject.SetActive(false); // 隐藏宝石预制体
            WallDissolve.gameObject.GetComponent<ParticleSystem>().Play(); // 播放宝石预制体的粒子效果

                if (audioSource != null && successSound != null)
                audioSource.PlayOneShot(successSound);

            if (lidSlider != null)
                lidSlider.OpenLid();

            if (boxToUnlock != null)
                boxToUnlock.SetActive(true);
        }
        
        else
        {
            Debug.Log("❌ Wrong melody. Resetting...");
            currentInput.Clear();

            if (audioSource != null && failureSound != null)
                audioSource.PlayOneShot(failureSound);

            if (boxShaker != null)
                boxShaker.Shake();
        }
    }

    }
}
