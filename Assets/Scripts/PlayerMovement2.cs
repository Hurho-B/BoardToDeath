using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMovement2 : MonoBehaviour
{
    public Rigidbody playerRB;

    public float forwardAcceleration, reverseAcceleration, maxSpeed, turnInput, turnStrength;

    public Vector3 forwardDirection, movementDirection;

    bool isMoving, isGrounded, isTricking;

    private void Awake()
    {
        forwardAcceleration = 5f;
        maxSpeed = 20f;

        turnStrength = 10f;

        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation = true;
        playerRB.maxLinearVelocity = maxSpeed;
    }

    private void Update()
    {
        turnInput = Input.GetAxis("Horizontal");
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime, 0f));

        // defines "forward" as the direction the player is facing
        // should be mostly independent from actual movement direction
        // for example when rotating in the air
        forwardDirection = transform.forward;
        movementDirection = transform.forward; // placeholder
    }

    private void FixedUpdate()
    {
        AutoForward();
    }

    public void AutoForward()
    {
        // accelerates the player along its local forward axis
        playerRB.AddForce(transform.forward * forwardAcceleration, ForceMode.Acceleration);
        Debug.Log(playerRB.linearVelocity.magnitude);

        // clamps speed
        if (playerRB.linearVelocity.magnitude > maxSpeed)
        {
            playerRB.linearVelocity = forwardDirection * maxSpeed;
        }
    }
}
