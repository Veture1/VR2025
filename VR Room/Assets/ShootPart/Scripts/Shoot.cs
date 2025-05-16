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

    private GameObject currentProjectile = null;

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


    private IEnumerator ScaleUpInitial(Transform target, float targetScale, float duration)
    {
        Vector3 initialScale = Vector3.one * 0.01f;
        Vector3 finalScale = Vector3.one * targetScale;
        target.localScale = initialScale;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = finalScale;
    }

    private bool initialScaleDone = false;
    private IEnumerator SetInitialScaleDoneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        initialScaleDone = true;
    }
    private void StartCharging(InputAction.CallbackContext context)
    {
        isCharging = true;
        chargeStartTime = Time.time;

        //currentProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        //currentProjectile.transform.localScale = Vector3.one * 0.02f; // ��ʼС�ߴ�
        // 创建子弹（但不发射）
        initialScaleDone = false;

        if (currentProjectile == null)
        {
            currentProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            currentProjectile.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(ScaleUpInitial(currentProjectile.transform, 0.02f, 0.3f));
            // 0.01 -> 0.02 是初始出现动画
            // 初始动画完成后，Update用蓄力进度动态放大
            StartCoroutine(SetInitialScaleDoneAfterDelay(0.3f));
        }

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

    private void Update()
    {
        if (isCharging && currentProjectile != null && initialScaleDone)
        {
            float chargeDuration = Mathf.Clamp(Time.time - chargeStartTime, 0f, maxChargeTime);
            float chargePercent = chargeDuration / maxChargeTime;
            float scale = Mathf.Lerp(0.02f, 0.1f, chargePercent);
            currentProjectile.transform.localScale = Vector3.one * scale;

            currentProjectile.transform.position = firePoint.position;
            currentProjectile.transform.rotation = firePoint.rotation;
        }
    }

    private void ReleaseProjectile(InputAction.CallbackContext context)
    {
        if (!isCharging) return;
        if (currentProjectile == null) return;
        isCharging = false;

        float chargeDuration = Mathf.Clamp(Time.time - chargeStartTime, 0f, maxChargeTime);
        float chargePercent = chargeDuration / maxChargeTime;

        // ������Ч����
        if (sfxSource != null && sfxSource.clip == chargeClip)
        {
            fadeOutCoroutine = StartCoroutine(FadeOutAudio(sfxSource, fadeOutDuration));
        }

        // ������Ч
        if (sfxSource != null && fireClip != null)
        {
            sfxSource.PlayOneShot(fireClip);
        }

        //// Instantiate projectile
        //GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        //// Scale based on charge
        //float scale = Mathf.Lerp(0.02f, 0.1f, chargePercent);
        //projectile.transform.localScale = Vector3.one * scale;

        //// Add force based on charge
        //Rigidbody rb = projectile.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    float speed = baseSpeed * Mathf.Lerp(0.5f, 2f, chargePercent);
        //    rb.linearVelocity = firePoint.forward * speed;
        //}
        // ������������

        // 调整最终大小（再保险）
        float scale = Mathf.Lerp(0.02f, 0.1f, chargePercent);
        currentProjectile.transform.localScale = Vector3.one * scale;

        Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            float speed = baseSpeed * Mathf.Lerp(0.5f, 2f, chargePercent);
            rb.linearVelocity = firePoint.forward * speed;
        }

        // �������
        currentProjectile = null;
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
