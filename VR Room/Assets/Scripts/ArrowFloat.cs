using UnityEngine;

public class ArrowFloat : MonoBehaviour
{
    public static ArrowFloat Instance { get; private set; }
    public float moveDistance = 0.2f; // 单位为 world space 的米
    public float moveDuration = 1f;
    public float pauseTime = 0.2f;

    private Vector3 basePosition;
    private Vector3 targetPosition;
    private bool movingUp = true;
    private float timer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 设置单例实例
        }
        else
        {
            Destroy(gameObject); // 确保只有一个实例
            return;
        }
    }
    private void Start()
    {
        basePosition = transform.position;
        UpdateTargetPosition();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / moveDuration);
        t = Mathf.SmoothStep(0, 1, t);

        transform.position = Vector3.Lerp(
            movingUp ? basePosition : targetPosition,
            movingUp ? targetPosition : basePosition,
            t
        );

        if (timer >= moveDuration + pauseTime)
        {
            movingUp = !movingUp;
            timer = 0f;
        }
    }

    /// <summary>
    /// 移动箭头到新的 world 位置上方
    /// </summary>
    public void MoveTo(Transform target)
    {
        Debug.Log($"Moving to {target.name}");
        basePosition = target.position + Vector3.up * 1.5f; // 上方 0.3 米
        transform.position = basePosition;
        UpdateTargetPosition();
    }

    private void UpdateTargetPosition()
    {
        Debug.Log($"Updating target position: {basePosition}");
        targetPosition = basePosition + Vector3.up * moveDistance;
    }
}
