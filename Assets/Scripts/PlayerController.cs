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
    private Animator m_animator;
    private Rigidbody m_rigidbody;
    private Transform m_playerModel;

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
    public float airMultiplier;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;

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

    // [Header("Physics Values")]
    private float gravity = 9.8f;

    private float airDrag;
    private float currentGravity = 0.0f;
    private float currentSpeed = 0.0f;
    public LayerMask whatIsGround;
    

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
        m_ollieAction = InputSystem.actions.FindAction("Jump");
        m_brakeAction = InputSystem.actions.FindAction("Brake");
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
    // void FixedUpdate()
    void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = m_moveAction.ReadValue<Vector2>();
        isOnGround = Physics.Raycast(transform.position, Vector3.down, 0.2f, whatIsGround);

        // drag on ground vs drag in the air, bake into MovePlayer()
        // if (isOnGround)
        // {
        //     m_rigidbody.linearDamping = groundDrag;
        //     doingJump = false;
        //     doingKickflip = false;
        //     m_animator.SetBool("isJumping", doingJump);
        // }
        // else
        // {
        //     m_rigidbody.linearDamping = 0;
        // }

        if (m_ollieAction.WasPressedThisFrame())
        {
            if (doingManual)
            {
                doingManual = false;
                // sliderScript.ToggleManual(doingManual);
                m_animator.SetBool("manny", doingManual);
            }
        }
        else if (m_ollieAction.IsPressed())
        {
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
            m_rigidbody.AddForce(transform.up * (baseOllieHeight * ollieHeightMult), ForceMode.Impulse);
            delayTime = 0f;
            ollieHeightMult = 1f;
            ollieSpeedMult = 1f;
        }

        if (m_manualAction.WasPressedThisFrame())
        {
            if (isOnGround && !doingManual)
            {
                doingManual = true;
                // sliderScript.ToggleManual(doingManual);
                m_animator.SetBool("manny", doingManual);
            }
            else if (isOnGround && doingManual)
            {
                doingManual = false;
                // sliderScript.ToggleManual(doingManual);
                m_animator.SetBool("manny", doingManual);
            }

        }

        if (m_kickflipAction.WasPressedThisFrame() && !isOnGround)
        {
            doingKickflip = true;
            m_animator.SetBool("kick", doingKickflip);
        }

        ApplyGravity();
        AcceleratePlayer(baseCruiseSpeed * ollieSpeedMult);
        TurnPlayer();
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

    public void ApplyGravity()
    {
        // Apply gravity to the Player, first check is a psudo grounded check.
        // Rework later when grounded check is good to go.
        float verticalMovement = m_rigidbody.linearVelocity.y;
        if (verticalMovement > -0.01)
            currentGravity = 0.0f;
        else if (currentGravity < gravity)
            currentGravity += gravity * Time.deltaTime;
        m_rigidbody.AddForce(Physics.gravity * currentGravity, ForceMode.Force);
    }

    public void AcceleratePlayer(float targetSpeed)
    {
        if (currentSpeed > targetSpeed + 0.2)
            currentSpeed -= targetSpeed * Time.deltaTime;
        else if (currentSpeed < targetSpeed - 0.2)
            currentSpeed += targetSpeed * Time.deltaTime;
        else
            currentSpeed = targetSpeed;
        // transform.position += transform.forward * currentSpeed * Time.deltaTime;
        // transform.Translate(transform.forward * currentSpeed * Time.deltaTime);
        m_rigidbody.AddForce(transform.forward * currentSpeed, ForceMode.Force);
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

    void Grab()
    {
        // Some lil air thing where you grab da board
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
