using System.Buffers.Text;
using UnityEngine;

public class AutoForward : MonoBehaviour
{
    public float accelRate;
    public float maxSpeed;

    public float currentSpeed;

    public Rigidbody skaterRB;

    public Animator skateAnimator;

    private void Start()
    {
        skaterRB.maxLinearVelocity = maxSpeed;
    }

    void FixedUpdate()
    {
        skaterRB.AddForce(transform.forward * accelRate, ForceMode.Acceleration);
    }

    private void Update()
    {
        currentSpeed = skaterRB.linearVelocity.magnitude;
    }

    //    //currentSpeed += baseSpeed * Time.deltaTime;

    //    if (currentSpeed >= maxSpeed )
    //    {
    //        currentSpeed = maxSpeed;
    //    }

    //    transform.position += transform.forward * currentSpeed * Time.deltaTime;

    //    skateAnimator.SetFloat("Speed", currentSpeed);

    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        maxSpeed--;
    //        if (maxSpeed < 0)
    //        { maxSpeed = 0; }
    //    }
    //    else { maxSpeed = 10; }
    //}
}
