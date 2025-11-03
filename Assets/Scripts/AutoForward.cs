using UnityEngine;

public class AutoForward : MonoBehaviour
{
    public float baseSpeed;
    public float currentSpeed;
    public float maxSpeed;

    void Update()
    {
        currentSpeed += baseSpeed * Time.deltaTime;

        if (currentSpeed >= maxSpeed )
        {
            currentSpeed = maxSpeed;
        }

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

    }
}
