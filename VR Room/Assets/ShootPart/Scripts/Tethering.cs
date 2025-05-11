using UnityEngine;
using UnityEngine.InputSystem;

public class Tethering : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform shootOrigin;
    public float maxDistance = 10f;
    public LayerMask hitLayers;
    public InputActionReference shootAction; // XR Controller Input（按键）

    private Rigidbody attachedRb;
    private bool isAttached = false;

    void Update()
    {
        if (shootAction.action.WasPressedThisFrame())
        {
            TryShootWeb();
        }

        if (shootAction.action.WasReleasedThisFrame())
        {
            Detach();
        }

        if (isAttached && attachedRb != null)
        {
            // 让物体朝手部移动（吸附）
            Vector3 direction = (shootOrigin.position - attachedRb.position);
            attachedRb.linearVelocity = direction * 10f;

            // 更新蛛丝位置
            UpdateLine(attachedRb.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void TryShootWeb()
    {
        Ray ray = new Ray(shootOrigin.position, shootOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                attachedRb = hit.collider.attachedRigidbody;
                isAttached = true;
                lineRenderer.enabled = true;
                UpdateLine(hit.point);
            }
        }
    }

    void Detach()
    {
        isAttached = false;
        attachedRb = null;
        lineRenderer.enabled = false;
    }

    void UpdateLine(Vector3 target)
    {
        lineRenderer.SetPosition(0, shootOrigin.position);
        lineRenderer.SetPosition(1, target);
    }
}
