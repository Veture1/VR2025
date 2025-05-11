using UnityEngine;
using UnityEngine.Events;

public class LidSlider : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0, 0.1f, 0);
    public float openSpeed = 1.0f;
    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool isOpening = false;

    void Start()
    {
        closedPosition = transform.localPosition;
        targetPosition = closedPosition;
    }

    public void OpenLid()
    {
        targetPosition = closedPosition + openOffset;
        isOpening = true;
    }

    void Update()
    {
        if (isOpening)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, openSpeed * Time.deltaTime);
            if (transform.localPosition == targetPosition)
            {
                isOpening = false;
            }
        }
    }

}