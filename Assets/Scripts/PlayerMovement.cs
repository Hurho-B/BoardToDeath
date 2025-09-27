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
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode controllerJumpKey = KeyCode.JoystickButton14;
    public KeyCode boonKey = KeyCode.L;
    public KeyCode boonResetKey = KeyCode.M;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public bool isGrounded;

    public Transform orientation;
    
    [Header("Animations")]
    public Animator trickAnimations;
    public bool isJumping;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        //Assign RigidBody and freeze rotations to prevent falling through floor
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        //checking if on ground
        grounded = Physics.Raycast(transform.position, Vector3.down, whatIsGround);

        MyInput();
        SpeedControl();

        //drag on ground vs drag in the air
        if (grounded)
            {
            rb.linearDamping = groundDrag;
            isGrounded = true;
            //Debug.Log("Grounded");
            }
        else
            {
            rb.linearDamping = 0;
            isGrounded = false;
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
        if((Input.GetKey(jumpKey) || Input.GetKey(controllerJumpKey)) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //moon jump boon
        if(Input.GetKey(boonKey))
        {
            airMultiplier = 0.5f;
            jumpForce = 15;
        }

        //undo moon jump
        if(Input.GetKey(boonResetKey))
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
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //aerial
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //Max speed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        //reset vertical velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        isJumping = true;

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        Debug.Log("Jumped");
    }

    void ResetJump()
    {
        readyToJump = true;
    }
}
