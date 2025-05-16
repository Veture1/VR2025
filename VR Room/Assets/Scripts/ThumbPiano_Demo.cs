using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThumbPiano_Demo : MonoBehaviour
{
    public AudioClip noteSound;
    private AudioSource audioSource;
    public string noteName;

    public float cooldownTime = 0.3f;
    private bool isCoolingDown = false;

    void Start()
    {
        // add sound source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = noteSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCoolingDown) return;

        // controller enter
        if (other.CompareTag("TouchDetector"))
        {
            audioSource.Play();
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCoolingDown = false;
    }


}


