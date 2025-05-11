using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Shoot : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public InputActionReference shootAction;
    public float maxChargeTime = 3f;
    public float baseSpeed = 10f;

    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip chargeClip;
    public AudioClip fireClip;
    //public AudioClip hitClip;
    public float fadeOutDuration = 0.3f;

    private float chargeStartTime = 0f;
    private bool isCharging = false;
    private Coroutine fadeOutCoroutine;

    private void OnEnable()
    {
        shootAction.action.performed += StartCharging;
        shootAction.action.canceled += ReleaseProjectile;
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        shootAction.action.performed -= StartCharging;
        shootAction.action.canceled -= ReleaseProjectile;
        shootAction.action.Disable();
    }

    private void StartCharging(InputAction.CallbackContext context)
    {
        isCharging = true;
        chargeStartTime = Time.time;

        if (sfxSource != null && chargeClip != null)
        {
            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);

            sfxSource.clip = chargeClip;
            sfxSource.volume = 1f;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    private void ReleaseProjectile(InputAction.CallbackContext context)
    {
        if (!isCharging) return;
        isCharging = false;

        float chargeDuration = Mathf.Clamp(Time.time - chargeStartTime, 0f, maxChargeTime);
        float chargePercent = chargeDuration / maxChargeTime;

        // 蓄力音效淡出
        if (sfxSource != null && sfxSource.clip == chargeClip)
        {
            fadeOutCoroutine = StartCoroutine(FadeOutAudio(sfxSource, fadeOutDuration));
        }

        // 发射音效
        if (sfxSource != null && fireClip != null)
        {
            sfxSource.PlayOneShot(fireClip);
        }

        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Scale based on charge
        float scale = Mathf.Lerp(0.02f, 0.1f, chargePercent);
        projectile.transform.localScale = Vector3.one * scale;

        // Add force based on charge
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float speed = baseSpeed * Mathf.Lerp(0.5f, 2f, chargePercent);
            rb.linearVelocity = firePoint.forward * speed;
        }
    }

    private IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
}
