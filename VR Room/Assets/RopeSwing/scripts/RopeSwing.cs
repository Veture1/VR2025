using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
public class RopeSwing : MonoBehaviour
{   
    [Header("Basic References")]
    public Transform startSwingHand;
    public LayerMask swingableLayer;
    public Transform predictedPoint;
    public LineRenderer lineRenderer;
    public XROrigin playerOrigin;
    
    
    [Header("Input ")]
    public InputActionProperty swingAction;

    [Header("Rope Parameters")]
    public float maxDistance = 10;
    public float springForce =20f;
    public float damper=200f;
    public float shrinkSpeed = 5f;               // m/s
    public float minLimit = 0.3f;                // shortest rope length
    public float playerAngularDrag = 1.5f;
    public float fallLinearDamping = 5f;
    [Range(0, 20)] public float VerticalDegreeAllowance = 5f;
    private float verticalCosAllowance;

    [Header("Optional Haptic Feedback")] 
    public HapticImpulsePlayer haptics;
    [Range(0, 1)]
    public float intensity=0.5f;
    [Range(0, 1)]
    public float duration = 0.15f;    //seconds
    [Header("Sound Effect")]
    public AudioClip fallSound;
    public AudioSource audioSource;
    
    private Rigidbody playerRigidbody;
    private ConfigurableJoint joint;
    private CharacterController characterController;

    private Vector3 swingPoint;
    private bool HasHit;
    private bool limitPullDirection = true;
    private float playerHeight;
    private bool wasGroundedLastFrame = true;
    
    private bool isFallingWithRope = false;


    private Vector3 yAxis = Vector3.up;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = playerOrigin.gameObject.GetComponent<CharacterController>();
        playerRigidbody = playerOrigin.gameObject.GetComponent<Rigidbody>();
        playerHeight = characterController.height;
        Debug.Log("player rigid body y: " + playerRigidbody.position.y);
        Debug.Log("character controller height:" + playerHeight);
        playerRigidbody.linearDamping = 0f;
        playerRigidbody.angularDamping = playerAngularDrag;
        
        verticalCosAllowance = Mathf.Cos(VerticalDegreeAllowance/180*Mathf.PI);
        Debug.Log("Swing target direction considered vertical when cos > " + verticalCosAllowance);
        Debug.Log("drag: " + playerRigidbody.linearDamping);
        Debug.Log("angularDrag: " + playerRigidbody.angularDamping);

    }

    // Update is called once per frame
    void Update()
    {
        GetSwingPoint();
        HandleInput();
        DrawRope();
        
    }

    public void HandleInput()
    {
        if (swingAction.action.WasPressedThisFrame() && AllowedDirection())
        {
            StartSwing();
            TriggerHaptic();
        }
            
        if (joint&& swingAction.action.IsPressed())
        {
            ShrinkLimit();
        }
        
        if (swingAction.action.WasReleasedThisFrame())
        {
            StopSwing();
        }

        if (!swingAction.action.IsPressed())
        {
            HandleLanding();
        }
    }
    public void StartSwing()
    {
        if (HasHit)
        {   
            characterController.enabled = false;
            playerRigidbody.isKinematic = false; // start physics
            
            joint = playerRigidbody.gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;
            joint.anchor = Vector3.zero;    
            
            Vector3 ropeDir = (swingPoint - playerRigidbody.position).normalized;
            joint.axis = playerRigidbody.transform.InverseTransformDirection(ropeDir); 
            joint.secondaryAxis = Vector3.up;

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
            
            SoftJointLimit limit = new SoftJointLimit();
            float dist = Vector3.Distance(playerRigidbody.position, swingPoint);
            limit.limit = dist;
            joint.linearLimit = limit;

            SoftJointLimitSpring limitSpring = new SoftJointLimitSpring();
            limitSpring.spring = springForce;
            limitSpring.damper = damper;
            
            joint.linearLimitSpring = limitSpring;
           
            joint.anchor = Vector3.zero;
            joint.targetPosition = Vector3.zero;
            
        }
    }

    public void ShrinkLimit()
    {
        var lim = joint.linearLimit;
        lim.limit = Mathf.MoveTowards(lim.limit, minLimit, shrinkSpeed * Time.deltaTime);
        joint.linearLimit = lim;
        Vector3 ropeDir = (swingPoint - playerRigidbody.position).normalized;
        joint.axis = playerRigidbody.transform.InverseTransformDirection(ropeDir); 
        joint.secondaryAxis = Vector3.up;
    }
    public void StopSwing()
    {
        Destroy(joint);
        isFallingWithRope = true;
        playerRigidbody.linearDamping = fallLinearDamping; 

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
        if (joint || isFallingWithRope)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startSwingHand.position);
            lineRenderer.SetPosition(1, swingPoint);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }


    public bool AllowedDirection()
    {
        if (!limitPullDirection)
            return true;
        else
        {
            // vertical direction (+- 5 degree)
            return  InVerticalDirection();
        }
    }

    public bool InHorizontalDirection()
    {
        float cosWithYAxis = Vector3.Dot(startSwingHand.forward.normalized, yAxis);
        if (cosWithYAxis <= 0.08f && cosWithYAxis >= -0.08f)
            return true;
        return false;
    }
    public bool InVerticalDirection()
    {
        float cosWithYAxis = Vector3.Dot(startSwingHand.forward.normalized, yAxis);
        if (cosWithYAxis >= verticalCosAllowance|| cosWithYAxis < -verticalCosAllowance)
            return true;
        return false;
    }
    

    public void ChangePredictedPointColor(bool ifValid)
    {
        Color color;
        if (ifValid) color = Color.green;
            else color = Color.white;
        var pointRenderer = predictedPoint.gameObject.GetComponent<Renderer>();
        pointRenderer.material.SetColor("_Color", color);
    }

    public void TriggerHaptic()
    {
        if (intensity > 0 && haptics != null)
            haptics.SendHapticImpulse(intensity, duration);

    }

    private bool isGrounded()
    {
        float rayLength = 0.01f;  
        // Debug.DrawRay(playerRigidbody.position, Vector3.down*rayLength, Color.red);

        return Physics.Raycast(playerRigidbody.position, Vector3.down, rayLength);
    }

    private void HandleLanding()
    {
        bool grounded = isGrounded();

        if (grounded && !wasGroundedLastFrame)//just landed
        {
            toKinematic();       
            triggerFallSound();   
            if (isFallingWithRope)
            {
                isFallingWithRope = false;
                playerRigidbody.linearDamping = 0f; // 恢复正常 damping
            }
        }

        wasGroundedLastFrame = grounded;
    }

    private void toKinematic()
    {
        
        // Debug.Log("Grounded, turn off rigid body");
        playerRigidbody.linearVelocity = Vector3.zero;
        characterController.enabled = true;
        playerRigidbody.isKinematic = true; // turn off physics
        
    }

    private void triggerFallSound()
    {
        if (audioSource && fallSound)
        {
            audioSource.clip = fallSound;
            audioSource.volume = 1f;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
}
