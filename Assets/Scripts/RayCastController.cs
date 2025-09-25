using UnityEngine;
using UnityEngine.UIElements;

public class RayCastController : MonoBehaviour
{
    public Rigidbody boardRB;
    public GameObject[] wheelPoints = new GameObject[4];

    public float kickoffSpeed;
    public float maxSpeed;
    public float upForce = 9.81f;

    public float strength = 0f;
    public float length = 0.022f;
    public float dampening;

    private float lastHitDist;
    private float hitDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boardRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetHeight();

        if (Input.GetKey(KeyCode.W))
        {

            boardRB.linearVelocity = new Vector3(0, 0, 1);
        }
        
        RaycastHit hit;

        for (int i = 0; i < wheelPoints.Length; i++)
        {
            Vector3 wheelPosition = wheelPoints[i].transform.position;

            if (Physics.Raycast(wheelPosition, Vector3.down, out hit, length))
            {
                Debug.DrawRay(wheelPosition, Vector3.down * hit.distance, Color.yellow);
                Debug.Log(hit.distance);

                hitDistance = hit.distance;

                float spring = strength * (length - hit.distance) / length;
                float damper = dampening * (lastHitDist - hitDistance);

                float forceAmount = spring + damper;

                boardRB.AddForceAtPosition(Vector3.up * forceAmount, wheelPosition);

                lastHitDist = hitDistance;


                //float forceAmount = 0;

                //forceAmount = strength * (length - hit.distance) / length + (dampening * (lastHitDist - hitDistance));
                //boardRB.AddForceAtPosition(Vector3.up * forceAmount, wheelPosition);

                //lastHitDist = hitDistance;
            }
            else
            {
                lastHitDist = length;
            }
        }
    }

    public void SetHeight()
    {
        boardRB.AddForceAtPosition(upForce * boardRB.mass * Vector3.up, boardRB.centerOfMass);
    }
}
