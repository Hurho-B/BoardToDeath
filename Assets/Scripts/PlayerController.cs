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
    private InputAction m_ollieAction;
    private InputAction m_brakeAction;
    private InputAction m_manualAction;
    private InputAction m_kickflipAction;

    // Grabbing components and data
    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;
    public Animator m_animator;
    private Rigidbody m_rigidbody;
    private Transform m_playerModel;
    private Transform currentRail;

    // Declaring states
    public bool isOnGround = true;
    public bool tiltingLeft;
    public bool tiltingRight;
    public bool doingSquat;
    public bool doingJump;
    public bool doingGrab;
    public bool doingKickflip;
    public bool doingManual;
    public bool doingRailGrind;

    [Header("Physics Values")]
    [Tooltip("Base speed the player will adjust towards.")]
    public float baseCruiseSpeed = 5;
    [Tooltip("Base degrees per second the player will spin..")]
    public float baseRotateSpeed = 5;
    [Tooltip("The rate per second the player will fall at.")]
    public float gravity = 9.8f;
    [Tooltip("The tag that an object must have to be considered ground.")]
    public LayerMask whatIsGround;

    [Header("Ollie Jump Values")]
    [Tooltip("The base ollie height without charging.")]
    public float baseOllieHeight = 5;
    [Tooltip("The rate that the height multiplyer increases by per second.")]
    public float ollieHeightChargeRate;
    [Tooltip("The largest value that the height multiplyer can be.")]
    public float maxOllieHeightMult;
    [Tooltip("How long an ollie must be charging before a height increase begins.")]
    public float delayBeforeOllieHeightCharge;

    [Header("Ollie Speed Values")]
    [Tooltip("The rate that the speed multiplyer increases by per second.")]
    public float ollieSpeedChargeRate;
    [Tooltip("The largest value that the speed multiplyer can be.")]
    public float maxOllieSpeedMult;
    [Tooltip("How long an ollie must be charging before a speed increase begins.")]
    public float delayBeforeOllieSpeedCharge;

    private float delayTime = 0f;
    private float ollieHeightMult = 1f;
    private float ollieSpeedMult = 1f;
    private float newOllieHeight;
    private float currentGravity = 0.0f;
    private float currentSpeed = 0.0f;
    

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
        m_ollieAction = InputSystem.actions.FindAction("Ollie");
        m_brakeAction = InputSystem.actions.FindAction("Brake");
        m_manualAction = InputSystem.actions.FindAction("Manual");
        m_kickflipAction = InputSystem.actions.FindAction("Manual");

        // m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.freezeRotation = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //m_rigidbody.maxLinearVelocity = 15f;
    }

    // Update is called once per frame
    // void FixedUpdate()
    void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = m_moveAction.ReadValue<Vector2>();
        isOnGround = Physics.Raycast(transform.position, Vector3.down, 0.2f, whatIsGround);

        PerformingOllie();

        if (m_manualAction.WasPressedThisFrame() && isOnGround)
        {
            PerformingManual();
        }

        if (m_kickflipAction.WasPressedThisFrame() && !isOnGround)
        {
            doingKickflip = true;
            m_animator.SetBool("kick", doingKickflip);
        }

        SetState();
        ApplyGravity();
        //AcceleratePlayer(baseCruiseSpeed * ollieSpeedMult);
        TurnPlayer();
    }

    public void CheckIfOnGround()
    {
        RaycastHit hit;
        //                  origin              direction     hitinfo  MaxDistance
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.17f))
        {
            Vector3 surfaceNormal = hit.normal; //stores normals of surface hit by raycast
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }
    }

    public void ApplyGravity()
    {
        // Apply gravity to the Player, first check is a psudo grounded check.
        // Rework later when grounded check is good to go.
        float verticalMovement = m_rigidbody.linearVelocity.y;
        if (verticalMovement > -0.01)
            currentGravity = 0.0f;
        else if (currentGravity < gravity)
            currentGravity += gravity * Time.deltaTime;
        m_rigidbody.AddForce(Vector3.down * currentGravity, ForceMode.Force);
    }

    public void AcceleratePlayer(float targetSpeed)
    {
        if (currentSpeed > targetSpeed + 0.2)
            currentSpeed -= targetSpeed * Time.deltaTime;
        else if (currentSpeed < targetSpeed - 0.2)
            currentSpeed += targetSpeed * Time.deltaTime;
        else
            currentSpeed = targetSpeed;
        m_rigidbody.AddForce(transform.forward * currentSpeed, ForceMode.Force);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void TurnPlayer()
    {
        // Vector3 currentVelocity = m_rigidbody.linearVelocity;

        if (m_moveAction.IsPressed())
        {
            Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, baseRotateSpeed, 0) * m_moveAmt.x * Time.deltaTime);
            m_rigidbody.MoveRotation(m_rigidbody.rotation * deltaRotation);
        }
    }

    void PerformingOllie()
    {
        if (m_ollieAction.WasPressedThisFrame())
        {
            if (doingManual)
            {
                doingManual = false;
                // sliderScript.ToggleManual(doingManual);
            }
        }
        else if (m_ollieAction.IsPressed())
        {
            doingSquat = true;
            delayTime += Time.deltaTime;
            if (delayTime > delayBeforeOllieSpeedCharge)
                if (ollieSpeedMult < maxOllieSpeedMult)
                    ollieSpeedMult += ollieSpeedChargeRate * Time.deltaTime;
            if (delayTime > delayBeforeOllieHeightCharge)
                if (ollieHeightMult < maxOllieHeightMult)
                    ollieHeightMult += ollieHeightChargeRate * Time.deltaTime;
        }
        else if (m_ollieAction.WasReleasedThisFrame())
        {
            doingSquat = false;
            doingJump = true;
            m_rigidbody.AddForce(transform.up * (baseOllieHeight * ollieHeightMult), ForceMode.Impulse);
            delayTime = 0f;
            ollieHeightMult = 1f;
            ollieSpeedMult = 1f;
        }
    }

    public void PerformingManual()
    {
        if (isOnGround && !doingManual)
        {
            doingManual = true;
            // sliderScript.ToggleManual(doingManual);
        }
        else if (isOnGround && doingManual)
        {
            doingManual = false;
            // sliderScript.ToggleManual(doingManual);
        }
    }

    public void PerformingGrab()
    {
        // Some lil air thing where you grab da board
    }

    public void PerformingKickflip()
    {
        // Some lil air thing where you grab da board
    }

    public void SetState()
    {
        m_animator.SetFloat("Speed", currentSpeed);
        m_animator.SetBool("DoSquat", doingSquat);
        m_animator.SetBool("IsGrounded", isOnGround);
        m_animator.SetBool("TiltingLeft", tiltingLeft);
        m_animator.SetBool("TiltingRight", tiltingRight);

        m_animator.SetBool("DoingManual", doingManual);
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
