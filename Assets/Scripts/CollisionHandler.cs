using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.DrawLine(transform.position, hit.point);
        }
    }


}
