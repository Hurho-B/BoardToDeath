using System.Collections;
using System.Collections.Generic;
using Unity.Mathmatics;
using UnityEngine;
using UnityEngine.Splines;

public class LV_RailController : MonoBehaviour
{
    private bool normalDirection;
    private SplineContainer rail;
    public float railLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rail = GetComponent<SplineContainer>();
        railLength = rail.CalculateLength();
    }

    public Vector3 LocalToWorldConversion(float3 localPoint)
    {
        Vector3 worldPos = transform.TransformPoint(localPoint);
        return worldPos;
    }

    public float3 WorldToLocalConversion(Vector3 worldPoint)
    {
        float3 localPos = transform.InverseTransformPoint(worldPoint);
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

    public void CalculateDirection(float3 railForward, Vector3 playerForward)
    {
        float angle = Vector3.Angle(railForward, playerForward.normalized);
        if (angle > 90f)
            normalDir = false;
        else
            normalDir = true;
    }
}
