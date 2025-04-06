using UnityEngine;

public class ThumbPiano : MonoBehaviour
{
    public AudioClip noteSound;
    private AudioSource audioSource;

    void Start()
    {
        // add sound source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = noteSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        // controller enter
        if (other.CompareTag("TouchDetector"))
        {
            audioSource.Play();
        }
    }
}
