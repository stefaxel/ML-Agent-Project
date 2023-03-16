using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField, Range(5f, 60f)] private float slopeLimit;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private bool allowJump;
    [SerializeField] private float jumpSpeed;

    public bool IsGrounded { get; private set; }
    public float ForwardInput { get; set; }
    public float TurnInput { get; set; }
    public bool JumpInput { get; set; }

    public bool ButtonInput { get; set; }

    new private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    
    private void Awake()
    {
        rigidbody= GetComponent<Rigidbody>();
        capsuleCollider= GetComponent<CapsuleCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        //PressButtonAction();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        ProcessActions();
    }

    private void CheckGrounded()
    {
        IsGrounded = false;
        float capsuleHeight = Mathf.Max(capsuleCollider.radius * 2f, capsuleCollider.height);
        Vector3 capsuleBottom = transform.TransformPoint(capsuleCollider.center - Vector3.up * capsuleHeight / 2f);
        float radius = transform.TransformVector(capsuleCollider.radius, 0f, 0f).magnitude;
        Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, radius * 5f))
        {
            float normalAngle = Vector3.Angle(hit.normal, transform.up);
            if (normalAngle < slopeLimit)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                if (hit.distance < maxDist)
                    IsGrounded = true;
            }
        }
    }

    private void ProcessActions()
    {
        // Process Turning
        if (TurnInput != 0f)
        {
            float angle = Mathf.Clamp(TurnInput, -1f, 1f) * turnSpeed;
            transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle);
        }
        // Process Movement/Jumping
        if (IsGrounded)
        {
            // Reset the velocity
            rigidbody.velocity = Vector3.zero;
            // Check if trying to jump
            if (JumpInput && allowJump)
            {
                // Apply an upward velocity to jump
                rigidbody.velocity += Vector3.up * jumpSpeed;
            }

            // Apply a forward or backward velocity based on player input
            rigidbody.velocity += transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed;
        }
        else
        {
            // Check if player is trying to change forward/backward movement while jumping/falling
            if (!Mathf.Approximately(ForwardInput, 0f))
            {
                // Override just the forward velocity with player input at half speed
                Vector3 verticalVelocity = Vector3.Project(rigidbody.velocity, Vector3.up);
                rigidbody.velocity = verticalVelocity + transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed / 2f;
            }
        }

        
    }

    //private void PressButtonAction()
    //{
    //    if (Input.GetKeyDown(KeyCode.LeftShift))
    //    {
    //        float interactZone = .5f;
    //        Collider[] collider3DArray = Physics.OverlapBox(transform.position, Vector3.one * interactZone);
    //        foreach (Collider collider in collider3DArray)
    //        {
    //            if(collider.TryGetComponent(out Button activatePlatform))
    //            {
    //                if (activatePlatform.CanUseButton())
    //                {
    //                    activatePlatform.UseButton();
    //                }
    //            }
    //        }
    //    }
    //}
}
