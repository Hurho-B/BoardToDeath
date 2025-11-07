using UnityEngine;

public class SurfaceAlignment : MonoBehaviour
{
    // checkpoints does need to be manually assigned, checkedNormals does not
    // luckily it's only 4 points, located at each wheel
    public Transform[] wheelContacts = new Transform[4];
    public Vector3[] checkedNormals = new Vector3[4];

    [Tooltip("The angle when the board is no longer considered perfectly stable.")]
    [Range(0f, 90f)]
    public float stabilityThreshold1;
    [Tooltip("The angle when the board is no longer considered somewhat stable.")]
    [Range(0f, 90f)]
    public float stabilityThreshold2;

    // Depending on the angle of the board, the board is either in a stable
    // state or an unstable state. The state the board is in will affect
    // what rotation it attempts to take once airborne.

    private void Update()
    {
        FetchWheelNormals();
        AverageWheelNormals();
    }

    public void FetchWheelNormals()
    {
        for (int i = 0; i < wheelContacts.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(wheelContacts[i].position, transform.TransformDirection(Vector3.down), out hit))
            {
                // puts normal data into checkedNormals for each wheel to be used for averaging
                checkedNormals[i] = hit.normal;

                // visualization stuff
                Debug.DrawRay(wheelContacts[i].position, transform.TransformDirection(Vector3.down) * hit.distance, UnityEngine.Color.yellow);
                Debug.DrawLine(wheelContacts[i].position, hit.point, UnityEngine.Color.red);
                Debug.DrawRay(hit.point, hit.normal, UnityEngine.Color.green);
            }
            else
            {
                // nothing should happen.. yet?
                Debug.DrawRay(wheelContacts[i].position, transform.TransformDirection(Vector3.down) * 1000, UnityEngine.Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    public void AverageWheelNormals()
    {
        Vector3 normalSum = Vector3.zero;
        Vector3 averageNormal;

        // adds each Vector3 from checkedNormals to normalSum
        for (int i = 0; i < wheelContacts.Length; i++)
        {
            normalSum += checkedNormals[i];
        }

        averageNormal = normalSum / checkedNormals.Length;

        // aligns local Y (up) to averageNormal direction
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, averageNormal).normalized;
        transform.rotation = Quaternion.LookRotation(forward, averageNormal);
    }
}
