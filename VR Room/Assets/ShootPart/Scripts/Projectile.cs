using UnityEngine;

public class Projectile : MonoBehaviour
{
    public AudioClip hitClip;
    public ParticleSystem dissolveParticles;
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
        // 获取命中点和法线
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        var pillar = collision.collider.GetComponent<ResonancePillar>();
        if (pillar != null)
        {
            pillar.RegisterHit(contact.point);
        }
        // 检查是否有震动脚本
        ShakeOnHit shaker = collision.collider.GetComponent<ShakeOnHit>();
        if (shaker != null)
        {
            shaker.Shake();
            // 保持原状，让它掉下来
        }
        else
        {
            // 粘在碰撞物体上
            rb.isKinematic = true;
            transform.parent = collision.transform;

            transform.position = hitPoint + hitNormal * 0.01f;
            transform.rotation = Quaternion.LookRotation(-hitNormal);

            // 切换成“坨状”外观
            if (normalVisual != null) normalVisual.SetActive(false);
            if (squashedVisual != null) squashedVisual.SetActive(true);
        }

        // 开始溶解流程
        Invoke(nameof(StartDissolve), dissolveDelay);


        //// 判断是否有 Shake 脚本
        //ShakeOnHit shaker = collision.collider.GetComponent<ShakeOnHit>();

        //if (shaker != null)
        //{
        //    shaker.Shake();
        //    // 不粘附，让子弹自然掉落
        //    return;
        //}

        //// 粘附逻辑
        //Rigidbody rb = GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.isKinematic = true; // 停止物理运动
        //}
        //transform.position = hitPoint + hitNormal * 0.01f;
        //transform.rotation = Quaternion.LookRotation(-hitNormal);

        //// 切换成“坨状”外观
        //if (normalVisual != null) normalVisual.SetActive(false);
        //if (squashedVisual != null) squashedVisual.SetActive(true);

        //// 绑定到目标表面（可选，让它跟着动）
        //transform.parent = collision.transform;
        //// 开始溶解流程
        //Invoke(nameof(StartDissolve), dissolveDelay);
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

