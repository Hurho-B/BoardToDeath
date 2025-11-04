using UnityEngine;

public class AutoForward : MonoBehaviour
{
    public float baseSpeed;
    public float currentSpeed;
    public float defaultMaxSpeed = 20;
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
            maxSpeed-= baseSpeed * 1.5f * Time.deltaTime;
            if (maxSpeed < 0)
            { 
                maxSpeed = 0; 
            }
        }
        else 
        { 
            maxSpeed = defaultMaxSpeed; 
        }
    }
}
