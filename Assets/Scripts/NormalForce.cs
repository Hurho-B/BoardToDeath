using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NormalForce : MonoBehaviour
{
    public ApplyGravity gravity;
    public SurfaceAlignment alignmentScript;

    public float[] wheelDistances = new float[4];
    public float groundLevel;

    private void Update()
    {
        CheckGroundDistance();
        ApplyGroundForce();
    }
    public void CheckGroundDistance()
    {
        for (int i = 0; i < alignmentScript.wheelContacts.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(alignmentScript.wheelContacts[i].position, transform.TransformDirection(Vector3.down), out hit))
            {
                // puts normal data into checkedNormals for each wheel to be used for averaging
                wheelDistances[i] = hit.distance;
                groundLevel = hit.distance;

                // visualization stuff
                Debug.DrawRay(alignmentScript.wheelContacts[i].position, transform.TransformDirection(Vector3.down) * hit.distance, UnityEngine.Color.yellow);
                Debug.DrawLine(alignmentScript.wheelContacts[i].position, hit.point, UnityEngine.Color.red);
                Debug.DrawRay(hit.point, hit.normal, UnityEngine.Color.green);
            }
            else
            {
                // nothing should happen.. yet?
                Debug.DrawRay(alignmentScript.wheelContacts[i].position, transform.TransformDirection(Vector3.down) * 1000, UnityEngine.Color.white);
                Debug.Log("Did not Hit");
            }
        }

    }

    public void ApplyGroundForce()
    {
        float distanceSum = 0;
        float averageDistance;

        // adds each Vector3 from checkedNormals to normalSum
        for (int i = 0; i < wheelDistances.Length; i++)
        {
            distanceSum += wheelDistances[i];
        }

        averageDistance = distanceSum / wheelDistances.Length;

        //this is broken, fix later
        if (averageDistance <= groundLevel)
        {
            gravity.enabled = false;
            averageDistance = groundLevel;
            transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
        }

        else
        {
            gravity.enabled = true;
        }

    }
}
