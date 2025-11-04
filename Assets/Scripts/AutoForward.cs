using UnityEngine;

public class AutoForward : MonoBehaviour
{
    public float baseSpeed;
    public float currentSpeed;
    public float maxSpeed;

    public Animator skateAnimator;

    void Update()
    {
        currentSpeed += baseSpeed * Time.deltaTime;

        if (currentSpeed >= maxSpeed )
        {
            currentSpeed = maxSpeed;
        }

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        skateAnimator.SetFloat("Speed", currentSpeed);

        if (Input.GetKey(KeyCode.S))
        {
            maxSpeed--;
            if (maxSpeed < 0)
            { maxSpeed = 0; }
        }
        else { maxSpeed = 10; }
    }
}
