using System.Collections.Generic;
using UnityEngine;

public class ResonancePillar : MonoBehaviour
{
    [Header("Note Settings")]
    public int totalNotes = 8;
    public float pillarHeight = 2.0f;
    public List<int> targetSequence = new List<int>(); // 例如：1-3-5

    [Header("Success Action")]
    public GameObject objectToDisableOnSuccess;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip baseNoteClip; // 统一使用一个音

    private float[] notePitches = new float[]
    {
        1.0f,                              // 1
        Mathf.Pow(2f, 2f / 12f),           // 2
        Mathf.Pow(2f, 4f / 12f),           // 3
        Mathf.Pow(2f, 5f / 12f),           // 4
        Mathf.Pow(2f, 7f / 12f),           // 5
        Mathf.Pow(2f, 9f / 12f),           // 6
        Mathf.Pow(2f, 11f / 12f),          // 7
        2.0f                               // 8
    };

    private List<int> currentInput = new List<int>();
    private bool isSolved = false;

    public void RegisterHit(Vector3 hitPoint)
    {
        if (isSolved || audioSource == null || baseNoteClip == null) return;

        // 映射 hit 到音阶编号
        float localY = transform.InverseTransformPoint(hitPoint).y;
        int note = Mathf.Clamp(Mathf.FloorToInt(localY / (pillarHeight / totalNotes)) + 1, 1, totalNotes);

        // 播放音符（通过 pitch 调整）
        audioSource.pitch = notePitches[note - 1];
        audioSource.PlayOneShot(baseNoteClip);

        currentInput.Add(note);

        // 超出目标长度就重置
        if (currentInput.Count > targetSequence.Count)
        {
            currentInput.Clear();
            return;
        }

        // 检查序列匹配
        for (int i = 0; i < currentInput.Count; i++)
        {
            if (currentInput[i] != targetSequence[i])
            {
                currentInput.Clear();
                return;
            }
        }

        // 完成
        if (currentInput.Count == targetSequence.Count)
        {
            PuzzleSolved();
        }
    }

    private void PuzzleSolved()
    {
        isSolved = true;
        Debug.Log("Puzzle Solved!");
        if (objectToDisableOnSuccess != null)
        {
            objectToDisableOnSuccess.SetActive(false);
        }
    }
}
