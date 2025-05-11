// using UnityEngine;

// public class Door : MonoBehaviour
// {
//     [Header("Door Settings")]
//     [SerializeField] private float openAngle = 85f; // 门打开的角度
//     [SerializeField] private float openSpeed = 2f; // 门打开的速度
//     private bool isOpened = false;

//     private HingeJoint hingeJoint;
//     private JointLimits hingeLimits;
    
//     void Start()
//     {
//         // 获取 HingeJoint 组件
//         hingeJoint = GetComponent<HingeJoint>();
        
//         if (hingeJoint == null)
//         {
//             Debug.LogError("No HingeJoint component found on this door.");
//             return;
//         }

//         // 输出调试信息，检查 HingeJoint 的 Axis 和 Anchor
//         Debug.Log($"HingeJoint Axis: {hingeJoint.axis}");
//         Debug.Log($"HingeJoint Anchor: {hingeJoint.anchor}");
//         Debug.Log($"HingeJoint Use Limits: {hingeJoint.useLimits}");
        
//         // 设置初始的 HingeJoint 限制
//         hingeLimits = hingeJoint.limits;
//     }

//     public void Open()
//     {
//         if (hingeJoint == null)
//         {
//             Debug.LogError("HingeJoint component not found!");
//             return;
//         }

//         if (isOpened)
//         {
//             Debug.Log("[Door] The door is already open.");
//             return;
//         }

//         // 打开门的代码
//         Debug.Log("[Door] Door is opening...");

//         // 设置门的最大开角度
//         hingeLimits.max = openAngle;
//         hingeJoint.limits = hingeLimits;

//         // 播放动画或添加扭矩，模拟门打开的过程
//         hingeJoint.useMotor = true;
//         JointMotor motor = hingeJoint.motor;
//         motor.targetVelocity = openSpeed; // 控制门的转动速度
//         motor.force = 100; // 扭矩强度，控制门的开启速度
//         hingeJoint.motor = motor;

//         isOpened = true;

//         // 输出开门的调试信息
//         Debug.Log("[Door] Door opened.");
//     }

//     public void Close()
//     {
//         if (hingeJoint == null)
//         {
//             Debug.LogError("HingeJoint component not found!");
//             return;
//         }

//         if (!isOpened)
//         {
//             Debug.Log("[Door] The door is already closed.");
//             return;
//         }

//         // 关闭门的代码
//         Debug.Log("[Door] Door is closing...");

//         // 设置门的最大开角度为0（即完全关闭）
//         hingeLimits.max = 0f;
//         hingeJoint.limits = hingeLimits;

//         // 关闭门的过程，禁用 motor 或调节速度
//         hingeJoint.useMotor = false;
//         isOpened = false;

//         // 输出关门的调试信息
//         Debug.Log("[Door] Door closed.");
//     }
// }
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
