using UnityEngine;

public class Projectile : MonoBehaviour
{
    public AudioClip hitClip;
    public ParticleSystem dissolveParticles;
    public float offsetDistance;
    public float dissolveDelay = 1f;
    public float dissolveDuration = 1f;
    public GameObject squashedVisual; // 小坨的形态
    public GameObject normalVisual;   // 正常球体外观

    private Rigidbody rb;
    private bool hasCollided = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        // 播放Hit音效
        if (hitClip != null)
        {
            AudioSource.PlayClipAtPoint(hitClip, transform.position);
        }

        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        var pillar = collision.collider.GetComponent<ResonancePillar>();
        if (pillar != null)
        {
            pillar.RegisterHit(contact.point);
        }

        ShakeOnHit shaker = collision.collider.GetComponent<ShakeOnHit>();
        if (shaker != null && pillar != null)
        {
            shaker.Shake();
        }

        // 粘附在碰撞表面 - 改进部分
        rb.isKinematic = true;
        transform.parent = collision.transform;

        // 计算子弹在表面上的正确位置
        //float offsetDistance = 0.1f; // 增加偏移距离
                                     // 获取子弹的半径（假设是球体碰撞体）
        float projectileRadius = GetComponent<SphereCollider>()?.radius ?? 0.5f;
        // 考虑子弹半径和额外偏移
        transform.position = hitPoint + hitNormal * (projectileRadius + offsetDistance);

        // 调整旋转使子弹"平贴"在表面
        transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

        // 切换成"坨状"外观
        if (normalVisual != null) normalVisual.SetActive(false);
        if (squashedVisual != null) squashedVisual.SetActive(true);

        // 开始溶解流程
        Invoke(nameof(StartDissolve), dissolveDelay);
    }


    private void StartDissolve()
    {
        // 关闭视觉表现（防止和粒子重叠）
        if (normalVisual != null) normalVisual.SetActive(false);
        if (squashedVisual != null) squashedVisual.SetActive(false);

        // 播放粒子溶解动画
        if (dissolveParticles != null)
        {
            dissolveParticles.transform.parent = null;
            dissolveParticles.Play();
            Destroy(dissolveParticles.gameObject, dissolveParticles.main.duration);
        }

        // 最终销毁自身
        Destroy(gameObject);
    }
}

