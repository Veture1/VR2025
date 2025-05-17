using UnityEngine;
public class FollowHeadUI : MonoBehaviour
{
    public Transform head; // XR Rig  Camera
    public float distance = 2f;
    public float yOffset = 0.5f;

    void LateUpdate()
    {
        Vector3 targetPos = head.position + head.forward * distance;
        targetPos.y += yOffset;
        transform.position = targetPos; 
        Vector3 lookPos = new Vector3(head.position.x, transform.position.y, head.position.z);
        transform.LookAt(lookPos);
        transform.forward = -transform.forward;

    }
}