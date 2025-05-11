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
