// using System.Collections;
// using System.Collections.Generic;
// // using Unity.Mathmatics;
// using UnityEngine;
// using UnityEngine.Splines;

// public class LV_RailController : MonoBehaviour
// {
//     private bool normalDirection;
//     private SplineContainer rail;
//     public float railLength;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     private void Start()
//     {
//         rail = GetComponent<SplineContainer>();
//         railLength = rail.CalculateLength();
//     }

//     public Vector3 LocalToWorldConversion(Vector3 localPoint)
//     {
//         Vector3 worldPos = transform.TransformPoint(localPoint);
//         return worldPos;
//     }

//     public Vector3 WorldToLocalConversion(Vector3 worldPoint)
//     {
//         Vector3 localPos = transform.InverseTransformPoint(worldPoint);
//         return localPos;
//     }

//     public float CalculateTargetRailPoint(Vector3 playerPos, out Vector3 worldPosOnSpline)
//     {
//         Vector3 nearestPoint;
//         float time;
//         SplineUtility.GetNearestPoint(railSpline.Spline, WorldToLocalConversion(playerPos), out nearestPoint, out time);
//         worldPosOnSpline = LocalToWorldConversion(nearestPoint);
//         return time;
//     }

//     public void CalculateDirection(Vector3 railForward, Vector3 playerForward)
//     {
//         float angle = Vector3.Angle(railForward, playerForward.normalized);
//         if (angle > 90f)
//             normalDir = false;
//         else
//             normalDir = true;
//     }
// }
