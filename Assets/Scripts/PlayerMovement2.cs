using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement2 : MonoBehaviour
{
    [Header("Movement Variables")]
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
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Vector3 facingDirection = transform.forward.normalized;
        playerRB.linearVelocity = forwardDirection * moveSpeed;
    }


    public void AutoForward()
    {


    }    
}
