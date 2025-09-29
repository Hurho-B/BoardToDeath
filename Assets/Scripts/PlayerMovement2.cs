using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement2 : MonoBehaviour
{
    public InputActionAsset InputActions;

    public InputAction moveAction;
    public InputAction lookAction;
    public InputAction jumpAction;
    public InputAction manualAction;
    public InputAction kickflipAction;

    public Vector2 moveAmount;
    public Vector2 lookAmount;

    public Rigidbody rb;

    [Header("Movement Attributes")]
    public float movementSpeed = 20;
    public float rotateSpeed = 1000;
    public float jumpSpeed = 5;

    public float distanceFromGround;
    public bool isGrounded;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        manualAction = InputSystem.actions.FindAction("Manual");
        kickflipAction = InputSystem.actions.FindAction("Kickflip");

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        lookAmount = lookAction.ReadValue<Vector2>();

        if (distanceFromGround <= 1.00f)
        {
            isGrounded = true;
        }

        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
                Jump();
                isGrounded = false;
        }

        if (kickflipAction.WasPressedThisFrame())
        {
            Kickflip();
        }

    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            distanceFromGround = hit.distance;
        }

        Moving();
    }

    public void Jump()
    {
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    public void Moving()
    {
        float horizontal = moveAmount.x;
        float vertical = moveAmount.y;

        //Vector3 camForward = Camera.main.transform.forward;
        //Vector3 camRight = Camera.main.transform.right;

        //camForward.y = 0f;
        //camRight.y = 0f;

        //camForward.Normalize();
        //camRight.Normalize();

        Vector3 moveDirection =  new Vector3(horizontal, 0, vertical).normalized;

        rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
        }
    }

    public void Rotating()
    {

    }

    public void Kickflip()
    {
        Debug.Log("Kickflip Performed");
    }


}
