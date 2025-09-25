using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class RayCastController : MonoBehaviour
{
    public Rigidbody boardRB;
    public GameObject[] wheelPoints = new GameObject[4];
    public GameObject forcePoint;

    public float kickoffSpeed;
    public float torqueForce;
    public float maxSpeed;
    public float ollieForce = 30f;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            boardRB.AddForce(Vector3.up * ollieForce, ForceMode.Impulse);
            //boardRB.AddForceAtPosition(transform.up * ollieForce, forcePoint.transform.position);
        }


    }

    void FixedUpdate()
    {
        NormalForces();
        SetHeight();
        Movement();
    }

    public void NormalForces()
    {
        boardRB.AddForceAtPosition(upForce * boardRB.mass * Vector3.up, boardRB.centerOfMass);
    }

    public void SetHeight()
    {
        RaycastHit hit;

        for (int i = 0; i < wheelPoints.Length; i++)
        {
            Vector3 wheelPosition = wheelPoints[i].transform.position;

            if (Physics.Raycast(wheelPosition, Vector3.down, out hit, length))
            {
                Debug.DrawRay(wheelPosition, Vector3.down * hit.distance, Color.yellow);
                //Debug.Log(hit.distance);

                float forceAmount = HooksLawDampen(hit.distance);
                boardRB.AddForceAtPosition(Vector3.up * forceAmount, wheelPosition);
            }
            else
            {
                lastHitDist = length * 1.1f;
            }
        }
    }
    public void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {

            boardRB.AddForce(transform.forward * kickoffSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 1, 0), Space.Self);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, -1, 0), Space.Self);
        }
    }

    private float HooksLawDampen(float hitDistance)
    {
        float forceAmount = strength * (length - hitDistance) + (dampening * (lastHitDist - hitDistance));
        forceAmount = Mathf.Max(0f, forceAmount);
        lastHitDist = hitDistance;

        return forceAmount;
    }
}