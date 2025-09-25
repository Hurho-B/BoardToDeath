using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RailScript : MonoBehaviour
{
    public bool normalDir;
    public SplineContainer railSpline;
    public float totalSplineLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        railSpline = GetComponent<SplineContainer>();
        totalSplineLength = railSpline.CalculateLength();
    }

    public Vector3 LocalToWorldConversion(float3 localpoint)
    {
        Vector3 worldPos = transform.TransformPoint(localpoint);
        return worldPos;
    }

    public float3 WorldToLocalConversion(Vector3 worldpoint)
    {
        float3 localPos = transform.InverseTransformPoint(worldpoint);
        return localPos;
    }

    public float CalculateTargetRailPoint(Vector3 playerPos, out Vector3 worldPosOnSpline)
    {
        float3 nearestPoint;
        float time;
        SplineUtility.GetNearestPoint(railSpline.Spline, WorldToLocalConversion(playerPos), out nearestPoint, out time);
        worldPosOnSpline = LocalToWorldConversion(nearestPoint);
        return time;
    }

    public void CalculateDirection(float3 railForward, Vector3 playrforward)
    {
        float angle = Vector3.Angle(railForward, playrforward.normalized);
        if (angle > 90f)
        {
            normalDir = false;
        }
        else
        {
            normalDir = true;
        }
    }
}
