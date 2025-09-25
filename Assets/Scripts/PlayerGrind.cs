using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    public bool jump;
    public Vector3 input;

    public bool onRail;
    public float grindSpeed;
    public float heightOffset;
    public float timeForFullSpline;
    public float elapsedTime;
    public float lerpSpeed;

    RailScript currentRailScript;
    Rigidbody boardRB;
    CharacterController characterController;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (onRail)
        {
            MovePlayerAlongRail();
        }
    }

    void MovePlayerAlongRail()
    {
        if (currentRailScript != null && onRail)
        {
            float progress = elapsedTime / timeForFullSpline;

            if (progress < 0 || progress > 1)
            {
                ThrowOffRail();
                return;
            }
            float nextTimeNormalised;
            if (currentRailScript.normalDir)
                nextTimeNormalised = (elapsedTime + Time.deltaTime) / timeForFullSpline;
            else
                nextTimeNormalised = (elapsedTime - Time.deltaTime) / timeForFullSpline;

            float3 pos, tangent, up;
            float3 nextPosfloat, nextTan, nextUp;
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextTimeNormalised, out nextPosfloat, out nextTan, out nextUp);

            Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);
            Vector3 nextPos = currentRailScript.LocalToWorldConversion(nextPosfloat);

            transform.position = worldPos + (transform.up * heightOffset);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos - worldPos), lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, up) * transform.rotation, lerpSpeed * Time.deltaTime);

            if (currentRailScript.normalDir)
                elapsedTime += Time.deltaTime;
            else
                elapsedTime -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Rail")
        {
            onRail = true;
            currentRailScript = collision.gameObject.GetComponent<RailScript>();
            CalculateAndSetRailPosition();
        }
    }

    void CalculateAndSetRailPosition()
    {
        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;
        Vector3 splinePoint;
        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedTime = timeForFullSpline * normalisedTime;
        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);
        currentRailScript.CalculateDirection(forward, transform.forward);
        transform.position = splinePoint + (transform.up * heightOffset);
    }

    void ThrowOffRail()
    {
        onRail = false;
        currentRailScript = null;
        transform.position += transform.forward * 1;
    }
}
