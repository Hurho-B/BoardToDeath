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
    private InputAction m_manualAction;
    private InputAction m_grabAction;
    private InputAction m_kickflipAction;

    private InputAction m_tricksUp;
    private InputAction m_tricksRight;
    private InputAction m_tricksDown;
    private InputAction m_tricksLeft;

    // Grabbing components and data
    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;
    public Animator m_animator;
    private Rigidbody m_rigidbody;
    private Transform m_playerModel;
    private Transform currentRail;

    // Declaring states
    public bool isOnGround = true;
    private bool tiltingLeft;
    private bool tiltingRight;
    private bool doingSquat;
    private bool doingJump;
    private bool doingGrab;
    private bool doingKickflip;
    private bool doingManual;
    private bool doingRailGrind;

    private bool doingTrick1;
    private bool doingTrick2;
    private bool doingTrick3;
    private bool doingTrick4;

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
        m_manualAction = InputSystem.actions.FindAction("Manual");
        m_kickflipAction = InputSystem.actions.FindAction("Manual");
        m_grabAction = InputSystem.actions.FindAction("Manual");

        m_tricksUp = InputSystem.actions.FindAction("Tricks Up");
        m_tricksRight = InputSystem.actions.FindAction("Tricks Right");
        m_tricksDown = InputSystem.actions.FindAction("Tricks Down");
        m_tricksLeft = InputSystem.actions.FindAction("Tricks Left");

        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.freezeRotation = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //m_rigidbody.maxLinearVelocity = 15f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
        m_lookAmt = m_moveAction.ReadValue<Vector2>();

        isOnGround = Physics.Raycast(transform.position, Vector3.down, 0.2f, whatIsGround);

        // Run a series of checks to see what trick input was made.
        if (m_ollieAction.IsPressed() && isOnGround)
            PerformingOllie();
        else if (m_kickflipAction.WasPressedThisFrame() && !isOnGround)
            PerformingKickflip();
        else if (m_grabAction.WasPressedThisFrame() && !isOnGround)
            PerformingGrab();
        else if (m_manualAction.WasPressedThisFrame() && isOnGround)
            PerformingManual();
        // Next, check to see what physics should be applied.
        if (!doingRailGrind)
            ApplyGravity();
        if (m_moveAction.IsPressed())
            TurnPlayer();
        if (m_moveAction.IsPressed() && m_moveAmt.x < 0)
            AcceleratePlayer(0f);
        else
            AcceleratePlayer(baseCruiseSpeed * ollieSpeedMult);
        // Finally, perform tertiary checks and declare the animation state.
        GrabTrickVariant();
        SetState();
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
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, baseRotateSpeed, 0) * m_moveAmt.x * Time.deltaTime);
        m_rigidbody.MoveRotation(m_rigidbody.rotation * deltaRotation);
    }

    public void PerformingOllie()
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
        if (!doingManual)
        {
            doingManual = true;
            // sliderScript.ToggleManual(doingManual);
        }
        else if (doingManual)
        {
            doingManual = false;
            // sliderScript.ToggleManual(doingManual);
        }
    }

    public void PerformingGrab()
    {
        doingGrab = true;
        doingKickflip = false;
    }

    public void PerformingKickflip()
    {
        doingGrab = false;
        doingKickflip = true;
    }

    public void GrabTrickVariant()
    {
        if (m_tricksUp.WasPressedThisFrame())
        {
            doingTrick1 = true;
            doingTrick2 = false;
            doingTrick3 = false;
            doingTrick4 = false;
        }
        else if (m_tricksUp.WasPressedThisFrame())
        {
            doingTrick1 = false;
            doingTrick2 = true;
            doingTrick3 = false;
            doingTrick4 = false;
        }
        else if (m_tricksUp.WasPressedThisFrame())
        {
            doingTrick1 = false;
            doingTrick2 = false;
            doingTrick3 = true;
            doingTrick4 = false;
        }
        else if (m_tricksUp.WasPressedThisFrame())
        {
            doingTrick1 = false;
            doingTrick2 = false;
            doingTrick3 = false;
            doingTrick4 = true;
        }
    }

    public void SetState()
    {
        // Variables related to basic movement
        m_animator.SetFloat("Speed", currentSpeed);
        m_animator.SetBool("TiltingLeft", tiltingLeft);
        m_animator.SetBool("TiltingRight", tiltingRight);

        // Variables related to active actions
        m_animator.SetBool("DoSquat", doingSquat);
        m_animator.SetBool("IsGrounded", isOnGround);
        m_animator.SetBool("DoingManual", doingManual);
        m_animator.SetBool("DoingGrab", doingGrab);
        m_animator.SetBool("DoingKickflip", doingKickflip);

        // Variables related to different tricks
        m_animator.SetBool("DoingTrick1", doingTrick1);
        m_animator.SetBool("DoingTrick2", doingTrick2);
        m_animator.SetBool("DoingTrick3", doingTrick3);
        m_animator.SetBool("DoingTrick4", doingTrick4);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rail"))
        {
            gameObject.transform.position = other.transform.position;
            doingRailGrind = true;
            doingJump = false;

            // Disable gravity calcs when rail griding
            m_rigidbody.useGravity = false;

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

            // Reenable gravity calcs when railgrinding
            m_rigidbody.useGravity = true;
            Debug.Log("Player left the rail!");
        }
    }

}
