using UnityEngine;

public class SphereController : MonoBehaviour
{
    public float forwardForce = 10f;
    public float torqueForce = 5f;

    public Vector3 linearVelocity;

    public Rigidbody skateboardRB;
    public GameObject forcePoint;

    public void Start()
    {

    }

    public void Update()
    {

    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            skateboardRB.AddForceAtPosition(transform.forward * forwardForce, transform.position + Vector3.down * 0.2f);
        }


        if (Input.GetKey(KeyCode.A))
        {
            skateboardRB.AddTorque(new Vector3(0, -1, 0) * torqueForce);

        }

        if (Input.GetKey(KeyCode.D))
        {
            skateboardRB.AddTorque(new Vector3(0, 1, 0) * torqueForce);
        }

    }
}