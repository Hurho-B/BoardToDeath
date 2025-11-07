using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float turnForce;

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(transform.up * -turnForce * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(transform.up * turnForce * Time.deltaTime);
        }
    }
}
