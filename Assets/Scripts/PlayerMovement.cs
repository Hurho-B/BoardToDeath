using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float railSpeed;
    bool readyToJump = true;
    public bool onRail = false;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode controllerJumpKey = KeyCode.JoystickButton14;
    public KeyCode boonKey = KeyCode.L;
    public KeyCode boonResetKey = KeyCode.M;
    public KeyCode kickflip = KeyCode.Mouse1;
    public KeyCode manual = KeyCode.RightShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    [Header("Animations")]
    public Animator trickAnimations;
    public bool isJumping;
    public bool manny;
    public bool kick;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private Transform currentRail;

    void Start()
    {
        //Assign RigidBody and freeze rotations to prevent falling through floor
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        trickAnimations = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        //checking if on ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        CheckIfOnGround();

        MyInput();
        SpeedControl();

        //drag on ground vs drag in the air
        if (grounded)
        {
            rb.linearDamping = groundDrag;
            isJumping = false;
            kick = false;
            //Debug.Log("Grounded");
        }
        else
        {
            rb.linearDamping = 0;
        }

        if (readyToJump == false)
        {
            isJumping = true;
        }

        //manual
        if (Input.GetKeyDown(manual))
        {
            manny = true;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            manny = false;
        }

        //Kickflip Trick
        if (Input.GetKey(kickflip) && !grounded)
        {
            kick = true;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();


    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //jump ability
        if ((Input.GetKey(jumpKey) || Input.GetKey(controllerJumpKey)) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //moon jump boon
        if (Input.GetKey(boonKey))
        {
            airMultiplier = 0.5f;
            jumpForce = 15;
        }

        //undo moon jump
        if (Input.GetKey(boonResetKey))
        {
            airMultiplier = 0.2f;
            jumpForce = 7;
        }
    }

    void MovePlayer()
    {

        //movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //grounded
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //aerial
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //Max speed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        //reset vertical velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        Debug.Log("Jumped");
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    public void CheckIfOnGround()
    {
        //detects whether there is a ground below
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.17f))
        {
            Vector3 surfaceNormal = hit.normal; //stores normals of surface hit by raycast
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rail"))
        {
            gameObject.transform.position = other.transform.position;
            onRail = true;
            rb.useGravity = false;
            isJumping = false;
            currentRail = other.transform;
            Debug.Log("Player entered a rail!");
        }
    }

        private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rail"))
        {
            onRail = false;
            currentRail = null;
            rb.useGravity = true;
            Debug.Log("Player left the rail!");
        }
    }
}
