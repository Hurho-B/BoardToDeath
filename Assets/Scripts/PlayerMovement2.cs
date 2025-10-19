using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMovement2 : MonoBehaviour
{
    public float moveSpeed, turnRadius;

    public Vector3 forwardDirection = Vector3.forward;

    public Rigidbody playerRB;

    bool isMoving;
    bool isGrounded;
    bool isTricking;

    private void Awake()
    {
        moveSpeed = 20f;
        turnRadius = 10f;

        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation = true;
        playerRB.maxLinearVelocity = 100f;
    }

    private void Start()
    {
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        AutoForward();
        RotateCheck();
    }


    public void AutoForward()
    {
        // provides an acceleration: 
        Vector3 facingDirection = transform.forward.normalized;
        playerRB.linearVelocity += forwardDirection * moveSpeed * Time.deltaTime;
    }

    public void RotateCheck()
    {

    }
}
