using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float airMultiplier;
    public float railSpeed;
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

    float horizontalInput;
    float verticalInput;

    Rigidbody rb;

    private Transform currentRail;

    void Update()
    {
        //checking if on ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        CheckIfOnGround();

        MyInput();
        SpeedControl();

        if (readyToJump == false)
        {
            isJumping = true;
        }

    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
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

}
