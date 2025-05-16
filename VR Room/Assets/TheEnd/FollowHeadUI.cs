using UnityEngine;
public class FollowHeadUI : MonoBehaviour
{
    public Transform head; // XR Rig  Camera
    public float distance = 2f;
    public float yOffset = 0.5f;

    void LateUpdate()
    {
        Vector3 targetPos = head.position + head.forward * distance;
        transform.LookAt(head);
        targetPos.y += yOffset;
        transform.position = targetPos;
        transform.forward = -transform.forward; 
    }
}