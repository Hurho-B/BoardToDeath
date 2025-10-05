using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement2 : MonoBehaviour
{
    public InputActionAsset InputActions;

    public InputAction moveAction;
    public InputAction jumpAction;

    public Vector2 moveAmount;

    public Rigidbody rb;

    [Header("Movement Attributes")]
    public float movementSpeed = 20;
    public float rotateSpeed = 1000;
    public float jumpSpeed = 5;

    public float distanceFromGround;
    public bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Start()
    {

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");


        rb.freezeRotation = true;
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();

        if (distanceFromGround <= 1.00f)
        {
            isGrounded = true;
        }

        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
                Jump();
                isGrounded = false;
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
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
    }

    public void Moving()
    {
        float horizontal = moveAmount.x;
        float vertical = moveAmount.y;

        // Get camera directions, but ignore vertical tilt
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Movement relative to camera
        Vector3 moveDirection = (camForward * vertical + camRight * horizontal).normalized;

        // Apply movement
        rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
