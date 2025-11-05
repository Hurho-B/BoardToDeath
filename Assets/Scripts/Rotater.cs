using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float turnForce;
    public bool isTurning = false;
    public Rigidbody skaterRB;


    private void FixedUpdate()
    {
        if (isTurning)
        {
            skaterRB.AddTorque(transform.right * turnForce);
        }
    }

    private void Update()
    {
        if(turnForce < 0)
        {
            turnForce *= -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            isTurning = true;
            turnForce = -turnForce;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            isTurning = true;
        }
    }
}
