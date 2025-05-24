using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class StableGrabInteractable : XRGrabInteractable
{
    private float originalDrag;
    private float originalAngularDrag;

    protected override void Awake()
    {
        base.Awake();
        var rb = GetComponent<Rigidbody>();
        originalDrag = rb.linearDamping;
        originalAngularDrag = rb.angularDamping;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // �ָ� drag������ XR Toolkit ����
        var rb = GetComponent<Rigidbody>();
        rb.linearDamping = originalDrag;
        rb.angularDamping = originalAngularDrag;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // �ٴλָ�����ֹ Toolkit �޸�
        var rb = GetComponent<Rigidbody>();
        rb.linearDamping = originalDrag;
        rb.angularDamping = originalAngularDrag;
    }
}
