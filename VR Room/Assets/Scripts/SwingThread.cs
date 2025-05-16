using UnityEngine;
using UnityEngine.InputSystem;


public class SwingThread : MonoBehaviour
{
    [Header("Swing Settings")]
    public Transform startSwingHand;
    public float maxDistance = 10f;
    public LayerMask swingableLayer;
    public bool limitPullDirection = true;

    [Header("Input")]
    public InputActionProperty swingAction;  // e.g. triggerButton
    public InputActionProperty pullAction;   // e.g. primary2DAxis (Vector2)

    [Header("Physics")]
    public Rigidbody playerRigidbody;
    public float springForce = 20f;
    public float damper = 200f;
    public float pullForce = 80f;

    [Header("Visual")]
    public Transform predictedPoint;
    public LineRenderer lineRenderer;

    private ConfigurableJoint joint;
    private Vector3 swingPoint;
    private bool hasHit;

    private Vector3 yAxis = Vector3.up;
    private Vector3 xAxis = Vector3.right;
    private Vector3 zAxis = Vector3.forward;

    void Update()
    {
        GetSwingPoint();

        if (swingAction.action.WasPressedThisFrame() && AllowedDirection())
        {
            StartSwing();
        }
        else if (swingAction.action.WasReleasedThisFrame())
        {
            StopSwing();
        }

        DrawRope();
    }

    void FixedUpdate()
    {
        if (joint && pullAction.action.ReadValue<Vector2>().y > 0.75f && AllowedPullDirection())
        {
            Vector3 dir = (swingPoint - playerRigidbody.position).normalized;
            playerRigidbody.AddForce(dir * pullForce, ForceMode.Acceleration);
        }
    }

    void StartSwing()
    {
        if (!hasHit) return;

        joint = playerRigidbody.gameObject.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        joint.linearLimit = new SoftJointLimit { limit = 0.01f };
        joint.linearLimitSpring = new SoftJointLimitSpring
        {
            spring = springForce,
            damper = damper
        };

        joint.anchor = Vector3.zero;
        joint.targetPosition = Vector3.zero;
    }

    void StopSwing()
    {
        if (joint) Destroy(joint);
        playerRigidbody.linearVelocity = Vector3.zero;
    }

    void GetSwingPoint()
    {
        if (joint)
        {
            predictedPoint.gameObject.SetActive(false);
            return;
        }

        if (Physics.Raycast(startSwingHand.position, startSwingHand.forward, out RaycastHit hit, maxDistance, swingableLayer))
        {
            hasHit = true;
            swingPoint = hit.point;
            predictedPoint.gameObject.SetActive(true);
            predictedPoint.position = swingPoint;
            ChangePredictedPointColor(AllowedDirection());
        }
        else
        {
            hasHit = false;
            predictedPoint.gameObject.SetActive(false);
        }
    }

    void DrawRope()
    {
        if (!joint)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startSwingHand.position);
            lineRenderer.SetPosition(1, swingPoint);
        }
    }

    bool AllowedDirection()
    {
        if (!limitPullDirection) return true;

        Vector3 forward = startSwingHand.forward.normalized;
        float cosY = Mathf.Abs(Vector3.Dot(forward, yAxis));
        float cosX = Mathf.Abs(Vector3.Dot(forward, xAxis));
        float cosZ = Mathf.Abs(Vector3.Dot(forward, zAxis));

        return (cosY >= 0.95f) || (cosX >= 0.95f) || (cosZ >= 0.95f);
    }

    bool AllowedPullDirection()
    {
        Vector3 dir = (swingPoint - playerRigidbody.position).normalized;
        float cosY = Mathf.Abs(Vector3.Dot(dir, yAxis));
        float cosX = Mathf.Abs(Vector3.Dot(dir, xAxis));
        float cosZ = Mathf.Abs(Vector3.Dot(dir, zAxis));

        return (cosY >= 0.95f) || (cosX >= 0.95f) || (cosZ >= 0.95f);
    }

    void ChangePredictedPointColor(bool valid)
    {
        if (predictedPoint.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.SetColor("_Color", valid ? Color.green : Color.white);
        }
    }
}
