using UnityEngine;

public class VectorMovementCheck : MonoBehaviour
{
    public Vector3 lastPos;
    public Vector3 currentPos;

    void Update()
    {
        MoveDirectionCalc();
    }

    public void MoveDirectionCalc()
    {
        lastPos = currentPos;
        currentPos = transform.position;

        Vector3 moveDirection = (currentPos - lastPos);

        Debug.DrawRay(transform.position, moveDirection * 10);

        if (lastPos != currentPos)
        {
            Debug.Log("I am moving.");
        }
    }
}
