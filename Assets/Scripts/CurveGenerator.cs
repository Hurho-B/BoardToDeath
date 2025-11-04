using UnityEngine;

public class CurveGenerator : MonoBehaviour
{
    [SerializeField]private Vector3 basePoint, controlPoint, lipPoint;
    [SerializeField] private int numPoints = 2;
    [SerializeField] private Vector3[] positions;

    private void Start()
    {
        positions = new Vector3[numPoints];
    }

    // Update is called once per frame
    void Update()
    {
        basePoint = transform.position;
        controlPoint = new Vector3(0, 0, 1);
        lipPoint = new Vector3(0, basePoint.y + 1, basePoint.z + 1);

        DrawLinearCurve();
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 point in positions)
        {
            Gizmos.DrawSphere(point, 0.05f);
        }
    }

    private Vector3 CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        return p0 + t * (p1 - p0);
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    private void DrawLinearCurve()
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateQuadraticBezierPoint(t, basePoint, controlPoint, lipPoint);
        }
    }
}
