using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset InputActions;

    // Mapping Unity actions into player script
    private InputAction m_moveAction;
    private InputAction m_lookAction;
    private InputAction m_jumpAction;
    private InputAction m_brakeAction;
    private InputAction m_ollieAction;
    private InputAction m_manualAction;
    private InputAction m_kickflipAction;

    // Grabbing components and data
    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;
    private Animator m_animator;
    private Rigidbody m_rigidbody;
    private Transform currentRail;

    // Declaring conditional bools
    private bool readyToJump = true;
    public bool isOnGround = true;

    // Declaring animation bools
    public bool doingJump;
    public bool doingGrab;
    public bool doingManual;
    public bool doingKickflip;
    public bool doingRailGrind;

    // Declaring editable stats
    public float baseCruiseSpeed = 5;
    public float baseRotateSpeed = 5;
    public float baseOllieHeight = 5;
    public float airMultiplier;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_lookAction = InputSystem.actions.FindAction("Look");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_brakeAction = InputSystem.actions.FindAction("Brake");
        m_ollieAction = InputSystem.actions.FindAction("Ollie");
        m_manualAction = InputSystem.actions.FindAction("Manual");
        m_kickflipAction = InputSystem.actions.FindAction("Manual");

        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.freezeRotation = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = m_moveAction.ReadValue<Vector2>();

        // drag on ground vs drag in the air, bake into MovePlayer()
        if (isOnGround)
        {
            m_rigidbody.linearDamping = groundDrag;
            doingJump = false;
            doingKickflip = false;
            m_animator.SetBool("isJumping", doingJump);
        }
        else
        {
            m_rigidbody.linearDamping = 0;
        }

        if (m_jumpAction.WasPressedThisFrame() && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (m_manualAction.WasPressedThisFrame() && isOnGround)
        {
            doingManual = true;
            // sliderScript.ToggleManual(doingManual);
            m_animator.SetBool("manny", doingManual);
        }
        else if (m_manualAction.WasPressedThisFrame() && !isOnGround)
        {
            doingManual = false;
            // sliderScript.ToggleManual(doingManual);
            m_animator.SetBool("manny", doingManual);
        }

        if (m_kickflipAction.WasPressedThisFrame() && !isOnGround)
        {
            doingKickflip = true;
            m_animator.SetBool("kick", doingKickflip);
        }

        MovePlayer();
    }

    public void CheckIfOnGround()
    {
        // Checks below all 4 wheels to determine if the skateboard is isOnGround
        // RaycastHit[] wheels = new RaycastHit[4];
        // foreach (RaycaseHit wheel in wheels)
        // {
        // 
        // }

        RaycastHit hit;
        //                  origin              direction     hitinfo  MaxDistance
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.17f))
        {
            Vector3 surfaceNormal = hit.normal; //stores normals of surface hit by raycast
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }

        m_animator.SetBool("grounded", isOnGround);

    }

    public void Jump()
    {
        //reset vertical velocity
        m_rigidbody.linearVelocity = new Vector3(m_rigidbody.linearVelocity.x, 0f, m_rigidbody.linearVelocity.z);

        m_rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        Debug.Log("Jumped");
    }

    public void ResetJump()
    {
        readyToJump = true;
    }
    
    void MovePlayer()
    {
        //movement direction
        Vector3 moveDirection = transform.forward * m_moveAmt.y + transform.right * m_moveAmt.x;

        // If the player is considered on the ground...
        if (isOnGround)
            m_rigidbody.AddForce(moveDirection.normalized * baseCruiseSpeed * 10f, ForceMode.Force);
        // If the player is considered airborne...
        else if (!isOnGround)
            m_rigidbody.AddForce(moveDirection.normalized * baseCruiseSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(m_rigidbody.linearVelocity.x, 0f, m_rigidbody.linearVelocity.z);

        //Max speed
        if (flatVel.magnitude > baseCruiseSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * baseCruiseSpeed;
            m_rigidbody.linearVelocity = new Vector3(limitedVel.x, m_rigidbody.linearVelocity.y, limitedVel.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rail"))
        {
            gameObject.transform.position = other.transform.position;
            doingRailGrind = true;
            m_rigidbody.useGravity = false;
            doingJump = false;
            m_animator.SetBool("isJumping", doingJump);
            currentRail = other.transform;
            Debug.Log("Player entered a rail!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rail"))
        {
            doingRailGrind = false;
            currentRail = null;
            m_rigidbody.useGravity = true;
            Debug.Log("Player left the rail!");
        }
    }

}
