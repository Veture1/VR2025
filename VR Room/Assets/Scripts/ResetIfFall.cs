using UnityEngine;

public class ResetIfFall : MonoBehaviour
{
    public Vector3 resetPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RecoveryZone"))
        {
            transform.position = resetPosition;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }
    }
}
