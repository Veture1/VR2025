using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 85f; // 门打开的角度
    [SerializeField] private float openSpeed = 2f; // 门打开的速度
    private bool isOpened = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip openSound;
    private AudioSource audioSource;

    private HingeJoint hingeJoint;
    private JointLimits hingeLimits;

    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
        if (hingeJoint == null)
        {
            Debug.LogError("No HingeJoint component found on this door.");
            return;
        }

        hingeLimits = hingeJoint.limits;

        // 获取或添加 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }



        // 输出调试信息
        Debug.Log($"HingeJoint Axis: {hingeJoint.axis}");
        Debug.Log($"HingeJoint Anchor: {hingeJoint.anchor}");
        Debug.Log($"HingeJoint Use Limits: {hingeJoint.useLimits}");
    }

    public void Open()
    {
        if (hingeJoint == null)
        {
            Debug.LogError("HingeJoint component not found!");
            return;
        }

        if (isOpened)
        {
            Debug.Log("[Door] The door is already open.");
            return;
        }

        Debug.Log("[Door] Door is opening...");

        hingeLimits.max = openAngle;
        hingeJoint.limits = hingeLimits;

        hingeJoint.useMotor = true;
        JointMotor motor = hingeJoint.motor;
        motor.targetVelocity = openSpeed;
        motor.force = 100;
        hingeJoint.motor = motor;

        isOpened = true;

        // 播放开门音效
        if (openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
        else
        {
            Debug.LogWarning("[Door] No open sound assigned.");
        }

        Debug.Log("[Door] Door opened.");
    }

    public void Close()
    {
        if (hingeJoint == null)
        {
            Debug.LogError("HingeJoint component not found!");
            return;
        }

        if (!isOpened)
        {
            Debug.Log("[Door] The door is already closed.");
            return;
        }

        Debug.Log("[Door] Door is closing...");

        hingeLimits.max = 0f;
        hingeJoint.limits = hingeLimits;

        hingeJoint.useMotor = false;
        isOpened = false;

        Debug.Log("[Door] Door closed.");
    }
}
