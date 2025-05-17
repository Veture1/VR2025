using UnityEngine;

public class Fader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

}
