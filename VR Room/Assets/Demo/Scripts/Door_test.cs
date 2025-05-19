using UnityEngine;

[RequireComponent(typeof(HingeJoint), typeof(Rigidbody))]
public class TestDoor : MonoBehaviour
{
    public static TestDoor Instance { get; private set; }
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 85f;
    [SerializeField] private bool openOnStart = false;

    [Header("Spring Settings")]
    [SerializeField] private float springForce = 100f;
    [SerializeField] private float springDamping = 5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip openSound;
    private AudioSource audioSource;

    private HingeJoint hinge;
    private Rigidbody rb;

    public bool isOpened = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        rb = GetComponent<Rigidbody>();

        if (hinge == null || rb == null)
        {
            Debug.LogError("[TestDoor] Missing components!");
            return;
        }

        // 设置限制范围，限制门转动角度
        JointLimits limits = hinge.limits;
        limits.min = 0f;
        limits.max = openAngle;
        hinge.limits = limits;
        hinge.useLimits = true;

        // 禁用 motor，启用 spring
        hinge.useMotor = false;
        hinge.useSpring = true;

        // 音频设置
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 自动开门（可选）
        if (openOnStart)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    public void Toggle()
    {
        if (isOpened)
            Close();
        else
            Open();
    }

    public void Open()
    {
        JointSpring spring = hinge.spring;
        spring.spring = springForce;
        spring.damper = springDamping;
        spring.targetPosition = openAngle;
        hinge.spring = spring;
        hinge.useSpring = true;

        isOpened = true;

        if (openSound != null)
            audioSource.PlayOneShot(openSound);

        Debug.Log("[TestDoor] Door opened to angle: " + openAngle);
    }

    public void Close()
    {
        JointSpring spring = hinge.spring;
        spring.spring = springForce;
        spring.damper = springDamping;
        spring.targetPosition = 0f;
        hinge.spring = spring;
        hinge.useSpring = true;

        isOpened = false;

        Debug.Log("[TestDoor] Door closed to 0°");
    }
}
