using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NormalForce : MonoBehaviour
{
    public ApplyGravity gravity;
    public SurfaceAlignment alignmentScript;
    public Vector3 groundCoordinate;
    public float distanceToGround;
    public float groundLevel;

    private void Update()
    {
        CheckGroundDistance();
        ApplyGroundForce();
    }
    public void CheckGroundDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {;
            groundCoordinate = hit.point;
            groundLevel = groundCoordinate.y;

            // visualization stuff
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, UnityEngine.Color.yellow);
        }
        else
        {
            // nothing should happen.. yet?
            Debug.Log("Did not Hit");
        }
    }

    public void ApplyGroundForce()
    {
        if (transform.position.y < groundLevel)
        {
            gravity.enabled = false;
            transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
            
        }
        else
        {
            gravity.enabled = true;
        }

    }
}
