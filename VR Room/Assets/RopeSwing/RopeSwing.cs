using UnityEngine;
using UnityEngine.InputSystem;
public class RopeSwing : MonoBehaviour
{
    public Transform startSwingHand;
    public float maxDistance = 10;
    public LayerMask swingableLayer;

    public Transform predictedPoint;
    public bool limitPullDirection = true;
    
    public InputActionProperty swingAction;
    
    public Rigidbody playerRigidbody;
    
    public LineRenderer lineRenderer;
    
    private ConfigurableJoint joint;

    public float springForce =20f;
    public float damper=200f;
    private Vector3 swingPoint;
    private bool HasHit;
    
    private Vector3 yAxis = Vector3.up;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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

    public void StartSwing()
    {
        if (HasHit)
        {
            joint = playerRigidbody.gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;
            // joint.connectedBody = null;

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
            
            SoftJointLimit limit = new SoftJointLimit();
            limit.limit = 0.01f; 
            joint.linearLimit = limit;

            SoftJointLimitSpring limitSpring = new SoftJointLimitSpring();
            limitSpring.spring = springForce;
            limitSpring.damper = damper;
            
            joint.linearLimitSpring = limitSpring;
           
            joint.anchor = Vector3.zero;
            joint.targetPosition = Vector3.zero;
        }
    }

    public void StopSwing()
    {
        Destroy(joint);
        playerRigidbody.linearVelocity = Vector3.zero;
    }

    public void GetSwingPoint()
    {
        if (joint)
        {
            predictedPoint.gameObject.SetActive(false);
            return;
        }

        RaycastHit raycastHit;

        HasHit = Physics.Raycast(startSwingHand.position, startSwingHand.forward, out raycastHit, maxDistance,
            swingableLayer);
        if (HasHit)
        {
            swingPoint = raycastHit.point;
            predictedPoint.gameObject.SetActive(true);
            predictedPoint.position = swingPoint;
            ChangePredictedPointColor(AllowedDirection());
        }
        else
        {
            
            predictedPoint.gameObject.SetActive(false);
        }
    }

    public void DrawRope()
    {
        if(!joint)
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

    public bool AllowedDirection()
    {
        if (!limitPullDirection)
            return true;
        else
        {
            // horizontal/vertical direction (+- 5 degree)
            float cosWithYAxis = Vector3.Dot(startSwingHand.forward.normalized, yAxis);
            if (cosWithYAxis <= 0.08f && cosWithYAxis >= -0.08f)
                return true;
            if (cosWithYAxis >= 0.9 || cosWithYAxis < -0.9)
                return true;
               
            return false;
        }
    }

    public void ChangePredictedPointColor(bool ifValid)
    {
        Color color;
        if (ifValid) color = Color.green;
            else color = Color.white;
        var pointRenderer = predictedPoint.gameObject.GetComponent<Renderer>();
        pointRenderer.material.SetColor("_Color", color);
    }
}
