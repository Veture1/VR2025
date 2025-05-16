using System;
using UnityEngine;

public class EndZoneTrigger : MonoBehaviour
{
    public EndSequenceController sequenceController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Ending Trigger Zone Entered by " + other.gameObject.name);
            sequenceController.TriggerEnd();
        }
    }
}
